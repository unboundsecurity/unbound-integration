// Author: Yohei Ueda <yohei@jp.ibm.com>

const fs = require('fs-extra');
const path = require('path');
const program = require('commander');
const sprintf = require('sprintf-js').sprintf;
const yaml = require('js-yaml');

const sdk = require('fabric-client');
const swCryptoSuite = require('fabric-client/lib/impl/CryptoSuite_ECDSA_AES.js');
const pkcs11CryptoSuite = require('fabric-client/lib/impl/bccsp_pkcs11.js');

// Constants
const channelConfigPath = "./channel/mychannel.tx";
const chaincodePath = "example";
const chaincodeId = "example";
const chaincodeVersion = "v1";
const chaincodeType = "golang";

const logger = require('winston');
if (process.env.LOGLEVEL) {
    logger.level = process.env.LOGLEVEL;
}

class CustomCryptoSuite {
    // Derived from fabric-client/lib/api.js
    constructor() {
        this.impl = new swCryptoSuite(256, 'SHA2');
    }

    generateKey(opts) {
        return this.impl.generateKey(opts);
    }

    generateEphemeralKey() {
        return this.impl.generateEphemeralKey();
    }

    deriveKey(key, opts) {
        return this.impl.deriveKey(key, opts);
    }

    importKey(pem, opts) {
        return this.impl.importKey(pem, opts);
    }

    getKey(ski) {
        return this.impl.getKey(ski)
    }

    hash(msg, opts) {
        return this.impl.hash(msg, opts);
    }

    sign(key, digest) {
        return this.impl.sign(key, digest);
    }

    verify(key, signature, digest) {
        return this.impl.verify(key, signature, digest);
    }

    encrypt(key, plaintext, opts) {
        return this.impl.encrypt(key, plaintext, opts);
    }

    decrypt(key, ciphertext, opts) {
        return this.impl.decrypt(key, ciphertext, opts);
    }

    setCryptoKeyStore(cryptoKeyStore) {
        this._cryptoKeyStore = cryptoKeyStore;
        return this.impl.setCryptoKeyStore(cryptoKeyStore)
    }
}

class CustomKeyValueStore {
    // Derived from fabric-client/lib/impl/FileKeyValueStore.js

    constructor(options) {
        if (!options || !options.path) {
            throw new Error('Must provide the path to the directory to hold files for the store.');
        }

        const self = this;
        this._dir = options.path;
        return new Promise(((resolve, reject) => {
            fs.mkdirs(self._dir, (err) => {
                if (err) {
                    logger.error('constructor, error creating directory, code: %s', err.code);
                    return reject(err);
                }
                return resolve(self);
            });
        }));
    }

    getValue(name) {
        logger.debug('getValue', { key: name });

        const self = this;

        return new Promise(((resolve, reject) => {
            const p = path.join(self._dir, name);
            fs.readFile(p, 'utf8', (err, data) => {
                if (err) {
                    if (err.code !== 'ENOENT') {
                        return reject(err);
                    } else {
                        return resolve(null);
                    }
                }
                return resolve(data);
            });
        }));
    }

    setValue(name, value) {
        logger.debug('setValue', { key: name });

        const self = this;

        return new Promise(((resolve, reject) => {
            const p = path.join(self._dir, name);
            fs.writeFile(p, value, (err) => {
                if (err) {
                    reject(err);
                } else {
                    return resolve(value);
                }
            });
        }));
    }
};

class App {
    constructor(user, profile, storePath, channel, org, caServer, orderer, peer, gopath, cryptoSuite) {
        const conf = yaml.safeLoad(fs.readFileSync(profile, 'utf8'));

        function getFromProfile(name) {
            const keys = Object.keys(conf[name] || {});
            if (keys.length === 1) {
                return keys[0];
            }
            if (keys.length < 1) {
                console.error('No %s are defined in %s', name, profile);
            } else if (keys > 1) {
                console.error('Multiple %s are defined in %s. Please use --org option', name, profile);
            }
            throw new Error('Failed to initialize client.');
        }

        if (org === undefined) {
            org = getFromProfile('organizations');
        }
        if (channel === undefined) {
            channel = getFromProfile('channels');
        }
        if (caServer === undefined) {
            caServer = getFromProfile('certificateAuthorities');
        }
        if (orderer === undefined) {
            orderer = getFromProfile('orderers');
        }
        if (peer === undefined) {
            peer = getFromProfile('peers');
        }

        this.user = user;
        this.profile = profile;
        this.storePath = storePath;
        this.channel = channel;
        this.org = org;
        this.caServer = caServer;
        this.orderer = orderer;
        this.peer = peer;
        this.cryptoSuite = cryptoSuite;

        process.env.GOPATH = gopath;
    }

    async getClient() {
        const client = await sdk.loadFromConfig(this.profile);

        // Set up crypto store that contains user's public and private keys
        let cryptoSuite;
        switch (this.cryptoSuite) {
            case 'custom':
                cryptoSuite = new CustomCryptoSuite();
                break;
            case 'sw':
                cryptoSuite = new swCryptoSuite(256, 'SHA2');
                break;
            case 'pkcs11':
                cryptoSuite = new pkcs11CryptoSuite(256, 'SHA2');
                break;
            default:
                throw new Error(sprintf('Unknown crypto suite type: %s', this.cryptoSuite));
        }
        cryptoSuite.setCryptoKeyStore(sdk.newCryptoKeyStore(CustomKeyValueStore, { path: this.storePath + "/cryptostore" }));
        client.setCryptoSuite(cryptoSuite);

        // Set up state store that contains certificates
        client.setStateStore(await new CustomKeyValueStore({ path: this.storePath + "/statestore" }));

        return client;
    }

    async setup() {
        const client = await this.getClient();

        console.log("Creating a channel...");
        const envelope = fs.readFileSync(channelConfigPath);
        const channelConfig = client.extractChannelConfig(envelope);
        const sign = client.signChannelConfig(channelConfig);
        const channelRequest = {
            name: this.channel,
            orderer: client.getOrderer(this.orderer),
            config: channelConfig,
            signatures: [sign],
            txId: client.newTransactionID(true)
        };
        let result = await client.createChannel(channelRequest)
        if (!(result && result.status === 'SUCCESS')) {
            throw new Error(sprintf('Failed to create a channel: %j', result));
        }

        console.log("Joining the channel...");
        const channel = client.getChannel(this.channel);
        const genesisBlockRequest = {
            orderer: client.getOrderer(this.orderer),
            txId: client.newTransactionID(true)
        };
        const genesisBlock = await channel.getGenesisBlock(genesisBlockRequest);
        const joinRequest = {
            txId: client.newTransactionID(true),
            block: genesisBlock
        };
        let results = await channel.joinChannel(joinRequest);
        if (results.filter(res => !res || res instanceof Error || !res.response || res.response.status !== 200).length > 0) {
            throw new Error(sprintf('Failed to join a channel: %j', results));
        }

        console.log("Installing a chaincode...");
        const installRequest = {
            targets: [client.getPeer(this.peer)],
            chaincodePath: chaincodePath,
            chaincodeId: chaincodeId,
            chaincodeVersion: chaincodeVersion,
            chaincodeType: chaincodeType
        };
        results = await client.installChaincode(installRequest);
        if (results[0].filter(res => !res || res instanceof Error || !res.response || res.response.status !== 200).length > 0) {
            throw new Error(sprintf('Failed to install chaincode: %j', results));
        }

        console.log("Instantiating a chaincode...");
        const instantiateRequest = {
            targets: [client.getPeer(this.peer)],
            chaincodeId: chaincodeId,
            chaincodeType: chaincodeType,
            chaincodeVersion: chaincodeVersion,
            args: [],
            txId: client.newTransactionID(true),
        };
        const timeoutMS = 300000;
        const responses = await channel.sendInstantiateProposal(instantiateRequest, timeoutMS);
        if (responses[0].filter(res => !res || res instanceof Error || !res.response || res.response.status !== 200).length > 0) {
            throw new Error(sprintf('Failed to instantiate chaincode: %j', results));
        }
        const transactionRequest = {
            txId: client.newTransactionID(true),
            orderer: client.getOrderer(this.orderer),
            proposalResponses: responses[0],
            proposal: responses[1]
        };
        result = await channel.sendTransaction(transactionRequest);
        if (!(result && result.status === 'SUCCESS')) {
            throw new Error(sprintf('Failed to create a channel: %j', result));
        }
    }

    async register(username) {
        console.log("Creating a new user at CA server...");

        const client = await this.getClient();
        const ca = client.getCertificateAuthority();
        const registrar = ca.getRegistrar();
        const adminUser = await client.setUserContext({ username: registrar.enrollId, password: registrar.enrollSecret });
        const registerRequest = {
            enrollmentID: username,
            affiliation: 'org1.department1'
        };
        const secret = await ca.register(registerRequest, adminUser);

        console.log('Done.\n');
        console.log('enrollmentID: ', username);
        console.log('enrollmentSecret: ', secret);
    }

    async enroll(username, secret) {
        console.log("Generating a pair of public/private keys, and sending Certificate Signing Request with the public key to a CA server...");

        const client = await this.getClient();
        await client.setUserContext({ username: username, password: secret });

        console.log('Done.\n');
        console.log('Public and private keys are stored in crypto store');
        console.log('User state including certificate is stored in state store');
    }

    async execute(fcn, args) {
        console.log("Sending a signed transaction proposal with the certificate of %s...", this.user);

        const client = await this.getClient();
        await client.getUserContext(this.user, true);
        const channel = client.getChannel(this.channel);
        const tx_id = client.newTransactionID();

        const proposalRequest = {
            targets: [client.getPeer(this.peer)],
            chaincodeId: chaincodeId,
            fcn: fcn,
            args: args,
            txId: tx_id
        };
        const responses = await channel.sendTransactionProposal(proposalRequest);
        if (responses[0].filter(res => !res || res instanceof Error || !res.response || res.response.status !== 200).length > 0) {
            throw new Error(sprintf('Failed to call sendTransactionProposal: %j', responses[0]));
        }

        const orderer_request = {
            txId: tx_id,
            orderer: client.getOrderer(this.orderer),
            proposalResponses: responses[0],
            proposal: responses[1],
        };
        const result = await channel.sendTransaction(orderer_request);
        if (!(result && result.status === 'SUCCESS')) {
            throw new Error(sprintf('Failed to create a channel: %j', result));
        }
        console.log("Done.\n");
        console.log("Response status: ", responses[0][0].response.status);
        console.log("Response payload: %s", responses[0][0].response.payload);
    }

    async query(fcn, args) {
        console.log("Sending a signed transaction proposal with the certificate of %s...", this.user);

        const client = await this.getClient();
        await client.getUserContext(this.user, true);
        const channel = client.getChannel(this.channel);
        const tx_id = client.newTransactionID();

        const proposalRequest = {
            targets: [client.getPeer(this.peer)],
            chaincodeId: chaincodeId,
            fcn: fcn,
            args: args,
            txId: tx_id
        };
        const responses = await channel.sendTransactionProposal(proposalRequest);
        if (responses[0].filter(res => !res || res instanceof Error || !res.response || res.response.status !== 200).length > 0) {
            throw new Error(sprintf('Failed to call sendTransactionProposal: %j', responses[0]));
        }

        console.log("Done.\n");
        console.log("Response status: ", responses[0][0].response.status);
        console.log("Response payload: %s", responses[0][0].response.payload);
    }
}

function dispatch(methodName) {
    return function () {
        const app = new App(program.user, program.profile, program.storePath, program.channel, program.org, program.caServer, program.orderer, program.peer, program.gopath, program.cryptoSuite);
        const method = app[methodName];
        const args = Array.prototype.slice.call(arguments, 0, arguments.length - 1);
        method.apply(app, args).then(() => {
            process.exit(0);
        }).catch(err => {
            console.error('Process exited with error: ', err);
            process.exit(1);
        });
    };
}

function main() {
    program
        .option('--user [name]', "User name", "admin")
        .option('--profile [path]', "Connection profile", "./connection-profile.yaml")
        .option('--channel [string]', "Channel name")
        .option('--org [string]', "Organization name")
        .option('--peer [string]', "Peer name")
        .option('--orderer [string]', "Orderer name")
        .option('--ca-server [string]', "CA server name")
        .option('--store-path [path]', "File store path", "./store")
        .option('--crypto-suite [type]', "sw, pkcs11, or custom", "custom")
        .option('--gopath [path]', "gopath for chaincode", "./chaincode/go");

    program.command('setup').action(dispatch("setup"));
    program.command('register <username>').action(dispatch("register"));
    program.command('enroll <username> <secret>').action(dispatch("enroll"));
    program.command('execute <function> [args...]').action(dispatch("execute"));
    program.command('query <function> [args...]').action(dispatch("query"));

    program.parse(process.argv);
}

main();

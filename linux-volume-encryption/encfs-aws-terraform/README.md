# Volume encryption using the encfs user space file system

### Advantages of encfs

By default, all FUSE-based filesystems are visible only to the user who mounted them. This way, we can automatically prevent the **root** user from viewing and changing unencrypted files.

## Prerequisites

1. Create a UKC partition and sign-in to it
1. Create an RSA key in Unbound UI
1. Create an Ephemeral Client
1. Get activation code and use it in the VM setup
1. Generate an encrypted passphrase:
   1. Generate a random key (passphrase): ``head -c 128 /dev/random > /tmp/key``
   2. Encrypt the passphrase: ``ucl encrypt -n KEY_NAME  -i /tmp/key -o /tmp/key.out --user USERNAME -w PASSWORD``
   3. Convert to base 64 printable format: ``base64 /tmp/key.out | tr -d "\n"``
1. Update the `variables.tf` file. Set the `enc_keyphase` variable.
1. Run the ``terraform init`` command.

### How it works
Most of the interesting code is found in the ``userdata.sh`` file used for EC2 VM setup. It works as following:

1. Create and mount '/data' path pointing to external volume - provided by AWS.
2. Install needed packages (wget, fuse, jq, openssl, tinyxml2, fuse-encfs).
3. Download and install ekm-client rpm package.
4. Register ephemeral client.
6. Decrypt the ``passphrase`` using the ``ucl decrypt -i /data/keyphase.enc -o /dev/stdout`` command and use it for the ``encfs``.

## Setup

Run the ``terraform apply`` command.

## Troubleshooting
1. Destroy everything with this command: ``terraform destroy``.
1. View VM installation script (`userdata.sh`) progress (after ssh to VM): ``tail -f /var/log/cloud-init-output.log``

## Old method to encrypt the key

1. Create a UKC partition and sign-in to it
1. Create an RSA key in Unbound UI
1. Create an Ephemeral Client
1. Get activation code and use it in the VM setup
1. Export public key using ucl command: ``ucl export -n key-name --output cert.pub``
1. Generate an encrypted passphrase:
   1. Generate a random key (passphrase): ``head -c 128 /dev/random``
   2. Encrypt the passphrase: ``openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep``
   3. Convert to base 64 printable format: ``base64``
   4. In one command:
   ```
   head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64
   ```
1. Update the `variables.tf` file.
1. Run the ``terraform init`` command.

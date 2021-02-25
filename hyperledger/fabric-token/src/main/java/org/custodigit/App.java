/* Copyright (C) Custodigit AG
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vadim Uvin <vadim.uvin@custodigit.com>, August 2019
 */

package org.custodigit;

import org.example.client.CAClient;
import org.example.client.ChannelClient;
import org.example.client.FabricClient;
import org.example.user.UserContext;
import org.example.util.Util;
import org.hyperledger.fabric.sdk.*;

import java.util.Collection;


public class App
{
    private static final String CA_URL = "http://193.246.47.177:30012";
    private static final String ORG = "peer-org0.blockchain-factory.ch";
    private static final String AFFILIATION = "org1";
    private static final String CA_ADMIN = "admin";
    private static final String ORG0_ADMIN = "admin-org0";
    private static final String ORG0_ADMIN_CN = "Admin@peer-org0.blockchain-factory.ch";
    private static final String CHANNEL_NAME = "channel1";
    private static final String PEER_URL = "grpc://193.246.47.177:30014";
    private static final String EVENTHUB_URL = "grpc://193.246.47.177:30015";
    private static final String ORDERER_NAME = "orderer.orderer-org0.blockchain-factory.ch";
    private static final String ORDERER_URL = "grpc://193.246.47.177:30013";
    private static final String PEER_NAME = "peer0.peer-org0.blockchain-factory.ch";
    private static final String CC_NAME = "cstd-token";


    private static UserContext loadUserContext(String affiliation, String username) {
        try {
            return Util.readUserContext(AFFILIATION, username);
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    public static UserContext registerNewUser() {
        try {
            String caUrl = CA_URL;
            CAClient caClient = new CAClient(caUrl, null);
            UserContext admin = Util.readUserContext(AFFILIATION, CA_ADMIN);
            caClient.setAdminUserContext(admin);

            UserContext userContext = new UserContext();
            String name = "user" + System.currentTimeMillis();
            userContext.setAffiliation(AFFILIATION);
            userContext.setMspId(ORG);
            userContext.setName(name);

            String eSecret = caClient.registerUser(name, AFFILIATION);
            return caClient.enrollUser(userContext, eSecret);

        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    public static String query(ChannelClient channelClient, String function, String[] args) {
        try {
            Collection<ProposalResponse> responsesQuery = channelClient.queryByChainCode(CC_NAME, function, args);
            for (ProposalResponse pres : responsesQuery) {
                String stringResponse = new String(pres.getChaincodeActionResponsePayload());
                return stringResponse;
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    public static void invoke(ChannelClient channelClient, String function, String[] args) {
        try {
            FabricClient fabClient = channelClient.getFabClient();
            TransactionProposalRequest request = fabClient.getInstance().newTransactionProposalRequest();
            ChaincodeID ccid = ChaincodeID.newBuilder().setName(CC_NAME).build();
            request.setChaincodeID(ccid);
            request.setFcn(function);
            request.setArgs(args);
            request.setProposalWaitTime(1000);

            Collection<ProposalResponse> responses = channelClient.sendTransactionProposal(request);
            for (ProposalResponse res: responses) {
                ChaincodeResponse.Status status = res.getStatus();
                System.out.println(status);
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static ChannelClient getChannelClient(UserContext context) {
        try {
            FabricClient fabClient = new FabricClient(context);
            ChannelClient channelClient = fabClient.createChannelClient(CHANNEL_NAME);
            Channel channel = channelClient.getChannel();
            Peer peer = fabClient.getInstance().newPeer(PEER_NAME, PEER_URL);
            EventHub eventHub = fabClient.getInstance().newEventHub("eventhub01", EVENTHUB_URL);
            Orderer orderer = fabClient.getInstance().newOrderer(ORDERER_NAME, ORDERER_URL);
            channel.addPeer(peer);
            channel.addEventHub(eventHub);
            channel.addOrderer(orderer);
            channel.initialize();
            return channelClient;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }

    public static void sleep() {
        try {
            Thread.sleep(5000);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void printBalance(ChannelClient client, String username) {
        String result = query(client, "balance", new String[] {String.format("{\"user\": \"%s\"}", username)});
        System.out.println("Balance: " + result);
    }

    public static void main(String[] args)
    {
        ChannelClient channelClient = getChannelClient(loadUserContext(AFFILIATION, ORG0_ADMIN));
        UserContext newUser = registerNewUser();
        System.out.println("Registered new user: " + newUser.getName());

        System.out.println("Balance before transfer:");
        printBalance(channelClient, ORG0_ADMIN_CN);
        printBalance(channelClient, newUser.getName());

        invoke(channelClient, "transfer", new String[] {String.format("{\"to\": \"%s\", \"value\": 10}", newUser.getName())});
        sleep();

        System.out.println("Balance after transfer:");
        printBalance(channelClient, ORG0_ADMIN_CN);
        printBalance(channelClient, newUser.getName());

        System.out.println("Sending the tokens back...");
        ChannelClient newUserClient = getChannelClient(loadUserContext(AFFILIATION, newUser.getName()));
        invoke(newUserClient, "transfer", new String[] {String.format("{\"to\": \"%s\", \"value\": 10}", ORG0_ADMIN_CN)});

        sleep();

        System.out.println("Balance after transfer:");
        printBalance(channelClient, ORG0_ADMIN_CN);
        printBalance(channelClient, newUser.getName());
    }
}
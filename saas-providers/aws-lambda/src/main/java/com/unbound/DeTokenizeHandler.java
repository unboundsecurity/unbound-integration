package com.unbound;

import com.amazonaws.services.lambda.runtime.Context;
import com.amazonaws.services.lambda.runtime.LambdaLogger;
import com.amazonaws.services.lambda.runtime.RequestHandler;
import com.dyadicsec.advapi.SDEKey;
import com.dyadicsec.advapi.SDESessionKey;
import com.unbound.model.request.TokenizationRequest;
import com.unbound.model.response.TokenizationResponse;

public class DeTokenizeHandler implements RequestHandler<TokenizationRequest, TokenizationResponse>, TokHandler{

    @Override
    public TokenizationResponse handleRequest(TokenizationRequest request, Context context) {
        LambdaLogger logger = context.getLogger();
        try {

            loadUnbound();

            SDEKey sdeKey = SDEKey.findKey(request.getKeyId());
            SDESessionKey sessionKey = sdeKey.generateSessionKey(SDEKey.PURPOSE_EMAIL_ENC, request.getTweak());
            String token = sessionKey.decryptEMailAddress(request.getValue());

            return
                    new TokenizationResponse()
                            .token(token)
                            .tweak(request.getTweak())
                            .uid(request.getKeyId());

        } catch (Exception e) {
            logger.log(e.toString());
            e.printStackTrace();
            return
                    new TokenizationResponse()
                            .error(e.toString());
        }
    }
}

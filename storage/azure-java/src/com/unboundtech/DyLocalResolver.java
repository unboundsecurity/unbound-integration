/**
 * Copyright 2018 Dyadic Security Ltd.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License
 */

package com.unboundtech;

import com.microsoft.azure.keyvault.core.IKey;
import com.microsoft.azure.keyvault.core.IKeyResolver;
import org.apache.commons.lang3.concurrent.ConcurrentUtils;

import java.io.IOException;
import java.security.*;
import java.util.concurrent.Future;

public class DyLocalResolver  implements IKeyResolver {


    @Override
    public Future<IKey> resolveKeyAsync(String s) {
        KeyStore ks = null;
        try {
            DyRSAKey dyRSAKey = new DyRSAKey(s);
            dyRSAKey.load();
            return ConcurrentUtils.constantFuture((IKey) dyRSAKey);

        } catch (GeneralSecurityException | IOException e) {
            e.printStackTrace();
        }
        return null;
    }
}

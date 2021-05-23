import base64
import sys

from pypkcs11.defines import *
from pypkcs11.defaults import *
from pypkcs11.default_templates import CKM_ECDSA_KEY_PAIR_GEN_PUBTEMP, CKM_ECDSA_KEY_PAIR_GEN_PRIVTEMP
from pypkcs11.session_management import c_initialize, c_open_session, login
from pypkcs11.key_generator import c_generate_key, c_destroy_object
from pypkcs11.token_management import get_token_by_label
from pypkcs11.misc import c_digestkey, c_create_object
from pypkcs11.object_attr_lookup import c_find_objects
from pypkcs11.encryption import c_wrap_key

PLAINTEXT_SECRET_HASH_B64 = "BYOK-hash.b64"
ENCRYPTED_SECRET_B64 = "BYOK-wrapped.b64"

if len(sys.argv) != 3:
    print('Generates a random 256-bit AES key, its hash, and its wrapped key material using the imported SF key and the UKC partition name')
    print('Usage : python ub_byok.py &lt;Partition name&gt; &lt;Imported wrapping-key name&gt; ')
    sys.exit(-1)

partition = sys.argv[1]
pubKeyName = sys.argv[2]

c_initialize()
rv, slot = get_token_by_label(partition)
assert rv == CKR_OK
rv, session = c_open_session(slot)
assert rv == CKR_OK, "c_open_session"
# perform login, if needed
# rv = login(session, slot, '')
# assert rv == CKR_OK, "login " + hex(rv)

# get UID of the wrapping key
template = {CKA_ID: pubKeyName}
rv, keys = c_find_objects(session, template, 1)
assert rv == CKR_OK and keys
wrap_key = keys.pop(0)

# generate AES key to be used by SF
key_template = {CKA_CLASS: CKO_SECRET_KEY, CKA_KEY_TYPE: CKK_AES, CKA_TOKEN: True,
                CKA_VALUE_LEN: 32, CKA_ENCRYPT: True, CKA_DECRYPT: True, CKA_EXTRACTABLE: True, CKA_WRAP_WITH_TRUSTED: False , CKA_DERIVE: True}
rv, aes_key = c_generate_key(session, CKM_AES_KEY_GEN, key_template)
assert rv == CKR_OK

# digest the AES key
rv, digest = c_digestkey(session, aes_key, CKM_SHA256)
assert rv == CKR_OK

# wrap the AES key
wrap_mech = {"mech_type": CKM_RSA_PKCS_OAEP,
             "params": {'hashAlg': CKM_SHA_1,
                        'mgf': CKG_MGF1_SHA1,
                        'source': 0,
                        'sourceData': list(),
                        'ulSourceDataLen': 0}}
rv, wrapped = c_wrap_key(session, wrap_key, aes_key, wrap_mech)
assert rv == CKR_OK


with open(ENCRYPTED_SECRET_B64, 'wb') as f:
    f.write(base64.b64encode(wrapped))
with open(PLAINTEXT_SECRET_HASH_B64, 'wb') as f:
    f.write(base64.b64encode(digest))

print('Generated files: ' + ENCRYPTED_SECRET_B64 +
      ', ' + PLAINTEXT_SECRET_HASH_B64)
print('Both of these should be uploaded to Salesforce.')

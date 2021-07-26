#
# DigiCert CA integeration script - Part I
# Creates Certificate Request and submits it to DigiCert
#
import http.client
import json
import base64

import sys
from pypkcs11.session_management import c_initialize, c_open_session, login
from pypkcs11.defines import *
from pypkcs11.key_generator import c_generate_key_pair
from pypkcs11.default_templates import CKM_RSA_PKCS_KEY_PAIR_GEN_PUBTEMP, CKM_RSA_PKCS_KEY_PAIR_GEN_PRIVTEMP
from pypkcs11.unbound import dyc_create_x509_request
from pypkcs11.token_management import get_token_by_label

organizationId = 123456  # DigiCert ID of your organisation
devkey = 'YOUR-DC-DEVKEY'

userPin = ''
c_initialize()
rv, slot = get_token_by_label(b'demopartition')
assert rv == CKR_OK
rv, session = c_open_session(slot, CKF_SERIAL_SESSION | CKF_RW_SESSION)
assert rv == CKR_OK
rv = login(session, 0, userPin)
assert rv == CKR_OK
csrSubj = 'C=IL, L=Petah Tikva, O=Unbound Security Ltd, OU=Test1, CN=www.unboundsecurity.com'
pbkey_template, prkey_template = (
    CKM_RSA_PKCS_KEY_PAIR_GEN_PUBTEMP, CKM_RSA_PKCS_KEY_PAIR_GEN_PRIVTEMP)
pbkey_template[CKA_LABEL] = b'DemoPublicKey'
prkey_template[CKA_LABEL] = b'DemoPrivateKey'
ret, pub_key, priv_key = c_generate_key_pair(session,
                                             mechanism=CKM_RSA_PKCS_KEY_PAIR_GEN,
                                             pbkey_template=pbkey_template,
                                             prkey_template=prkey_template)
assert rv == CKR_OK

ret, csrDer = dyc_create_x509_request(session, priv_key, CKM_SHA256, csrSubj)
assert rv == CKR_OK
csr = base64.b64encode(csrDer)

orderCert = {'certificate': {'common_name': 'www.unboundsecurity.com', 'csr': csr.decode("utf-8"), 'signature_hash': 'sha256'},
             'organization': {'id': organizationId}, 'validity_years': 2}
body = json.dumps(orderCert)

conn = http.client.HTTPSConnection('www.digicert.com')
headers = {'Content-Type': 'application/json',
           'Accept': 'application/json', 'X-DC-DEVKEY': devkey}
conn.request("POST", "/services/v2/order/certificate/ssl_plus", body, headers)
resp = conn.getresponse()
assert resp.status == 201, resp.reason

data = resp.read()
jata = json.loads(data)
orderId = jata['id']
certificateId = jata['certificate_id']
print('Order submitted OK, OrderId = %d CertificateId = %d' %
      (orderId, certificateId))

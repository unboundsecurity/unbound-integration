#
# DigiCert CA integeration script - Part II
# Downloads certificate from DigiCert and imports it
#
import http.client
import json
import zipfile
import OpenSSL
import io
import sys

from pypkcs11.session_management import c_initialize, c_open_session, login
from pypkcs11.defines import *
from pypkcs11.misc import c_create_object
from pypkcs11.attributes import to_byte_array
from pypkcs11.token_management import get_token_by_label

devkey = 'YOUR-DC-DEVKEY'
if len(sys.argv) != 2:
    print("Usage: " + sys.argv[0] + " <order_id>")
    exit(0)
orderId = sys.argv[1]
userPin = ''
c_initialize()
rv, slot = get_token_by_label(b'demopartition')
assert rv == CKR_OK
rv, session = c_open_session(slot, CKF_SERIAL_SESSION | CKF_RW_SESSION)
assert rv == CKR_OK
assert login(session, 0, userPin) == CKR_OK

conn = http.client.HTTPSConnection('www.digicert.com')
headers = {"Accept": "application/json", 'X-DC-DEVKEY': devkey}

# Obtain Certificate ID from Order ID
conn.request("GET", "/services/v2/order/certificate/" +
             orderId, None, headers)
resp = conn.getresponse()
assert resp.status == 200, resp.reason
data = resp.read()
jata = json.loads(data)
certificateId = jata['certificate']['id']
print('Certificate ID:', certificateId)

# Download certificate
headers = {'Accept': '*/*', 'X-DC-DEVKEY': devkey}
conn.request("GET", "/services/v2/certificate/" +
             str(certificateId) + '/download/platform', None, headers)
resp = conn.getresponse()
assert resp.status == 200, resp.reason
data = resp.read()

# Unpack the received ZIP archive
zf = zipfile.ZipFile(io.BytesIO(data))
info_list = zf.infolist()
zf.extractall()

# Save certificate chain to the server
for item in info_list:
    if not item.filename.endswith('.crt'):
        continue
    certFile = open(item.filename, "r")
    cert_file = certFile.read()
    cert_pem = OpenSSL.crypto.load_certificate(
        OpenSSL.crypto.FILETYPE_PEM, cert_file)
    cert_der = OpenSSL.crypto.dump_certificate(
        OpenSSL.crypto.FILETYPE_ASN1, cert_pem)

    certBuf = bytearray(certFile.read(), 'utf8')

    certTempl = {CKA_TOKEN: True, CKA_PRIVATE: False,
                 CKA_CLASS: CKO_CERTIFICATE, CKA_CERTIFICATE_TYPE: CKC_X_509, CKA_VALUE: cert_der}
    rv, hCert = c_create_object(session, certTempl)
    assert rv == CKR_OK
    print("Certificate imported, handle: ", hex(hCert))

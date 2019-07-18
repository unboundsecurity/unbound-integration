import binascii
import os
import os.path as path
import argparse

from pypkcs11.session_management import(
  c_initialize, c_open_session, c_close_session, login)

import pypkcs11.session_management as sess_mng
from pypkcs11.defines
import *
from pypkcs11.misc
import c_create_object
from pypkcs11.encryption
import c_encrypt, c_decrypt
from pypkcs11.object_attr_lookup
import c_find_objects
from pypkcs11.object_attr_lookup
import c_get_attribute_value

parser = argparse.ArgumentParser(formatter_class = argparse.ArgumentDefaultsHelpFormatter,
  conflict_handler = 'resolve',
  description = ''
  'Sync offline target keys with UKC.'
  '')
parser.add_argument('--creds', help = 'UKC credentials (json)')
args = parser.parse_args()

keys_dir = path.join(os.environ['HOME'], '.docker/trust/private')

key_files = [f
  for f in os.listdir(
    keys_dir) if path.isfile(path.join(keys_dir, f))
]

rv = c_initialize()
assert rv == CKR_OK

rv, slot_list = sess_mng.c_get_slot_list()
assert rv == CKR_OK and slot_list
slot = slot_list[0]

rv, session = c_open_session(slot)
assert rv == CKR_OK, 'c_open_session failed'

rv = login(session, slot, args.creds)
assert rv == CKR_OK

rv, handles = c_find_objects(session, {
  CKA_CLASS: CKO_DATA
}, 1000)
assert rv == CKR_OK

# 1. Download missing
for handle in handles:
  rv, attrs = c_get_attribute_value(
    session, handle, {
      CKA_ID: None,
      CKA_VALUE: None
    })
assert rv == CKR_OK

fname = attrs[CKA_ID].decode('utf-8') + '.key'
if fname not in key_files:
  with open(path.join(keys_dir, fname), 'wb') as f:
  f.write(binascii.unhexlify(attrs[CKA_VALUE]))
else :
  key_files.remove(fname)

# 2. Upload new keys
for key_file in key_files:
  with open(path.join(keys_dir, key_file), 'r') as f:
  data = f.read()
if 'role: root' in data:
  continue# skip root keys
rv, _ = c_create_object(
  session, {
    CKA_CLASS: CKO_DATA,
    CKA_TOKEN: True,
    CKA_ID: key_file[: -4].encode('ascii'),
    CKA_VALUE: binascii.hexlify(data.encode('ascii'))
  })
assert rv == CKR_OK
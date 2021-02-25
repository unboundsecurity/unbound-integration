#!/usr/bin/python
# -*- coding: utf-8 -*-
from __future__ import print_function
import sys
import binascii
import os
import os.path as path
import argparse

from pypkcs11.session_management import c_initialize, c_open_session, \
    c_close_session, login

import pypkcs11.session_management as sess_mng
from pypkcs11.defines import *
from pypkcs11.misc import c_create_object
from pypkcs11.encryption import c_encrypt, c_decrypt
from pypkcs11.object_attr_lookup import c_find_objects
from pypkcs11.object_attr_lookup import c_get_attribute_value

def check_rv(rv,op):
    if (rv != CKR_OK): halt(op,rv)

def halt(message,rv):
    print("Error : " + message, file=sys.stderr)
    sys.exit(rv)

parser = \
    argparse.ArgumentParser(formatter_class=argparse.ArgumentDefaultsHelpFormatter,
                            conflict_handler='resolve',
                            description='''Sync offline target keys with UKC.'''
                            )
parser.add_argument('--creds', help='UKC credentials (json)')
args = parser.parse_args()

keys_dir = path.join(os.environ['HOME'], '.docker/trust/private')

key_files = [f for f in os.listdir(keys_dir)
             if path.isfile(path.join(keys_dir, f))]

rv = c_initialize()
check_rv(rv,"c_initialize()")

(rv, slot_list) = sess_mng.c_get_slot_list()
check_rv(rv,"c_get_slot_list()")

slot = slot_list[0]  

(rv, session) = c_open_session(slot)
check_rv(rv,"c_open_session()")

rv = login(session, slot, args.creds)
check_rv(rv,"login()")

(rv, handles) = c_find_objects(session, {CKA_CLASS: CKO_DATA}, 1000)
check_rv(rv,"c_find_objects()")

# 1. Download missing keys
for handle in handles:
    (rv, attrs) = c_get_attribute_value(session, handle, {CKA_ID: None,
            CKA_VALUE: None})
    check_rv(rv,"c_get_attribute_value()")
		
    fname = attrs[CKA_ID].decode('utf-8') + '.key'
    if fname not in key_files:
        with open(path.join(keys_dir, fname), 'wb') as f:
            print("  Get key '" + fname + "' from UKC")
            f.write(binascii.unhexlify(attrs[CKA_VALUE]))
    else:
        key_files.remove(fname)

# 2. Upload new keys
for key_file in key_files:
    with open(path.join(keys_dir, key_file), 'r') as f:
        data = f.read()
        if 'role: root' in data:
            continue  # skip root keys
        kname = key_file[:-4].encode('ascii')
        print("  Import key '" + kname + "' to UKC")
        (rv, _) = c_create_object(session, {
            CKA_CLASS: CKO_DATA,
            CKA_TOKEN: True,
            CKA_ID: kname,
            CKA_VALUE: binascii.hexlify(data.encode('ascii')),
            })
        check_rv(rv,"c_create_object()")

print("Keys synchronization done successfully")

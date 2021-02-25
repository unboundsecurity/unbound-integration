# @license
# Copyright 2019 Google LLC.
#
# This software is provided as-is, without warranty or
# representation for any use or purpose. Your use of it is
# subject to your agreements with Google.

"""Functionality for retrieving OIDC auth-tokens."""

import os
import sys
import googleapiclient.discovery
#from typing import Text  # pylint: disable=unused-import
from google.oauth2 import service_account
#import ekms_key_uri as ekms_key_uri_util


def ObtainToken(ekms_key_uri):
  # type: (Text) -> Text
  """Obtains an OpenID Connect Token for the given External-KMS key URI.

  This relies on the service account credentials file being present in the
  GOOGLE_APPLICATION_CREDENTIALS env var. If this is run on GCE, this should
  just work, as long as the service account used has the correct permissions, as
  outlined in
  https://cloud.google.com/iam/credentials/reference/rest/v1/
  projects.serviceAccounts/generateIdToken.

  Args:
    ekms_key_uri: A string uniquely identifying the EKMS key to obtain a token
      for.
      For example "https://ekms.com/my_key"

  Returns:
    str: The OIDC token.
  """
  credentials = service_account.Credentials.from_service_account_file(
      filename=os.environ['GOOGLE_APPLICATION_CREDENTIALS'])
  service = googleapiclient.discovery.build(
      'iamcredentials', 'v1', credentials=credentials)
  body = {
      'includeEmail': True,
      'audience': ekms_key_uri, # ekms_key_uri_util.EkmsId(ekms_key_uri),
      'delegates': None,
  }
  token = service.projects().serviceAccounts().generateIdToken(
      name='projects/-/serviceAccounts/{0}'.format(
          credentials.service_account_email),
      body=body).execute()['token']
  print(token)
  return token

ObtainToken(sys.argv[1])



# Volume encryption using the encfs user space file system

### Advantages of encfs

By default, all FUSE-based filesystems are visible only to the user who mounted them. This way, we can automatically prevent the **root** user from viewing and changing unencrypted files.

### How it works
Most of the interesting code is found in the ``userdata.sh`` file used for EC2 VM setup. It works as following:

1. Create and mount '/data' partition pointing to external volume - provided by AWS.
2. Install needed packages (wget, fuse, jq, openssl, tinyxml2, fuse-encfs).
3. Download and install ekm-client rpm package.
4. Register ephemeral client.
5. Setup Unboundsecurity OpenSSL support
6. Decrypt the ``key phase`` using the OpenSSL Unboundsecurity engine and use it for the ``encfs``.

## Prerequisites

1. Create a partition and sign-in to it
1. Create an RSA key in Unbound UI
1. Create an Ephemeral Client
1. Get activation code and use it in the VM setup
1. Export public key using ucl command: ``ucl export -n key-name --output cert.pub``
1. Generate an encrypted key phase:
   1. Generate a random key (key phase): ``head -c 128 /dev/random``
   2. Encrypt the key phrase: ``openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep``
   3. Convert to base 64 printable format: ``base64``
   4. In one command:
   ```
   head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64
   ```
1. Update the `variables.tf` file.
1. Run the ``terraform init`` command.

## Setup

Run the ``terraform apply`` command.

## Troubleshooting
1. Destroy everything with this command: ``terraform destroy``.
1. View VM installation script (`userdata.sh`) progress (after ssh to VM): ``tail -f /var/log/cloud-init-output.log``

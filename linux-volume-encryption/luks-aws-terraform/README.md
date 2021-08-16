# Volume encryption using the LUKS Linux kernel level support

### How it works

Most of the interesting code is found in the ``userdata.sh`` file used for EC2 VM setup. It works as following:

1. Dump encrypted key used for disk encryption using the Unbound API.
1. Save the file to ``/root/keyfile``.
1. Format additional disk available (provided by AWS) using the ``luksFormat``.
1. Create an ext4 file system on a new encrypted volume.
1. Update the ``/etc/crypttab`` and ``/etc/fstab`` with the encrypted volume details.
1. Mount encrypted partition.

## Setup

Run the ``terraform apply`` command.

## Troubleshooting
1. Destroy everything with this command: ``terraform destroy``.
1. View VM installation script (`userdata.sh`) progress (after ssh to VM): ``tail -f /var/log/cloud-init-output.log``

# debsigs

This script is used to sign or verify a signature on a file.

## Usage

```
debsigs --sign=type [--default-key=keyID] [-v] file [file...]
    options:
    -u [--key-uid]     UID of private key
    -p [--partition]   Partition name (optional)
    --user	   UKC username to perform sign operation (optional)
    -w [--password]	   Password of UKC username to perform sign operation (optional)
    --verbose          Verbose output
	
debsigs --version
```

The signature type will usually be one of 'origin', 'maint', or 'archive';
see the manual page for details.

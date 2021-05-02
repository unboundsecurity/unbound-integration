# Sample CI/CD pipeline implementing Unbound Security code signing for jar & rpm files

This is a sample Java Hello World project. CI/CD automation can be found in the `.gitlab-ci.yml` file. It does the following:

1. Download missing packages: `java-devel`, `wget`, etc...
1. Build `hello.jar` file.
1. Download Unbound Security client application `ekm-client`.
1. Register an ephemeral client in the UKC client.
1. Sign on `hello.jar` file.
1. Generate RPM file: `hello-1.0.0-1.noarch.rpm`.
1. Generate signed RPM file: `hello-1.0.0-1.noarch.signed.rpm`.
1. Export RPM public key: `GPGkey-pub.pgp`.
1. Save `GPGkey-pub.pgp` and `hello-1.0.0-1.noarch.signed.rpm` files as build **artifacts**.

## Build parameters

The build parameters found in the `.gitlab-ci.yml` file should be stored in this GitLab Project -> Settings -> CI/CD -> Variables.

The `$ACTIVATION_CODE` variable is a UKC client activation code.

The `$EKM_CLIENT_RPM` variable is the url used to download ekm client rpm file.

The `$KEY_NAME` is a key name you created in the EKM service.

The `install_props.txt` file contains the server address of the Unbound Security Entry Point Server.

## Why Unbound Security?

With [Unbound Security](https://www.unboundsecurity.com/) code signing is a natural, zero hassle part of software development process.

1. Top-level security. Key material is split between multiple servers without bringing the key in one place.
1. FIPS protected pure-software virtual HSM.
1. State of the art multi-party computation algorithms used to sign on the files. 
1. Solution that applies to all development teams globally.
1. Easy to deploy and use. For example with this GitLab CI/CD pipeline.
1. Central management for keys and certificates.
1. Detailed audit log to track all code-signing activities.

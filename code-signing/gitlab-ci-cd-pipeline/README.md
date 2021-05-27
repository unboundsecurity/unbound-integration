# GitLab CI/CD pipeline for code signing JAR and RPM files

This is a sample Java project showing GitLab CI/CD pipeline for code signing JAR and RPM files. See [here](https://www.unboundsecurity.com/solutions/information-security/) for more information about using Unbound for code signing.

CI/CD automation can be found in the `.gitlab-ci.yml` file. 

The sample does the following:

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

- `$UNBOUND_EP_SERVER` is an Unbound Entry Point server running in your environment. It can be a hostname or IP address.
- `$ACTIVATION_CODE` is a UKC client activation code.
- `$EKM_CLIENT_RPM` is the URL used to download ekm client RPM file.
- `$KEY_NAME` is a signing key name you created in the EKM service.

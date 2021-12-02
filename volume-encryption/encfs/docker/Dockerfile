FROM ubuntu:18.04

RUN apt update && apt upgrade && \
  apt install -y curl iputils-ping encfs wget

RUN wget --no-check-certificate https://repo.dyadicsec.local/cust/autotest/ekm/2.0.2106.42245/linux/ekm-client_2.0.2106.42245.deb10_amd64.deb \
    && dpkg -i ekm-client_2.0.2106.42245.deb10_amd64.deb \
    && yes | /etc/ekm/dy_openssl \
    && rm ekm-client_2.0.2106.42245.deb10_amd64.deb


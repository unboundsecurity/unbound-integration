FROM ubuntu:18.04

RUN apt update && apt upgrade -y && \
  apt install -y curl iputils-ping encfs wget

# Uncomment these next lines to install and use Unbound CORE Client
# ARG UNBOUND_CLIENT_INSTALLATION_URL=https://repo.dyadicsec.local/cust/autotest/ekm/2.0.2106.42245/linux
# ARG UNBOUND_CLIENT_INSTALLATION_FILE=ekm-client_2.0.2106.42245.deb10_amd64.deb
# RUN wget --no-check-certificate $UNBOUND_CLIENT_INSTALLATION_URL/$UNBOUND_CLIENT_INSTALLATION_FILE \
#     && dpkg -i $UNBOUND_CLIENT_INSTALLATION_FILE \
#     && rm $UNBOUND_CLIENT_INSTALLATION_FILE

ADD bash-scripts /unbound-scripts/
RUN chmod +x /unbound-scripts/*
ENV PATH="/unbound-scripts:${PATH}"
ENTRYPOINT [ "ub-start" ]

# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.155.1/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="3.1"
FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0-${VARIANT}

######################################
RUN echo "Downloading UKC Utils";

# Common utilities and clean
RUN echo 'alias ll="ls -l"' >> ~/.bashrc; \
    apt-get update -y \
    apt-get install curl \
    apt-get install -y policycoreutils-python-utils \
    apt-get install libssl-dev; \
    apt-get clean -y;

# JQ - Json parser
RUN curl -LO# https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64; \
    mv ./jq-linux64 ./jq; \
    chmod +x ./jq; \
    mv jq /usr/bin    

RUN mkdir /root/data
COPY data /root/data

ARG UKC_CLIENT_INSTALLER_URL
RUN echo "Downloading UKC client install file: $UKC_CLIENT_INSTALLER_URL";

RUN  sudo apt install /root/data/ekm-client_2.0.2010.38476.deb9_amd64.deb; \
    rm $(basename /root/data/ekm-client_2.0.2010.38476.deb9_amd64.deb); \
    echo "UKC Client Installed successfully"

RUN chmod +x /root/data/*.sh;

#RUN mkdir /root/demo
#COPY ./src /root/demo/



######################################   

# [Option] Install Node.js
ARG INSTALL_NODE="true"
ARG NODE_VERSION="lts/*"
RUN if [ "${INSTALL_NODE}" = "true" ]; then su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; fi

# [Option] Install Azure CLI
ARG INSTALL_AZURE_CLI="false"
COPY library-scripts/azcli-debian.sh /tmp/library-scripts/
RUN if [ "$INSTALL_AZURE_CLI" = "true" ]; then bash /tmp/library-scripts/azcli-debian.sh; fi \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

# [Optional] Uncomment this section to install additional OS packages.
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends curl


# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

#install ssh
RUN apt-get update \
&& apt-get install -y --no-install-recommends openssh-server \
&& echo "root:Docker!" | chpasswd

COPY sshd_config /etc/ssh/


#COPY netcoreapp3.1 /root/data
RUN mkdir /root/data/published
COPY publish /root/data/published
RUN mkdir /home/published
COPY publish /home/published
#RUN mkdir /root/data/customer-key-store
#COPY customer-key-store /root/data/customer-key-store

#EXPOSE 5000
#EXPOSE 5001
#EXPOSE 8000
EXPOSE 2222
#EXPOSE 8443


ENV PORT 8080
#EXPOSE 8080
#EXPOSE 80 2222
EXPOSE 2222 8080

ENV ASPNETCORE_URLS "http://*:${PORT}"

#ENTRYPOINT ["dotnet", "/root/data/published/customerkeystore.dll"]
#ENTRYPOINT ["/bin/bash", "/root/data/start_test.sh"]
CMD "/root/data/start.sh"

#ENTRYPOINT ["tail", "-f", "/dev/null"]
#ENTRYPOINT ["/root/data/start.sh"]



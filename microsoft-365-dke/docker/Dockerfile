# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.155.1/containers/dotnet/.devcontainer/base.Dockerfile

# [Choice] .NET version: 5.0, 3.1, 2.1
ARG VARIANT="3.1"
FROM mcr.microsoft.com/vscode/devcontainers/dotnetcore:0-${VARIANT}

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

# Install UKC Client
ARG UKC_CLIENT_INSTALLER_URL
RUN echo "Downloading ${UKC_CLIENT_INSTALLER_URL}"; \
    curl -O#  "${UKC_CLIENT_INSTALLER_URL}"; \
    echo "Installing $(basename ${UKC_CLIENT_INSTALLER_URL})"; \
    sudo apt install $(basename "${UKC_CLIENT_INSTALLER_URL}"); \
    rm $(basename "${UKC_CLIENT_INSTALLER_URL}"); \
    echo "UKC Client Installed successfully"


RUN mkdir /unbound
COPY data /unbound
RUN chmod +x /unbound/*;
ENV PATH="/unbound:${PATH}"

######################################   

RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

#install ssh
# RUN apt-get update \
# && apt-get install -y --no-install-recommends openssh-server \
# && echo "root:Docker!" | chpasswd

# COPY sshd_config /etc/ssh/

ENV PORT 8080

EXPOSE 8080

ENV ASPNETCORE_URLS "http://*:${PORT}"

CMD "start.sh"



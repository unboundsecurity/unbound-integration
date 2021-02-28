#!/bin/bash

dir=$(dirname "$0")
cd "$dir/.."

os=$(go env GOOS 2> /dev/null || uname -s |tr '[:upper:]' '[:lower:]')
arch=$(go env GOARCH 2> /dev/null || uname -m | sed s/x86_64/amd64/)
rel=1.4.1

if [ ! -x bin/cryptogen ] || [ ! -x bin/configtxgen ]; then
  mkdir -p bin
  url="https://nexus.hyperledger.org/content/repositories/releases/org/hyperledger/fabric/hyperledger-fabric/$os-$arch-$rel/hyperledger-fabric-$os-$arch-$rel.tar.gz"
  curl "$url" | tar xzf - bin/cryptogen bin/configtxgen
fi

if [ ! -r ./core.yaml ]; then
  docker run -it --rm hyperledger/fabric-peer:1.4.1 cat /etc/hyperledger/fabric/core.yaml > ./core.yaml
fi

#!/bin/bash
set -x

echo "Waiting for EP to start"
until $(curl --output /dev/null -k --silent --head --fail \
  ${UB_CORE_URL}/api/v1/health); do
    printf '*'
    sleep 5
done

echo "UKC Ready"
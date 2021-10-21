#!/bin/bash

echo "=================="
echo "Building Client..."

docker build \
    -f ./opendesk-client/Client.Dockerfile \
    -t opendesk-client:demo \
    --build-arg API_URL=https://opendesk.iameric.net \
    --build-arg ENABLE_DEMO_MODE=true \
    ./opendesk-client

echo "Building API..."

docker build \
    -f ./opendesk-api/src/OpenDesk.API/API.Dockerfile \
    -t opendesk-api \
    --build-arg CONFIGURATION=Release \
    ./opendesk-api

echo "Built Client and API OK."
echo "=================="

exit 0
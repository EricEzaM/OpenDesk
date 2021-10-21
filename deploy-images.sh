#!/bin/bash

docker context use default

echo Should images be rebuilt?
read rebuild

if [ $rebuild -eq 1 ]
then
    ./build-images.sh || { echo "Image build failed"; exit 1; }
fi

echo What is the server IP address I should send the images to?
read ip

docker save opendesk-client:demo | bzip2 | ssh udocker@$ip docker load
docker save opendesk-api | bzip2 | ssh udocker@$ip docker load
docker context use my-apps
docker-compose -f docker-compose.production.yml up 

echo All done!
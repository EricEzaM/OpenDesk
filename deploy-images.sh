#!/bin/bash

docker context use default

echo >> Should images be rebuilt? [input 1 or 0 for 'yes' and 'no' respectively]
read rebuild

if [ $rebuild -eq 1 ]
then
    ./build-images.sh || { echo "Image build failed"; exit 1; }
fi

echo >> What is the server IP address I should send the images to?
read ip

echo >> Saving client:demo, and loading on remote server.
docker save opendesk-client:demo | bzip2 | ssh udocker@$ip docker load
echo >> Saving API, and loading on remote server.
docker save opendesk-api | bzip2 | ssh udocker@$ip docker load

echo >> Switching docker context
docker context use my-apps
echo >> Using docker-compose up on production compose file.
docker-compose -f docker-compose.production.yml up 

echo All done!
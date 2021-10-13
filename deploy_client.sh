docker context use default
docker-compose build client
docker save opendesk-client:demo | bzip2 | ssh udocker@172.105.183.8 docker load
docker context use my-apps
docker-compose -f docker-compose2.yml up 
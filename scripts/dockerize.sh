#!/bin/bash
TAG=''
VERSION_TAG=''

case "$TRAVIS_BRANCH" in
  "master")
    TAG=latest
    VERSION_TAG=3.1.0
    ;;
  "develop")
    TAG=dev
    VERSION_TAG=$TAG-3.1.0
    ;;
esac

REPOSITORY=$DOCKER_CONTAINER/qrcode

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker build -t $REPOSITORY:$TAG -t $REPOSITORY:$VERSION_TAG -f ./src/Genocs.QRCodeLibrary.WebApi/Dockerfile .
docker push $REPOSITORY:$TAG
docker push $REPOSITORY:$VERSION_TAG

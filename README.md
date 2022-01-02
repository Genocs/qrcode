![QRCode Library](https://raw.githubusercontent.com/genocs/clean-architecture-template/master/images/genocs-icon.png) QRCode scanner and builder
=========

This is simple library can be used to scan images containing QR code.

The library allows to build a QR code image as well.



[![.NET](https://github.com/Genocs/qrcode/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Genocs/qrcode/actions/workflows/dotnet.yml) [![Build Status](https://app.travis-ci.com/Genocs/qrcode.svg?branch=master)](https://app.travis-ci.com/github/Genocs/qrcode) <a href="https://www.nuget.org/packages/Genocs.QRCodeLibrary/" rel="Genocs.QRCodeLibrary">![NuGet](https://buildstats.info/nuget/genocs.qrcodelibrary)</a> <a href="https://hub.docker.com/repository/docker/genocs/qrcode.api/" rel="Genocs.QRCodeLibrary">![Docker Automated build](https://img.shields.io/docker/automated/genocs/qrcode.api)</a> [![Gitter](https://img.shields.io/badge/chat-on%20gitter-blue.svg)](https://gitter.im/genocs/)



## References

Please see the original version at [codeproject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/).

## Commands

###  Build the project

To build and test the project type following command:

```ps
dotnet build
dotnet test
```

Steps to build the Docker image and run the container

```ps
# Build the Docker image
docker build -t genocs.qrcode.api .

# Add a tag
docker tag genocs.qrcode.api genocs/qrcode.api

# Push to the container registry
docker push genocs/qrcode.api

# Run the container 
docker run -p 90:80 -d --name qrcodeapi-container genocs/qrcode.api
```

If you want to use the container into a docker network:

``` ps
docker run -p 90:80 -d --name qrcodeapi-container genocs/qrcode.api --network genocs-network
```

###  Push the images to the Docker image repository (Docker Hub)

The tagname is optional

``` ps
docker push genocs/qrcode.api:tagname
```

### Pull the image from Docker image repository (Docker Hub)

``` ps
docker pull genocs/qrcode.api:tagname
```

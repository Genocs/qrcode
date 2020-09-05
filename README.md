![QRCode Library](https://raw.githubusercontent.com/genocs/clean-architecture-template/master/images/genocs-icon.png) Fast qrcode scanner and builder library ported on .NET core 3.1
=========
This is simple but useful library that can be used to Scan images containing QR code. The library allows to build a QR code image as well.

----

[![Build Status](https://travis-ci.org/Genocs/genocs-library.svg?branch=master)](https://travis-ci.org/Genocs/genocs-library) [![NuGet](https://img.shields.io/badge/nuget-v1.0.1-blue)](https://www.nuget.org/packages/Genocs.Core) [![Gitter](https://img.shields.io/badge/chat-on%20gitter-blue.svg)](https://gitter.im/genocs/)


## References 
(see original version at [codeproject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/)).

## Commands

### Build the project
To build the project type following command:

```ps
dotnet build
```


If you want to use Docker

1. Build the Docker image
2. Create the image tag
3. Run The container


```ps
docker build -t genocs.qrcode.api .
docker tag genocs.qrcode.api genocs/qrcode.api
docker run -p 90:80 -d --name qrcodeapi-container genocs/qrcode.api
```

If you want to use the container into a docker network:
``` ps
docker run -d --name qrcodeapi-container -p 5001:80 genocs/qrcode.api --network genocs-network

```


### Push the images to the Docker image repository (Docker Hub)

The tagname is optinal 

``` ps
docker push genocs/qrcode.api:tagname
```

### Pull the image form Docker image repository (Docker Hub)

``` ps
docker pull genocs/qrcode.api:tagname
```

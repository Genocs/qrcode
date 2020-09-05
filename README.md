# qrcode [![Build Status](https://travis-ci.org/Genocs/genocs-library.svg?branch=master)](https://travis-ci.org/Genocs/genocs-library) [![NuGet](https://img.shields.io/badge/nuget-v1.0.1-blue)](https://www.nuget.org/packages/Genocs.Core) 

Fast qrcode scanner library ported on .NET core 3.1

---

## References 
(see original version at [codeproject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/)).
```
/////////////////////////////////////////////////////////////////////
//
//	QR Code Library
//
//	QR Code finder class.
//
//	Author: Uzi Granot
//	Original Version: 1.0
//	Date: June 30, 2018
//	Copyright (C) 2018-2019 Uzi Granot. All Rights Reserved
//	For full version history please look at QRDecoder.cs
//
//	QR Code Library C# class library and the attached test/demo
//  applications are free software.
//	Software developed by this author is licensed under CPOL 1.02.
//	Some portions of the QRCodeVideoDecoder are licensed under GNU Lesser
//	General Public License v3.0.
//
//	The solution is made of 3 projects:
//	1. QRCodeDecoderLibrary: QR code decoding.
//	3. QRCodeDecoderDemo: Decode QR code image files.
//	4. QRCodeVideoDecoder: Decode QR code using web camera.
//		This demo program is using some of the source modules of
//		Camera_Net project published at CodeProject.com:
//		https://www.codeproject.com/Articles/671407/Camera_Net-Library
//		and at GitHub: https://github.com/free5lot/Camera_Net.
//		This project is based on DirectShowLib.
//		http://sourceforge.net/projects/directshownet/
//		This project includes a modified subset of the source modules.
//
//	The main points of CPOL 1.02 subject to the terms of the License are:
//
//	Source Code and Executable Files can be used in commercial applications;
//	Source Code and Executable Files can be redistributed; and
//	Source Code can be modified to create derivative works.
//	No claim of suitability, guarantee, or any warranty whatsoever is
//	provided. The software is provided "as-is".
//	The Article accompanying the Work may not be distributed or republished
//	without the Author's consent
//
//	For version history please refer to QRDecoder.cs
/////////////////////////////////////////////////////////////////////
```

## Commands

### Build the project
To build the project type following command:
```ps
dotnet build
```


### Build the Docker image

``` ps
docker build -t genocs.qrcode .
```

### Create the image tag

``` ps
docker tag genocs.qrcode genocs/genocs.qrcode
```

### Run the container
Before start check if the network exist otherwise remove it or create the network

``` ps
docker run -d --name qrcode_1 -p 5001:80 genocs.qrcode --network genocs-network
docker run -d --name qrcode_2 -p 5002:80 genocs.qrcode --network genocs-network

```



### Push the images to the Docker image repository (Docker Hub)

``` ps
docker push genocs/genocs.qrcode:tagname
```



If you want to learn more about creating good readme files then refer the following [guidelines](https://www.visualstudio.com/en-us/docs/git/create-a-readme). You can also seek inspiration from the below readme files:
- [ASP.NET Core](https://github.com/aspnet/Home)
- [Visual Studio Code](https://github.com/Microsoft/vscode)
- [Chakra Core](https://github.com/Microsoft/ChakraCore)

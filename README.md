<!-- PROJECT SHIELDS -->
[![License][license-shield]][license-url]
[![Build][build-shield]][build-url]
[![DownloadsBarcode][downloads-br-shield]][downloads-br-url]
[![DownloadsQRCode][downloads-qr-shield]][downloads-qr-url]
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Discord][discord-shield]][discord-url]
[![Gitter][gitter-shield]][gitter-url]
[![Twitter][twitter-shield]][twitter-url]
[![Twitterx][twitterx-shield]][twitterx-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

[license-shield]: https://img.shields.io/github/license/Genocs/qrcode?color=2da44e&style=flat-square
[license-url]: https://github.com/Genocs/qrcode/blob/main/LICENSE
[build-shield]: https://github.com/Genocs/qrcode/actions/workflows/build_and_test.yml/badge.svg?branch=main
[build-url]: https://github.com/Genocs/qrcode/actions/workflows/build_and_test.yml
[downloads-br-shield]: https://img.shields.io/nuget/dt/Genocs.BarcodeLibrary.svg?color=2da44e&label=downloads_barcode&logo=nuget
[downloads-br-url]: https://www.nuget.org/packages/Genocs.BarcodeLibrary
[downloads-qr-shield]: https://img.shields.io/nuget/dt/Genocs.QRCodeLibrary.svg?color=2da44e&label=downloads_qrcode&logo=nuget
[downloads-qr-url]: https://www.nuget.org/packages/Genocs.QRCodeLibrary
[contributors-shield]: https://img.shields.io/github/contributors/Genocs/qrcode.svg?style=flat-square
[contributors-url]: https://github.com/Genocs/qrcode/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Genocs/qrcode?style=flat-square
[forks-url]: https://github.com/Genocs/qrcode/network/members
[stars-shield]: https://img.shields.io/github/stars/Genocs/qrcode.svg?style=flat-square
[stars-url]: https://img.shields.io/github/stars/Genocs/qrcode?style=flat-square
[issues-shield]: https://img.shields.io/github/issues/Genocs/qrcode?style=flat-square
[issues-url]: https://github.com/Genocs/qrcode/issues
[discord-shield]: https://img.shields.io/discord/1106846706512953385?color=%237289da&label=Discord&logo=discord&logoColor=%237289da&style=flat-square
[discord-url]: https://discord.com/invite/fWwArnkV
[gitter-shield]: https://img.shields.io/badge/chat-on%20gitter-blue.svg
[gitter-url]: https://gitter.im/genocs/
[twitter-shield]: https://img.shields.io/twitter/follow/genocs?color=1DA1F2&label=Twitter&logo=Twitter&style=flat-square
[twitter-url]: https://twitter.com/genocs
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/giovanni-emanuele-nocco-b31a5169/
[twitterx-shield]: https://img.shields.io/twitter/url/https/twitter.com/genocs.svg?style=social
[twitterx-url]: https://twitter.com/genocs


<p align="center">
    <img src="./assets/genocs-library-logo.png" alt="icon">
</p>


Barcode builder and QRCode scanner and builder
=========

This library can be used to build and scan images containing QR code.

The library allows to build a different type of Barcode.

The library do not contains reference to System.Drawing.Common library, so it can be used into Docker Image Linux native


![Docker Automated build](https://img.shields.io/docker/automated/genocs/qrcode)</a> 


## Commands

###  Build the project

To build and test the project type following command:

``` bash
dotnet build
dotnet test
```

Steps to build the Docker image and run the container

``` bash
# Build the Docker image
docker build -f webapi.dockerfile -t genocs/codes-webapi:1.0.1 -t genocs/codes-webapi:latest .

# Add a tag
docker tag genocs/codes-webapi:1.0.1 genocs/codes-webapi:latest

# Push to the container registry
docker push genocs/codes-webapi:1.0.1
docker push genocs/codes-webapi:latest

# Run the container 
docker run -p 5900:8080 -d --name qrcodeapi-container genocs/codes-webapi:1.0.1
```

If you want to use the container into a docker network:

``` bash
docker run -p 5900:8080 -d --name qrcodeapi-container genocs/codes-webapi:1.0.1 --network genocs-network
```

###  Push the images to the Docker image repository (Docker Hub)

*tagname* is optional

``` bash
docker push genocs/codes-webapi:tagname
```

### Pull the image from Docker image repository (Docker Hub)

``` bash
docker pull genocs/codes-webapi:tagname
```

### Deploy in a cloud instance

You can deploy Demo Application with one click in Heroku, Microsoft Azure, or Google Cloud Platform: 

[<img src="https://www.herokucdn.com/deploy/button.svg" height="30px">](https://heroku.com/deploy?template=https://github.com/heartexlabs/label-studio/tree/heroku-persistent-pg)
[<img src="https://aka.ms/deploytoazurebutton" height="30px">](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fheartexlabs%2Flabel-studio%2Fmaster%2Fazuredeploy.json)
[<img src="https://deploy.cloud.run/button.svg" height="30px">](https://deploy.cloud.run)

## License

This project is licensed with the [MIT license](LICENSE).

## Changelogs

View Complete [Changelogs](https://github.com/Genocs/qrcode/blob/main/CHANGELOGS.md).

## Community

- Discord [@genocs](https://discord.com/invite/fWwArnkV)
- Facebook Page [@genocs](https://facebook.com/Genocs)
- Youtube Channel [@genocs](https://youtube.com/c/genocs)


## Support

Has this Project helped you learn something New? or Helped you at work?
Here are a few ways by which you can support.

- ⭐ Leave a star! 
- 🥇 Recommend this project to your colleagues.
- 🦸 Do consider endorsing me on LinkedIn for ASP.NET Core - [Connect via LinkedIn](https://www.linkedin.com/in/giovanni-emanuele-nocco-b31a5169/) 
- ☕ If you want to support this project in the long run, [consider buying me a coffee](https://www.buymeacoffee.com/genocs)!
  

[![buy-me-a-coffee](https://raw.githubusercontent.com/Genocs/qrcode/main/assets/buy-me-a-coffee.png "buy-me-a-coffee")](https://www.buymeacoffee.com/genocs)

## Code Contributors

This project exists thanks to all the people who contribute. [Submit your PR and join the team!](CONTRIBUTING.md)

[![genocs contributors](https://contrib.rocks/image?repo=Genocs/qrcode "genocs contributors")](https://github.com/genocs/qrcode/graphs/contributors)

## Financial Contributors

Become a financial contributor and help me sustain the project. [Support the Project!](https://opencollective.com/genocs/contribute)

<a href="https://opencollective.com/genocs"><img src="https://opencollective.com/genocs/individuals.svg?width=890"></a>


## Acknowledgements

Please see the original version at [codeproject](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-NET-Framework-Standard/).


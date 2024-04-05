#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ["src/Genocs.QRCodeLibrary.WebApi", "Genocs.QRCodeLibrary.WebApi/"]
COPY ["src/Genocs.QRCodeLibrary", "Genocs.QRCodeLibrary/"]
COPY ["src/Genocs.BarcodeLibrary", "Genocs.BarcodeLibrary/"]

COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Build.targets", "Directory.Build.targets"]
COPY ["dotnet.ruleset", "dotnet.ruleset"]
COPY ["global.json", "global.json"]
COPY ["stylecop.json", "stylecop.json"]
COPY ["LICENSE", "LICENSE"]
COPY ["icon.png", "icon.png"]

WORKDIR "/src/Genocs.QRCodeLibrary.WebApi"

RUN dotnet restore "Genocs.QRCodeLibrary.WebApi.csproj"

RUN dotnet build "Genocs.QRCodeLibrary.WebApi.csproj" -c Debug -o /app/build

FROM build-env AS publish
RUN dotnet publish "Genocs.QRCodeLibrary.WebApi.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.QRCodeLibrary.WebApi.dll"]
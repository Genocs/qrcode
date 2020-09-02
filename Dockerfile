FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
 
WORKDIR /app
COPY . .
RUN dotnet publish src/Genocs.QRCodeLibrary.WebApi -c release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

RUN apt-get update \
&& apt-get install -y --allow-unauthenticated \
    libc6-dev \
    libgdiplus \
    libx11-dev \
 && rm -rf /var/lib/apt/lists/*
 
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet Genocs.QRCodeLibrary.WebApi.dll
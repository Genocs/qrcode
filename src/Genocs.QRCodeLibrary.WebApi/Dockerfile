#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Genocs.QrCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj", "Genocs.QrCodeLibrary.WebApi/"]
COPY ["Genocs.QRCodeLibrary/Genocs.QRCodeLibrary.csproj", "Genocs.QRCodeLibrary/"]
RUN dotnet restore "Genocs.QrCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj"
COPY . .
WORKDIR "/src/Genocs.QrCodeLibrary.WebApi"
RUN dotnet build "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.QRCodeLibrary.WebApi.dll"]
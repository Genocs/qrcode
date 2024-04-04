#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj", "Genocs.QRCodeLibrary.WebApi/"]
COPY ["src/Genocs.QRCodeLibrary/Genocs.QRCodeLibrary.csproj", "Genocs.QRCodeLibrary/"]
COPY ["src/Genocs.BarcodeLibrary/Genocs.BarcodeLibrary.csproj", "Genocs.BarcodeLibrary/"]


RUN dotnet restore "src/Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj"
COPY . .
WORKDIR "/src/Genocs.QRCodeLibrary.WebApi"
RUN dotnet build "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.QRCodeLibrary.WebApi.dll"]
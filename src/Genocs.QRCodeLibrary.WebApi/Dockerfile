#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj", "src/Genocs.FormRecognizer.WebApi/"]
COPY ["src/Genocs.QRCodeLibrary/Genocs.QRCodeLibrary.csproj", "src/Genocs.QRCodeLibrary/"]
COPY ["src/Genocs.BarcodeLibrary/Genocs.BarcodeLibrary.csproj", "src/Genocs.BarcodeLibrary/"]


RUN dotnet restore "src/Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Genocs.QRCodeLibrary.WebApi"
RUN dotnet build "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/publish

# Install gdiplus library on Linx container
# See following link:  https://stackoverflow.com/questions/52069939/dockerized-dotnet-core-2-1-throws-gdip-exception-when-using-select-htmltopdf-net
#RUN apt-get update \
#    && apt-get install -y --allow-unauthenticated \
#    libc6-dev \
#    libgdiplus \
#    libx11-dev \
#    && rm -rf /var/lib/apt/lists/*

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.QRCodeLibrary.WebApi.dll"]
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copy everything else and build
COPY . .
RUN dotnet publish src/Genocs.QRCodeLibrary.WebApi -c release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
# Install gdiplus library on Linx container
# See following link:  https://stackoverflow.com/questions/52069939/dockerized-dotnet-core-2-1-throws-gdip-exception-when-using-select-htmltopdf-net
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
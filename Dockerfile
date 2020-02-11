FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY ./src/Genocs.QRCode.WebApi/bin/docker .
ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT docker
EXPOSE 5000
ENTRYPOINT dotnet Genocs.QRCode.WebApi.dll
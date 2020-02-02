FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR ./
COPY ["Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj", "Genocs.QRCodeLibrary.WebApi/"]
COPY ["Genocs.QRCodeLibrary/Genocs.QRCodeLibrary.csproj", "Genocs.QRCodeLibrary/"]
RUN dotnet restore "Genocs.QRCodeLibrary.WebApi/Genocs.QRCodeLibrary.WebApi.csproj"
COPY . .
WORKDIR "/Genocs.QRCodeLibrary.WebApi"
RUN dotnet build "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Genocs.QRCodeLibrary.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Genocs.QRCodeLibrary.WebApi.dll"]
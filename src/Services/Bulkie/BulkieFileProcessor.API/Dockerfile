#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Services/Bulkie/BulkieFileProcessor.API/BulkieFileProcessor.API.csproj", "src/Services/Bulkie/BulkieFileProcessor.API/"]
RUN dotnet restore "src/Services/Bulkie/BulkieFileProcessor.API/BulkieFileProcessor.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Bulkie/BulkieFileProcessor.API"
RUN dotnet build "BulkieFileProcessor.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BulkieFileProcessor.API.csproj" -c Release -o /app/publish
ADD ["src/Services/Bulkie/BulkieFileProcessor.API/Setup", "/app/publish/Setup"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BulkieFileProcessor.API.dll"]
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY InglesApp.Domain/InglesApp.Domain.csproj InglesApp.Domain/
COPY InglesApp.Application/InglesApp.Application.csproj InglesApp.Application/
COPY InglesApp.Infrastructure/InglesApp.Infrastructure.csproj InglesApp.Infrastructure/
COPY InglesApp.Api/InglesApp.Api.csproj InglesApp.Api/

RUN dotnet restore InglesApp.Api/InglesApp.Api.csproj

COPY . .
RUN dotnet publish InglesApp.Api/InglesApp.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InglesApp.Api.dll"]

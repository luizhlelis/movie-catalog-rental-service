FROM mcr.microsoft.com/dotnet/sdk:6.0 AS PUBLISH_STAGE
WORKDIR /build
ADD RentMovie.Web ./RentMovie.Web
ADD RentMovie.Infrastructure ./RentMovie.Infrastructure
ADD RentMovie.Application ./RentMovie.Application
RUN dotnet publish RentMovie.Web/RentMovie.Web.csproj -r linux-x64 -o bin -c Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS RUNTIME_STAGE
WORKDIR /app
COPY --from=PUBLISH_STAGE /build/bin ./
#docker build -t sharesafely:latest . 
#docker run -p 8090:8090 sharesafely

FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
WORKDIR /App/
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App/
COPY --from=build-env /App/out .
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["./Sharesafely"]

RUN apt-get -y update
RUN apt-get -y install curl

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

LABEL maintainer="3temal@gmail.com" 

WORKDIR /src/Identity.API/
COPY ["./Identity.API.csproj", "./"]
RUN dotnet restore "./Identity.API.csproj"
COPY . .

RUN pwd
RUN ls
RUN dotnet build "./Identity.API.csproj" -c Release -o /app/build
RUN dotnet ef database update

FROM build AS publish
RUN dotnet publish "./Identity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
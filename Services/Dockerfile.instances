#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Instances/Instances.API/Instances.API.csproj", "Services/Instances/Instances.API/"]
COPY ["Services/Common/CommonLib/CommonLib.csproj", "Services/Common/CommonLib/"]
RUN dotnet restore "Services/Instances/Instances.API/Instances.API.csproj"
COPY . .
WORKDIR "/src/Services/Instances/Instances.API"
RUN dotnet build "Instances.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instances.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Instances.API.dll"]
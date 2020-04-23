FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src
COPY ["ApiGateways/Standart/StandartGateway/StandartGateway.csproj", "ApiGateways/Standart/StandartGateway/StandartGateway.csproj"]
COPY ["Services/Common/CommonLib/CommonLib.csproj", "Services/Common/CommonLib/"]
RUN dotnet restore "./ApiGateways/Standart/StandartGateway/StandartGateway.csproj"

COPY [ "ApiGateways/Standart/StandartGateway/", "ApiGateways/Standart/StandartGateway/" ]
COPY [ "Services/Common/CommonLib/", "Services/Common/CommonLib/" ]

COPY [ "Services/Disks/Disks.gRPC.Service/Protos/volumes.proto", "Services/Disks/Disks.gRPC.Service/Protos/volumes.proto" ]
COPY [ "Services/Instances/Instances.API/Protos/instances.proto", "Services/Instances/Instances.API/Protos/instances.proto" ]

WORKDIR "/src/ApiGateways/Standart/StandartGateway/"
RUN dotnet build "StandartGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StandartGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StandartGateway.dll"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Disks/Disks.gRPC.Service/Disks.gRPC.Service.csproj", "Services/Disks/Disks.gRPC.Service/"]
COPY ["Services/Common/CommonLib/CommonLib.csproj", "Services/Common/CommonLib/"]
RUN dotnet restore "Services/Disks/Disks.gRPC.Service/Disks.gRPC.Service.csproj"


COPY [ "Services/Disks/Disks.gRPC.Service/", "Services/Disks/Disks.gRPC.Service/" ]
COPY [ "Services/Common/CommonLib/", "Services/Common/CommonLib/" ]

WORKDIR "/src/Services/Disks/Disks.gRPC.Service"
RUN dotnet build "Disks.gRPC.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Disks.gRPC.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Disks.gRPC.Service.dll"]
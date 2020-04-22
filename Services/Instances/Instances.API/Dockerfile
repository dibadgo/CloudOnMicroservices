FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Instances/Instances.API/Instances.API.csproj", "Services/Instances/Instances.API/Instances.API.csproj"]
COPY ["Services/Common/CommonLib/CommonLib.csproj", "Services/Common/CommonLib/"]
RUN dotnet restore "Services/Instances/Instances.API/Instances.API.csproj"


COPY [ "Services/Instances/Instances.API/", "Services/Instances/Instances.API/" ]
COPY [ "Services/Common/CommonLib/", "Services/Common/CommonLib/" ]

WORKDIR "/src/Services/Instances/Instances.API/"
RUN dotnet build "Instances.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instances.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Instances.API.dll"]
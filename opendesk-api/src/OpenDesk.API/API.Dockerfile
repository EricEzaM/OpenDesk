FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/OpenDesk.API/OpenDesk.API.csproj", "src/OpenDesk.API/"]
COPY ["src/OpenDesk.Infrastructure/OpenDesk.Infrastructure.csproj", "src/OpenDesk.Infrastructure/"]
COPY ["src/OpenDesk.Domain/OpenDesk.Domain.csproj", "src/OpenDesk.Domain/"]
COPY ["src/OpenDesk.Application/OpenDesk.Application.csproj", "src/OpenDesk.Application/"]
RUN dotnet restore "src/OpenDesk.API/OpenDesk.API.csproj"
COPY . .
WORKDIR "/src/src/OpenDesk.API"
ARG CONFIGURATION
RUN dotnet build "OpenDesk.API.csproj" -c ${CONFIGURATION} -o /app/build

FROM build AS publish
ARG CONFIGURATION
RUN dotnet publish "OpenDesk.API.csproj" -c ${CONFIGURATION} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenDesk.API.dll"]
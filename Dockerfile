FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Parsobober.Cli/Parsobober.Cli.csproj", "Parsobober.Cli/"]
RUN dotnet restore "Parsobober.Cli/Parsobober.Cli.csproj"
COPY ./src ./
WORKDIR "/src/Parsobober.Cli"
RUN dotnet build "Parsobober.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Parsobober.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parsobober.Cli.dll", "/app/code"]
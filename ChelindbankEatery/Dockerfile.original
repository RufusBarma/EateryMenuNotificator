FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChelindbankEatery/ChelindbankEatery.csproj", "ChelindbankEatery/"]
RUN dotnet restore "ChelindbankEatery/ChelindbankEatery.csproj"
COPY . .
WORKDIR "/src/ChelindbankEatery"
RUN dotnet build "ChelindbankEatery.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChelindbankEatery.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChelindbankEatery.dll"]

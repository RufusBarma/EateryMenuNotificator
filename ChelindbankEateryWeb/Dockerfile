FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Set the locale
RUN apt-get update && apt-get install -y locales
RUN sed -i -e 's/# ru_RU.UTF-8 UTF-8/ru_RU.UTF-8 UTF-8/' /etc/locale.gen && dpkg-reconfigure --frontend=noninteractive locales && update-locale LANG=ru_RU.UTF-8
ENV LANG ru_RU.UTF-8
ENV LANGUAGE ru_RU:ru
ENV LC_ALL ru_RU.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChelindbankEateryWeb/ChelindbankEateryWeb.csproj", "ChelindbankEateryWeb/"]
RUN dotnet restore "ChelindbankEateryWeb/ChelindbankEateryWeb.csproj"
COPY . .
WORKDIR "/src/ChelindbankEateryWeb"
RUN dotnet build "ChelindbankEateryWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChelindbankEateryWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChelindbankEateryWeb.dll"]

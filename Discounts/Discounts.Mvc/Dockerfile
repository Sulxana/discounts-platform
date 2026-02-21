FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Discounts.slnx", "./"]
COPY ["Discounts.Api/Discounts.Api.csproj", "Discounts.Api/"]
COPY ["Discounts.Mvc/Discounts.Mvc.csproj", "Discounts.Mvc/"]
COPY ["Discounts.Application/Discounts.Application.csproj", "Discounts.Application/"]
COPY ["Discounts.Domain/Discounts.Domain.csproj", "Discounts.Domain/"]
COPY ["Discounts.Infrastracture/Discounts.Infrastracture.csproj", "Discounts.Infrastracture/"]

RUN dotnet restore "Discounts.Mvc/Discounts.Mvc.csproj"

COPY . .

WORKDIR "/src/Discounts.Mvc"
RUN dotnet build "Discounts.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discounts.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# curl for healthcheck
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discounts.Mvc.dll"]

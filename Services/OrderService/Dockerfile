FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["Services/OrderService/OrderService.Domain/OrderService.Domain.csproj", "Services/OrderService/OrderService.Domain/"]
COPY ["Services/OrderService/OrderService.Application/OrderService.Application.csproj", "Services/OrderService/OrderService.Application/"]
COPY ["Services/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "Services/OrderService/OrderService.Infrastructure/"]
COPY ["Services/OrderService/OrderService.API/OrderService.API.csproj", "Services/OrderService/OrderService.API/"]

RUN dotnet restore "Services/OrderService/OrderService.API/OrderService.API.csproj"

COPY . .

WORKDIR "/src/Services/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL service="order-service"
LABEL version="1.0.0"
LABEL architecture="tomato"

ENTRYPOINT ["dotnet", "OrderService.API.dll"]

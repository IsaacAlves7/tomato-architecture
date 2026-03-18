FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
COPY ["Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "Services/ProductService/ProductService.Domain/"]
COPY ["Services/ProductService/ProductService.Application/ProductService.Application.csproj", "Services/ProductService/ProductService.Application/"]
COPY ["Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "Services/ProductService/ProductService.Infrastructure/"]
COPY ["Services/ProductService/ProductService.API/ProductService.API.csproj", "Services/ProductService/ProductService.API/"]

RUN dotnet restore "Services/ProductService/ProductService.API/ProductService.API.csproj"

COPY . .

WORKDIR "/src/Services/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL service="product-service"
LABEL version="1.0.0"
LABEL architecture="tomato"

ENTRYPOINT ["dotnet", "ProductService.API.dll"]

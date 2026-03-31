FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["DemoECommerce.ProductApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj", "DemoECommerce.ProductApiSolution/ProductApi.Presentation/"]
COPY ["DemoECommerce.ProductApiSolution/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj", "DemoECommerce.ProductApiSolution/ProductApi.Infrastructure/"]
COPY ["DemoECommerce.ProductApiSolution/ProductApi.Application/ProductApi.Application.csproj", "DemoECommerce.ProductApiSolution/ProductApi.Application/"]
COPY ["DemoECommerce.ProductApiSolution/ProductApi.Domain/ProductApi.Domain.csproj", "DemoECommerce.ProductApiSolution/ProductApi.Domain/"]
COPY ["DemoECommerce.SharedLibrarySolution/eCommerce.SharedLibrary/eCommerce.SharedLibrary.csproj", "DemoECommerce.SharedLibrarySolution/eCommerce.SharedLibrary/"]

RUN dotnet restore "DemoECommerce.ProductApiSolution/ProductApi.Presentation/ProductApi.Presentation.csproj"

COPY ["DemoECommerce.ProductApiSolution/", "DemoECommerce.ProductApiSolution/"]
COPY ["DemoECommerce.SharedLibrarySolution/", "DemoECommerce.SharedLibrarySolution/"]

WORKDIR /src/DemoECommerce.ProductApiSolution/ProductApi.Presentation
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductApi.Presentation.dll"]

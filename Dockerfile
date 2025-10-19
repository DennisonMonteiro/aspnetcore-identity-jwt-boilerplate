FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT="Development"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Identity.Api/Identity.Api.csproj", "Identity.Api/"]
COPY ["src/Identity.Data/Identity.Data.csproj", "Identity.Data/"]
COPY ["src/Identity.Domain/Identity.Domain.csproj", "Identity.Domain/"]
RUN dotnet restore "Identity.Api/Identity.Api.csproj"

COPY src/ .

WORKDIR "/src/Identity.Api"
RUN dotnet build "Identity.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]
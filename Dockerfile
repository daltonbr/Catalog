FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Catalog.csproj", "./"]
RUN dotnet restore "Catalog.csproj"
COPY . .
# We are going to skip these unnecessary steps, since publish also builds
#WORKDIR "/src/."
#RUN dotnet build "Catalog.csproj" -c Release -o /app/build

#FROM build AS publish
RUN dotnet publish "Catalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
#COPY --from=publish /app/publish .
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Catalog.dll"]

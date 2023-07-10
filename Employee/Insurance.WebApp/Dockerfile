FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Insurance.WebApp/Insurance.WebApp.csproj", "Insurance.WebApp/"]
RUN dotnet restore "Insurance.WebApp/Insurance.WebApp.csproj"
COPY . .
WORKDIR "/src/Insurance.WebApp"
RUN dotnet build "Insurance.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insurance.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5131
EXPOSE 5131
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insurance.WebApp.dll"]
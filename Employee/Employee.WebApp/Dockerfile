FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Employee.WebApp/Employee.WebApp.csproj", "Employee.WebApp/"]
RUN dotnet restore "Employee.WebApp/Employee.WebApp.csproj"
COPY . .
WORKDIR "/src/Employee.WebApp"
RUN dotnet build "Employee.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Employee.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5201
EXPOSE 5201
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Employee.WebApp.dll"]
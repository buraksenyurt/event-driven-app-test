FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EmployeeService/EmployeeService.csproj", "EmployeeService/"]
COPY ["./Common/Common.csproj", "./Common/"]
RUN dotnet restore "EmployeeService/EmployeeService.csproj"
COPY . .
WORKDIR "/src/EmployeeService"
RUN dotnet build "EmployeeService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeService.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5226
EXPOSE 5226
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeService.dll"]
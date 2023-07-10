FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["InsuranceService/InsuranceService.csproj", "InsuranceService/"]
COPY ["./Common/Common.csproj", "./Common/"]
RUN dotnet restore "InsuranceService/InsuranceService.csproj"
COPY . .
WORKDIR "/src/InsuranceService"
RUN dotnet build "InsuranceService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InsuranceService.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://*:5011
EXPOSE 5011
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InsuranceService.dll"]
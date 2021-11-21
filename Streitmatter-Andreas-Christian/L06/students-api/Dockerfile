FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["students-api.csproj", "./"]
RUN dotnet restore "./students-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "students-api.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "students-api.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "students-api.dll"]
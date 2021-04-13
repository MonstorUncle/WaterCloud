#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
WORKDIR /src
COPY ["WaterCloud.Web/WaterCloud.Web.csproj", "WaterCloud.Web/"]
RUN dotnet restore "WaterCloud/WaterCloud.Web.csproj"
COPY . .
WORKDIR "/src/WaterCloud.Web"
RUN dotnet build "WaterCloud.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WaterCloud.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WaterCloud.Web.dll"]
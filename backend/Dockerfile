FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["EmotionAnalyzeApi.csproj", "./"]
RUN dotnet restore "EmotionAnalyzeApi.csproj"
COPY . .
RUN dotnet build "EmotionAnalyzeApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmotionAnalyzeApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmotionAnalyzeApi.dll"]


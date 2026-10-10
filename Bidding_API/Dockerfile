# Build context must be the repository root:
#   docker build -f Bidding_API/Dockerfile .
# On Railway: keep service root directory "/" and set RAILWAY_DOCKERFILE_PATH=Bidding_API/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

# Restore first (better layer caching)
COPY Bidding_Models/Bidding_Models.csproj Bidding_Models/
COPY Bidding_Common/Bidding_Common.csproj Bidding_Common/
COPY Bidding_DLL/Bidding_DLL.csproj Bidding_DLL/
COPY Bidding_Core/Bidding_Core.csproj Bidding_Core/
COPY Bidding_API/Bidding_API.csproj Bidding_API/
RUN dotnet restore Bidding_API/Bidding_API.csproj

COPY Bidding_Models/ Bidding_Models/
COPY Bidding_Common/ Bidding_Common/
COPY Bidding_DLL/ Bidding_DLL/
COPY Bidding_Core/ Bidding_Core/
COPY Bidding_API/ Bidding_API/
RUN dotnet publish Bidding_API/Bidding_API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Upload/static folders the app writes to at runtime
RUN mkdir -p wwwroot/Images wwwroot/ArticleImages wwwroot/TemFiles \
    wwwroot/uploads/jobseeker-attachments App_Data/jobseeker-attachments

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Railway injects PORT; fall back to 8080 for local runs
CMD ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} dotnet Bidding_API.dll

# Use the latest .NET 9 preview SDK image
FROM mcr.microsoft.com/dotnet/nightly/sdk:9.0 AS build

# RUN apt-get update && apt-get install -y \
#     libicu-dev \
#     libgdiplus

WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
#RUN dotnet restore

# Copy the entire source and build the application
#COPY . ./
COPY ./publish/ ./  
#RUN dotnet publish -r linux-x64 --self-contained -c Release -o /app/publish

# Use a smaller runtime image
#FROM mcr.microsoft.com/dotnet/nightly/aspnet:9.0 AS runtime

#WORKDIR /app

# Copy the published app from the build stage
#COPY --from=build /app/publish ./

# Make sure the app is executable (usually needed for .NET Core apps)
#RUN chmod +x /app/dotnet-kafka-app

# Run the application
ENTRYPOINT ["./dotnet-kafka-app"]
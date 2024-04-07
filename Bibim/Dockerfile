FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

USER root
RUN apt-get update && apt-get install -y libopus-dev libsodium-dev ffmpeg yt-dlp \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bibim/Bibim.csproj", "Bibim/"]
COPY ["Services/Abstractions/Abstractions.csproj", "Services/Abstractions/"]
COPY ["Services/Discord/Discord.csproj", "Services/Discord/"]
COPY ["Services/AudioQueue/AudioQueue.csproj", "Services/AudioQueue/"]
COPY ["Services/YouTube/YouTube.csproj", "Services/YouTube/"]
COPY ["Services/YoutubeDL/YoutubeDL.csproj", "Services/YoutubeDL/"]
RUN dotnet restore "Bibim/Bibim.csproj"
COPY . .
WORKDIR "/src/Bibim"
RUN dotnet build "Bibim.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Bibim.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/bin/Debug/net8.0 \
    && cp /usr/lib/$(uname -m)-linux-gnu/libopus.so.0 /app/libopus.so \
    && cp /usr/lib/$(uname -m)-linux-gnu/libopus.so.0.8.0 /app/libopus.so.0.8.0
# RUN cp /usr/lib/x86_64-linux-gnu/libopus.so.0 libopus.so

USER $APP_UID

ENTRYPOINT ["dotnet", "Bibim.dll"]

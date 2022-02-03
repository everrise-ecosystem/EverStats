#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/EverStats.csproj", "src/"]
RUN dotnet restore "src/EverStats.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "EverStats.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EverStats.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt-get update -yqq
RUN apt-get install -yqq wget unzip fontconfig
RUN wget https://fonts.google.com/download?family=Lato -O lato.zip
RUN unzip lato.zip -d lato
RUN mv lato/* /usr/share/fonts/
RUN fc-cache -f -v
RUN fc-list | grep "Lato"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EverStats.dll"]
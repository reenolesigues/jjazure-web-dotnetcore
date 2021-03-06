FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# fix on alpine image - https://www.gitmemory.com/issue/dotnet/SqlClient/220/535820849
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["jjwebapicore/jjwebapicore.csproj", "jjwebapicore/"]
RUN dotnet restore "jjwebapicore/jjwebapicore.csproj"
COPY . .
WORKDIR "/src/jjwebapicore"
RUN dotnet build "jjwebapicore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "jjwebapicore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "jjwebapicore.dll"]
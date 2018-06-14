FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 50400
EXPOSE 44344

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY WebCV-Fiches/WebCV-Fiches.csproj WebCV-Fiches/
RUN dotnet restore WebCV-Fiches/WebCV-Fiches.csproj
COPY . .
WORKDIR /src/WebCV-Fiches
RUN dotnet build WebCV-Fiches.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebCV-Fiches.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebCV-Fiches.dll"]

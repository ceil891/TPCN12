FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 83

# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 🔧 Dòng này sửa lại tên file .fsproj
COPY ["TrienKhaiPhanMem4.fsproj", "src/"]
RUN dotnet restore "src/TrienKhaiPhanMem4.fsproj"

COPY . .
WORKDIR /src
RUN dotnet build "TrienKhaiPhanMem4.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrienKhaiPhanMem4.fsproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrienKhaiPhanMem4.dll"]

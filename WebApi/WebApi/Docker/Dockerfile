FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
RUN git clone https://github.com/ErnestESeleznev/TZ002.git
RUN dotnet publish -c Release -o out TZ002/WebApi/WebApi.slnx

#ENTRYPOINT ["sleep", "infinity"]


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

#ENTRYPOINT ["sleep", "infinity"]
ENTRYPOINT ["./WebApi"]

FROM microsoft/dotnet:2.1-runtime-alpine
RUN mkdir netapp
COPY ./bin/Release/netcoreapp2.0/publish/ /netapp
WORKDIR netapp
ENTRYPOINT ["dotnet", "server.dll"]
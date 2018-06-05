FROM microsoft/dotnet:2.1.0-runtime-alpine3.7
RUN mkdir netapp
COPY ./bin/Release/netcoreapp2.0/publish/ /netapp
WORKDIR netapp
ENTRYPOINT ["dotnet", "server.dll"]
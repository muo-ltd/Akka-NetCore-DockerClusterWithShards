FROM microsoft/dotnet:2.0.5-runtime
RUN mkdir netapp
COPY ./bin/Debug/netcoreapp2.0/publish/ /netapp
WORKDIR netapp
ENTRYPOINT ["dotnet", "client.dll"]
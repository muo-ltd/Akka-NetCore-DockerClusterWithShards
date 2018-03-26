FROM microsoft/dotnet:2.0.6-runtime-nanoserver-1709
RUN mkdir netapp
COPY ./bin/Release/netcoreapp2.0/publish/ /netapp
WORKDIR netapp
ENTRYPOINT ["dotnet", "client.dll"]
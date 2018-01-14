FROM microsoft/dotnet:2.0.0-sdk
COPY /deploy /
WORKDIR /Server
EXPOSE 8080
EXPOSE 8085
ENTRYPOINT ["dotnet", "Server.dll"]
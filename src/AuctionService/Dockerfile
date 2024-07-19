FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
# copy all .csproj files and restore as distinct layers. Use of the same copy command for every dockerfile in the project to take
# of the docker caching mechanism
COPY Carolls.sln Carolls.sln
COPY Auctions/AuctionService.csproj Auctions/AuctionService.csproj 
COPY SearchService/SearchService.csproj SearchService/SearchService.csproj
COPY GatewayService/GatewayService.csproj GatewayService/GatewayService.csproj
COPY Contracts/Contracts.csproj Contracts/Contracts.csproj
COPY IdentityService/IdentityService.csproj IdentityService/IdentityService.csproj

# Restores package deps 
RUN dotnet restore Carolls.sln

# Copy the app folders
COPY Auctions src/AuctionService
COPY Contracts src/Contracts
WORKDIR /app/src/AuctionService
RUN dotnet publish Carolls.sln -c Release -o /app/src/out


#build runtime image  
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app 
COPY --from=build /app/src/out .
ENTRYPOINT ["dotnet", "AuctionService.dll"]
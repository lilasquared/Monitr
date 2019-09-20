FROM node:12 as client
WORKDIR /src

COPY ./package.json ./yarn.lock ./

RUN yarn

COPY . .

RUN yarn build

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS server

WORKDIR /src

COPY ./src/server .
RUN dotnet restore Monitr.sln
RUN dotnet publish Monitr.sln --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

RUN curl -sSL https://get.docker.com/ | sh

WORKDIR /opt
COPY --from=server /app .
COPY --from=client /src/build /opt/wwwroot

ENTRYPOINT ["dotnet", "Monitr.dll"]
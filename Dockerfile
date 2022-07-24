FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /src
COPY ["AspNetIntegrationTesting/AspNetIntegrationTesting.csproj", "AspNetIntegrationTesting/"]
RUN dotnet restore "AspNetIntegrationTesting/AspNetIntegrationTesting.csproj"
WORKDIR "/src/AspNetIntegrationTesting"
COPY ["AspNetIntegrationTesting/.", "."]
RUN dotnet publish "AspNetIntegrationTesting.csproj" -c Release -r linux-x64 --no-self-contained -p:PublishReadyToRun=true -o /app/publish

FROM base AS final

# Install Chrome
ARG CHROME_VERSION="103.0.5060.134-1"
RUN apt-get update && apt-get -y install wget
RUN wget --no-verbose -O /tmp/chrome.deb http://dl.google.com/linux/chrome/deb/pool/main/g/google-chrome-stable/google-chrome-stable_${CHROME_VERSION}_amd64.deb \
&& apt-get install -y /tmp/chrome.deb --no-install-recommends --allow-downgrades \
&& rm /tmp/chrome.deb

ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetIntegrationTesting.dll"]
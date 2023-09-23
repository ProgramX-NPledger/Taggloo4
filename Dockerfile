FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /ConsoleApp1TestingDocker

COPY . .

ENV PATH="${PATH}:/ConsoleApp1TestingDocker/bin/Debug/net7.0"

CMD ["ConsoleApp1TestingDocker.exe"]

# Frends.As4.SendMessage

Task to send messages with AS4 protocol.

[![SendMessage_build](https://github.com/FrendsPlatform/Frends.As4/actions/workflows/SendMessage_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.As4/actions/workflows/SendMessage_build_and_test_on_main.yml)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.As4/Frends.As4.SendMessage|main)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

## Installing

You can install the Task via Frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.As4.git`

### Build the project

`dotnet build`

### Run tests

* For a local AS4 partner first build the bundled receiver: `dotnet build Frends.As4.SendMessage.Tests/docker/receiver/As4TestReceiver.csproj`

* Then start it from `Frends.As4.SendMessage.Tests/docker`:
`docker compose up`

* And run tests with: `dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### Third party licenses

StyleCop.Analyzer version (unmodified version 1.1.118) used to analyze code uses Apache-2.0 license, full text and
source code can be found at https://github.com/DotNetAnalyzers/StyleCopAnalyzers

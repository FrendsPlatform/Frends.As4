# Frends.As4.ValidateAndParsePayload

Task to validate an incoming AS4 message, extracts the EDI payload, and generates an MDN receipt

[![ValidateAndParsePayload_build](https://github.com/FrendsPlatform/Frends.As4/actions/workflows/ValidateAndParsePayload_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.As4/actions/workflows/ValidateAndParsePayload_test_on_main.yml)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.As4/Frends.As4.ValidateAndParsePayload|main)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

## Installing

You can install the Task via Frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.As4.git`

### Build the project

`dotnet build`

### Run tests

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### StyleCop.Analyzers Version
This project uses StyleCop.Analyzers 1.2.0-beta.556, as recommended by the author, to get the latest fixes and improvements not available in the last stable release.

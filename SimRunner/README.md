# SimRunner
This .NET 8 application takes in the SimSharer output, runs the simulations and updates WoWAudit.

## Installation
- Pull this repo
```shell
git pull https://github.com/JoschiZ/WoWSimSync
```
- Build and publish the project
```shell
cd WoWSimSync
cd SimRunner
dotnet build 
dotnet tool update --global PowerShell
pwsh bin/Debug/net8.0/playwright.ps1 install
dotnet publish
```
- Go into the publish directory and provide your audit API key

## Usage
- Paste the SimSharer output into the `input.json` file.
- Run the executable
- Log into your raidbots account

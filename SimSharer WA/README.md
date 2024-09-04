# Sim Sharer
This WA provides mechanism for autonomous SimC string collection from your raid.
It consists of two different Auras.

## Sharer - Client
This lightweight WA uses the SimulationCraft Addon to export your SimC string and POST it to the server aura.
There is no user interaction required.
Everybody of whoom you want to collect the simc strings needs install this aura.

## Sharer - Server
The server aura will send GET requests to each client in your Raid and collect their SimC responses.
It reacts to three different command macros.
Only the one who wants to collect the strings needs this aura.

### 1. Collection Start
    /run WeakAuras.ScanEvents("SHARER_START_COLLECTION")
Sends out the GET request and opens the server for PUT requests.

### 2. Export 
    /run WeakAuras.ScanEvents("SHARER_EXPORT_STRINGS")
JSON formats the gathered information and opens a window for copy pasting.

### 3. Reset
    /run WeakAuras.ScanEvents("SHARER_RESET")
This command will delete all stored information and close the server.
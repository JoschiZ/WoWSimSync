# WoW-SymSync

This project aims to provide a streamlined way of updating the [WoW Audit](https://wowaudit.com/) wishlist for your whole guild at once, with minimal user interaction.

## How does it work?
There are two parts to this project.

First a [WeakAuras2](https://github.com/WeakAuras/WeakAuras2) WeakAura, which will collect the [SimulationCraft](https://simulationcraft.org/) strings from your raid members and export them in a machine readable `.json` format.
See [here](SimSharer%20WA) for the documentation.

Secondly there is a .NET 8 application, which injests this `.json` output and runs Droptimizer simulations using PlayWright and [Raidbots](https://www.raidbots.com/simbot).
It then provides the reports to the Audit API.
See [here](SimRunner) for the documentation and installation guide.
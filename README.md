## Adam Plugins
This repository contains AdamAdam Unturned plugins he was selling on ImperialPlugins.com but discontinued them.  
Some of them were fixed and improved by Restore Monarchy.

Join Restore Monarchy [Discord](https://discord.gg/Z3BWae5) if you need help  
Download lastest version of each plugin from [Releases](https://github.com/RestoreMonarchyPlugins/AdamPlugins/releases)


### Restore Monarchy fixes and improvements
*UPlayerLoot, QueueBypasser and SentryCommander weren't fixed because I didn't find them too useful, but their source codes are included*  

**[UAuction](UAuction.md) 3.0**
* Added MinimumTimeAfterBid to config. When there's a new bid and the auction time is under MinimumTimeAfterBid, it is set to MinimumTimeAfterBid
* Fixed plugin not return auction items and bids if server shutdown or crash

**[UBankRobbery](UBankRobbery.md) 3.0**
* Removed support for RocketRegions, but added support for AdvancedZones
* Fixed and restored support for AdvancedRegions
* Added Uconomy support for rewards

**[UPets](UPets.md) 3.0**
* Completely rewrote the plugin
* Changed data saving from file to json file database writing (still allowing MySQL)
* Improved performance of the plugin (MySQL database calls are done asynchronously, less ticks) 
* Removed support for UI
* Improved pet command

**[InfoRestorer](InfoRestorer.md) 3.0**
* Minimum small improvements

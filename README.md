UAuction:
- Added MinimumTimeAfterBid to config. When there's a new bid and the auction time is under MinimumTimeAfterBid, it is set to MinimumTimeAfterBid
- Fixed plugin not return auction items and bids if server shutdown or crash

UBankRobbery:
- Removed support for RocketRegions, but added support for AdvancedZones
- Fixed and restored support for AdvancedRegions
- Added Uconomy support for rewards

UPets:
- Completely rewrote the plugin
- Changed data saving from file to json file database writing (still allowing MySQL)
- Improved performance of the plugin (MySQL database calls are done asynchronously, less ticks) 
- Removed support for UI
- Improved pet command

InfoRestorer:
- Minimum small improvements
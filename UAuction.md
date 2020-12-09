## UAuction

29th November 2020 Update log `v3.0.0`:
```
- Added MinimumTimeAfterBid to config. When there's a new bid and the auction time is under MinimumTimeAfterBid, it is set to MinimumTimeAfterBid
- Fixed plugin not return auction items and bids if server shutdown or crash
```

**Features**
* UI with current auction info
* All barricades on vehicles are saved when auctioned away, as well as fuel, battery etc.
* When auctioning away items a storage box pops up for you to put in all the items you want to auction away.
* You can preview the current auction items by typing /previewauction or (/previewa) and a storage container pops up with all items being auctioned away. You can not take them out and it's 100% glitch safe.
* An auction is not cancelled by the owner or a bidder leaving.
* An auction queue exists.
* An auction interval timer exists (so there's a pause between auctions).
* You can customize an icon in the chat (e.g an icon pops up before all messages produced by the plugin)
* All attachments, magazine etc is saved on the item, as well as health.
* Announcments on the auction countdown configurable, e.g when 30 seconds left say **this**.

**Commands**
* /bid <amount> - Bids on the current auction
* /startauction <starting bid> - Starts an auction, if there's an auction running you're put in the queue.
* /auctionvehicle <starting bid> - Starts an auction of the vehicle you're currently sitting in. The player needs to be the driver of the vehicle.
* /cancelauction - Cancels the auction, but only if it's in queue. If not then  the auction continues.
* /previewauction - Previews the auction via showing the player a storage container of the auctioning items. 100% glitch proof.

[Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2303086930)
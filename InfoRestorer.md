## InfoRestorer
### Features
* Everytime a player dies it makes a backup of the inventory for admins to be able to restore it.

#### Commands
`/restore <player> <deaths ago>`

#### Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<InfoRestorerConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <InfoStorageCapacity>30</InfoStorageCapacity>
  <RemoveInfoOnLeave>true</RemoveInfoOnLeave>
  <ShouldClearInventory>true</ShouldClearInventory>
</InfoRestorerConfiguration>
```

#### Translations
```xml
<?xml version="1.0" encoding="utf-8"?>
<Translations xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Translation Id="invalid_syntax" Value="Invalid Syntax! Usage: /restore &lt;player&gt; &lt;times ago&gt;" />
  <Translation Id="player_not_found" Value="Player not found!" />
  <Translation Id="not_number" Value="{0} is not a number higer then 0!" />
  <Translation Id="too_much" Value="{0} hasn't died that much!" />
  <Translation Id="restored" Value="You've succesfully restored {0}'s inventory!" />
</Translations>
```

#### Permissions:
`restore` - to use /restore
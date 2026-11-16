# Choose Your Own Adventure

A console adventure in which a Player creates a character, chooses a Path, and may face a Dragon. All player-facing text is available in several Languages.

## Language

**Language**:
One complete set of player-facing text the game can be played in (currently English, Spanish, French, Italian).
_Avoid_: Locale, translation

**Canonical Name**:
The language-independent name the game rules use for a Race, Occupation, Weapon, or Armor (e.g. `Halfling`, `Fighter`). Every Language translates to and from it.
_Avoid_: English name

**Command**:
A fixed input the game accepts identically in every Language (e.g. `n`, `s`, `a`, `r`, `roll`, and numbered menu choices). Commands are never translated; translated text only tells the Player which Command to type.
_Avoid_: Localized command, shortcut

## Adventure

**Path**:
A direction the Player chooses at the crossroads: North (leads to the Dragon), South (leads to the Merchant), or West (leads to a risk/reward detour).

**Merchant**:
The character met every time the Player takes the South Path. The Merchant offers a choice of one Magical Item; once the Player has one, the Merchant refuses to sell anything else.
_Avoid_: Shop, vendor, trader

**Magical Item**:
Either Magical Armor or a Magical Weapon, as offered by the Merchant. A Player receives at most one.

**Magical Armor**:
Protection that reduces the damage of every Dragon hit on the Player by a fixed amount.
_Avoid_: Shield, defense bonus

**Shrine**:
The place at the end of the West Path. The Player may make one Prayer there per adventure; once they have prayed, the Shrine is silent on later visits. Leaving without praying does not use up the Prayer.
_Avoid_: Temple, altar, fountain

**Prayer**:
The Player's single gamble at the Shrine, which can raise or lower their HealthPoints.
_Avoid_: Wish, offering, bet

**Blessing**:
A Prayer outcome that raises the Player's HealthPoints.

**Curse**:
A Prayer outcome that lowers the Player's HealthPoints, but never below 1, and never raises them. Only the Dragon can kill the Player.
_Avoid_: Smite, punishment

**Magical Weapon**:
The Player's own Weapon, enchanted by the Merchant to deal more damage. It keeps its Occupation's character (a magical long sword is still a long sword).
_Avoid_: New weapon, replacement weapon

# Choose Your Own Adventure (CYOA) Game

A C# (.NET 10) console game for CIDM Software Systems Development. Players create a character, choose a path (north to the Dragon, south to a Merchant, west to a Shrine), check their Inventory, and fight or flee from the Dragon. Supports English, Spanish, French, and Italian.

```
dotnet run                # play
dotnet test CYOATests     # run the tests
```

## Development Changes

### 1. Fixed `Messages.cs` bugs
**Branch:** `fix/messages-null-maps-and-language-reload` · **Commit:** `5b51db2`

- **Null display maps:** The constructor never set up the display maps, so translating before `ReadDictionary()` crashed. They are now set up in the constructor, which also removed three CS8618 build warnings.
- **Language switching did nothing:** `SetCurrentLanguage()` now reloads the text if text was already loaded.

### 2. Added Italian
**Branch:** `feature/italian-language` · **Commit:** `7f412ab`

- Added an `"Italian"` section to `language_data.json` and `4. Italiano` to the language menu.
- Added 3 tests, including one that fails if Italian is missing any key English has.

Commands (`n`, `s`, `w`, `i`, `e`, `a`, `r`, `roll`) stay in English, and accents are left out. See [ADR 0001](docs/adr/0001-add-italian-language.md).

### 3. Added the South Path Merchant
**Branch:** `feature/south-merchant` · **Commits:** `e54bdcd`, `776c067`

- Going south (`s`) meets a Merchant who gives one free Magical Item: Magical Armor (each Dragon hit does 2 less damage) or a Magical Weapon (+2 max damage). After that, the Merchant refuses.
- The rules live in `Merchant.cs`, with no console code.

### 4. Added the West Path Shrine
**Commit:** `1050de1`

- Going west (`w`) leads to a Shrine with one Prayer per adventure. A d6 decides it: 1-2 Curse (-3 HP, never below 1), 3-4 nothing, 5-6 Blessing (+3 HP). Leaving keeps the Prayer for later.
- The rules live in `Shrine.cs`, with no console code.

### 5. Added the Inventory Command
**Commit:** `73494d9`

- Typing `i` or `inventory` at the crossroads shows the Weapon and its max damage, plus Magical Armor if owned, then returns to the path prompt. Looking changes nothing.
- The lines come from `Inventory.cs`, with no console code.

### 6. Fixed Halfling starting health
**Commit:** `b666bb9`

- Starting HealthPoints are a d20 roll plus the Strength modifier, but never below 1. A Halfling's negative modifier could make them start at 0 or less.
- `Player` now takes its `Die` in the constructor, so tests can script character-creation rolls.

### 7. One Prompt module for all input
**Commits:** `5591014`, `e7dfc53`, `c3a9623`, `a617276`, `418da80`, `79b7c41`

- Every prompt goes through `Prompt.cs`: ask, re-ask with an error on invalid input. The language menu still reads once.
- If input runs out at any prompt (for example, a closed console), the adventure ends with "Adventure ends. Bye." instead of looping forever.
- Invalid input at the combat prompt now asks again instead of counting as an attack.
- `Game` takes an optional input, output and `Die`, so tests play whole adventures, including Dragon fights and Prayers, with scripted rolls. Shared test helpers live in `CYOATests/TestSupport.cs`.

### 8. Documentation
- Added [ADR 0001](docs/adr/0001-add-italian-language.md) (Italian) and [ADR 0002](docs/adr/0002-magical-weapon-canonical-names.md) (Magical Weapon names).
- Added domain terms to `GLOSSARY.md`.
- Specs and tickets for each feature live in `.scratch/`.

## Test Status
262 MSTest tests, all passing.

## Known Issues (not yet fixed)
- `ReadDictionary()` has no error handling for a missing or malformed JSON file.
- The language menu is hard-coded in `Game.cs` instead of read from the JSON file.

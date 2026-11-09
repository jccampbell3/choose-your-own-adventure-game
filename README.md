# Choose Your Own Adventure (CYOA) Game

A C# (.NET 10) console game for CIDM Software Systems Development. Players create a character, choose a path, and fight or flee from a dragon. Supports English, Spanish, French, and Italian.

```
dotnet run                # play
dotnet test CYOATests     # run the tests
```

## Development Changes

### 1. Fixed `Messages.cs` bugs
**Branch:** `fix/messages-null-maps-and-language-reload` · **Commit:** `5b51db2`

- **Null display maps:** The constructor never set up `_displayRaceMap`, `_displayOccupationMap`, or `_displayWeaponMap`, so calling a `Translate...ForDisplay` method before `ReadDictionary()` crashed. They are now set up in the constructor, which also removed the three CS8618 build warnings.
- **Language switching did nothing:** `SetCurrentLanguage()` changed the language name but not the loaded text. It now reloads the text if text was already loaded, and `CurrentLanguage` has a private setter.

### 2. Added Italian
**Branch:** `feature/italian-language` · **Commit:** `7f412ab`

- **`language_data.json`:** Added an `"Italian"` section with all messages, and with race, occupation, and weapon translations that map to the English names the game logic uses.
- **`Game.cs`:** Added `4. Italiano` to the language menu.
- **Tests:** Added 3 tests, including one that fails if Italian is missing any key English has.

Commands (`n`, `s`, `e`, `a`, `r`, `roll`) stay in English, and accents are left out to match the existing languages. See [ADR 0001](docs/adr/0001-add-italian-language.md).

### 3. Documentation
- Added [ADR 0001](docs/adr/0001-add-italian-language.md) for the Italian decision (commit `bc93769`).
- Rewrote this README.

## Test Status
97 MSTest tests, all passing, with no build warnings when the `Messages.cs` fix is included.

## Known Issues (not yet fixed)
- Invalid input at the combat prompt counts as an attack.
- A Halfling can start with 0 or less health.
- `ReadDictionary()` has no error handling for a missing or malformed JSON file.
- The language menu is hard-coded in `Game.cs` instead of read from the JSON file.

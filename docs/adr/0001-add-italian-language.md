# ADR 0001: Add Italian as a Supported Language

- **Status:** Accepted
- **Date:** 2026-11-09
- **Decider:** John Campbell
- **Related branch:** `feature/italian-language` (commit `7f412ab`)

## Context

The Choose Your Own Adventure game let the player choose English, Spanish, or French at startup. The game's text is data-driven:

- Every message the game prints is stored in `language_data.json` and looked up by key with `Messages.GetMessage(key)`.
- Each language has six sections: `dictionary`, `raceMap`, `occupationMap`, `displayRaceMap`, `displayOccupationMap`, and `displayWeaponMap`.
- The game logic in `Player.cs`, `Dragon.cs`, and `Combat.cs` only works with English canonical names such as `"Halfling"` and `"Fighter"`. The maps convert the player's typed input to those names and convert them back for display.
- The language menu and its number-to-language mapping were hard-coded in `Game.StartGame()`.

We wanted to support Italian-speaking players. We also wanted to confirm that the existing design could take a new language without changes to the game logic.

## Decision

We added Italian as a fourth language, following the same pattern as the existing languages:

1. **`language_data.json`:** We added an `"Italian"` section that contains all six sections. It translates every dictionary message and maps Italian race and occupation names (`umano`, `elfo`, `nano`, `mezzuomo`; `guerriero`, `mago`, `ladro`, `arciere`) to the English canonical names.
2. **`Game.cs`:** We added `4. Italiano` to the language menu and mapped input `"4"` to `"Italian"`. Unrecognized input still defaults to English.
3. **Tests:** We added tests in `CYOATests/MessagesTests.cs` that load the Italian welcome message, check that Italian has every key English has, and check that Italian input maps to the right canonical names.

We also made these choices about the text:

- **Commands stay in English.** The game checks for the exact inputs `n`, `s`, `e`, `a`, `r`, and `roll`, so the Italian prompts tell players to type these English letters, the same way the Spanish and French prompts do.
- **No accented characters.** We wrote "e" instead of "è" and "puo" instead of "può", the same way the Spanish and French text leaves out accents. This avoids garbled characters in Windows consoles that are not set to UTF-8.
- **Full story text.** The Italian victory, defeat, and retreat endings translate the full English text instead of the shorter Spanish and French versions.

We rejected these alternatives:

- **Translating command letters**, for example `a` for *attacca*. This would mean changing input handling in `Game.cs`, `Combat.cs`, and `Player.cs`, which is a larger change than adding the language.
- **Reading the language menu from the JSON file.** This would let new languages be added without any code change. We left it for a possible future change to keep this one small.

## Consequences

**Positive**

- Italian-speaking players can play the whole game in Italian.
- Only `language_data.json` and one menu in `Game.cs` changed, so the game logic was untouched. This shows the data-driven design works as intended.
- The key-parity test catches any message that is added to English but not translated into Italian.

**Negative / Trade-offs**

- Every new message key must now be added in four places instead of three, which increases translation upkeep.
- Writing Italian without accents is grammatically imperfect (for example, "e" means "and" while "è" means "is"). Native speakers may find it slightly awkward.
- The translations have not been reviewed by a native Italian speaker.
- Players still type English command letters, which may be confusing in an Italian game.
- Adding a fifth language would still require a code change in `Game.cs`, because the menu is hard-coded.
- Only Italian has a test that checks it has every English key. Spanish and French have only a smaller list of required keys checked.

**Follow-up options**

- Have a native speaker review the translations.
- Build the language menu from the top-level keys in `language_data.json`.
- Extend the key-parity test to every language.
- Switch the console to UTF-8 (`Console.OutputEncoding = Encoding.UTF8`) and restore accents in all languages.

# 02: The game is given its input, output and Die

**What to build:** The game can be created with an input reader, an output writer and a Die. A game created with none of them uses the real console and a real Die, so the program entry point and normal play are unchanged. Every console read and write in the game, the Player, the Dragon and Combat goes through the reader and writer the game was given (stats screens, combat text and Dragon stats included), and the game passes its Die to the Player, the Dragon and the Shrine. The shared run-a-scripted-game helper now builds a game with scripted input, captured output and a scripted Die instead of swapping the global console. With dice scriptable, the North Path and the Shrine get deterministic end-to-end tests, and the Dragon-encounter test that calls a private method by reflection is rewritten through the public game entry point. No gameplay changes. See the parent spec at `.scratch/console-prompting/spec.md`.

**Blocked by:** 01 (Shared game-test helpers)

**Status:** resolved

- [ ] The game can be created with an input reader, an output writer and a Die; with none given, it uses the real console and a real Die
- [ ] Tests that still swap the global console before creating a game keep working
- [ ] Every read and write in the game, the Player, the Dragon and Combat goes through the given reader and writer; only the program entry point and the game's defaults refer to the console
- [ ] The game passes its Die to the Player, the Dragon and the Shrine
- [ ] The shared run-a-scripted-game helper uses scripted input, captured output and a scripted Die, not the global console
- [ ] A scripted North Path fight is tested end to end for victory, defeat and retreat
- [ ] Each Prayer outcome (Blessing, nothing, Curse) is tested end to end with a scripted roll, replacing the pattern-match check
- [ ] The Dragon-encounter test no longer uses reflection; it runs a scripted North Path through the public game entry point
- [ ] All existing tests pass; no player-facing text or behaviour changes

## Comments

**2026-11-16:** Implemented in e7dfc53.

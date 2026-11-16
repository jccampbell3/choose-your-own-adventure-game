# Spec: One Prompt module for all console input

Status: ready-for-agent

## Problem Statement

Every place the game asks the Player for input writes its own "ask, read, check, ask again" loop against the global console. There are about ten of these loops: the main menu, character creation (Race, name, Occupation, each `roll`, and the next-action menu), the Path prompt, the Merchant, the Shrine, the Dragon encounter, and the combat prompt. Because each loop is written by hand, they behave differently:

- When input runs out (a closed console, or a test script that ends), only the Merchant and Shrine stop. Every other prompt loops forever.
- At the combat prompt, invalid input is silently treated as an attack, so a typo can cost the Player the fight.
- The game creates its own console access and its own Die, so tests must swap the global console and cannot script the Dragon fight or a Prayer. The North Path has no deterministic end-to-end test, the Prayer test can only match a pattern, and one Dragon test reaches a private method through reflection.
- The game-level test helpers (run the game with scripted input, count occurrences, create an English Fighter, load Messages, the scripted Die) are copied across several test files.

## Solution

All console input goes through one Prompt module. It shows a prompt, reads a line, parses it, shows an error and asks again on invalid input, and reports when input has run out. The game is given its input, its output and its Die when it is created, and passes them to everything that reads, writes or rolls. When input runs out at any prompt, the adventure ends cleanly with the existing "adventure ends" message. Invalid input at the combat prompt shows the error and asks again, without attacking. Tests build a game with scripted input, captured output and a scripted Die, so every Path, including a full Dragon fight and every Prayer outcome, can be tested deterministically.

## User Stories

1. As a Player, I want invalid input at the combat prompt to show an error and ask again, so that a typo never counts as an attack.
2. As a Player, I want the combat prompt to keep accepting `a`/`attack` and `r`/`retreat` exactly as before, so that the Commands do not change.
3. As a Player, I want every other prompt to accept exactly the same Commands and answers as before, so that the game plays the same.
4. As a Player, I want every prompt to show the same text and the same error messages as before, in my Language, so that nothing I read changes.
5. As a Player whose input closes (for example, a closed console), I want the adventure to end with the "adventure ends" message, so that the game never hangs.
6. As a Player, I want input running out at the main menu to end the game cleanly.
7. As a Player, I want input running out during character creation (Race, name, Occupation, `roll`, or the next-action menu) to end the game cleanly.
8. As a Player, I want input running out at the Path prompt to end the game cleanly.
9. As a Player, I want input running out at the Merchant or the Shrine to end the game cleanly.
10. As a Player, I want input running out at the Dragon encounter or the combat prompt to end the game cleanly.
11. As a Player, I want the language menu to behave as before: any answer other than 2, 3 or 4 picks English, and running out of input there also picks English.
12. As a Player, I want the Inventory Command to keep showing my Inventory and then the Path prompt again.
13. As a Player, I want the Merchant and Shrine to keep re-asking after invalid input, as before.
14. As a Player, I want the game to roll its dice exactly as before during normal play, so that the odds do not change.
15. As a maintainer, I want one module to own asking, re-asking and end of input, so that a fix to prompting is made once.
16. As a maintainer, I want to create the game with my own input, output and Die, so that tests do not have to swap the global console.
17. As a maintainer, I want a parameterless game to use the real console and a real Die, so that the program entry point does not change.
18. As a maintainer, I want all game output, including stats screens, combat text and Dragon stats, to go to the output the game was given, so that a test sees everything the Player would see.
19. As a maintainer, I want to script a full North Path fight with a scripted Die, so that victory, defeat and retreat can be tested deterministically.
20. As a maintainer, I want to script each Prayer outcome (Blessing, nothing, Curse) through the game, so that the West Path test checks the actual outcome, not a pattern.
21. As a maintainer, I want the Dragon-encounter test to go through the public game entry point, so that no test reaches private methods by reflection.
22. As a maintainer, I want the character-creation tests to pass scripted input and output instead of swapping the global console.
23. As a maintainer, I want one shared set of test helpers (run a scripted game, count occurrences, the English-Fighter script, load Messages, the scripted Die), so that new game tests do not copy them again.
24. As a maintainer, I want existing tests that still swap the global console to keep working during the change, so that the migration can be gradual.
25. As a maintainer, I want the README's Known Issues to stop listing "invalid combat input counts as an attack" and "the game loops forever when input runs out".

## Implementation Decisions

- **Prompt module (new).** A deep module with one ask operation. A caller gives it the prompt text, a parse rule (input → a valid choice or no choice) and the text to show for invalid input, and gets back the parsed choice. The module shows the prompt, reads a line, re-shows the error and the prompt until the parse rule accepts, and owns the end-of-input rule. It is created with the game's input reader and output writer. Prompts that are written without a trailing newline today (the "enter choice" line) keep that look.
- **End of input (one rule).** When the reader has no more input, the Prompt module raises an end-of-input signal (an exception). Only the game's start method catches it: it prints the `adventure_ends` message in the current Language and returns. No other caller handles end of input. This replaces the Merchant's and Shrine's "end of input counts as Leave", so at the end of input those two no longer print their leave line before the game ends.
- **Game (modified).** The game is created with an input reader, an output writer and a Die. Each is optional: the parameterless form uses the real console input, the real console output and a real Die, so the program entry point is unchanged. Existing tests that swap the global console before creating a game keep working, because the default is read when the game is created. The game passes the reader and writer (through the Prompt module) and the Die to the Player, the Dragon, Combat, the Merchant and Shrine encounters, and the Inventory display.
- **Parse rules.** The Merchant and Shrine keep their existing choice parsing and pass it to the Prompt module. Small parse rules are added for the main menu, the next-action menu, `roll`, the Dragon-encounter choice and the combat choice. Race and Occupation parse through Messages as today. The name accepts any non-empty text.
- **Combat (modified).** The combat prompt goes through the Prompt module, so invalid input shows the error and asks again without attacking. `a`/`attack` continues the fight; `r`/`retreat` retreats as before.
- **Player, Dragon, Combat output (modified).** Every console read or write in these modules goes through the writer and Prompt module they are given. After this change, only the program entry point and the game's parameterless defaults refer to the console.
- **Character creation (modified).** Character creation is given the Prompt module and writer instead of reading the console. The Player already takes its Die when it is created.
- **Language menu (unchanged behaviour).** It is shown before Messages is loaded, so it stays a single read through the game's reader, with no re-asking: 2, 3 or 4 picks Spanish, French or Italian; anything else, including end of input, picks English.
- **Paths stay as they are.** The Path prompt moves onto the Prompt module, but Paths are still the existing text values, and the `IsValid…Choice` validators and their tests are untouched. Turning Paths into Commands and removing the validators is a separate change (architecture candidate #2). Inventory stays "show it, then ask again".
- **No new text.** No Language data changes: all prompts and errors use existing messages.

## Testing Decisions

- **What makes a good test here:** drive the game the way a Player does and check what the Player sees and what happens to the Player. Do not test the Prompt module's internals or private game methods.
- **Seams (agreed: two, both existing):**
  1. **Game entry point**, now built with a scripted reader, a captured writer and a scripted Die, instead of swapping the global console. Cases: end of input at each kind of prompt (main menu, character creation, Path, Merchant, Shrine, Dragon encounter, combat) ends with `adventure_ends` and the game returns; invalid combat input re-asks without attacking; a full scripted North Path fight to victory, defeat and retreat; each Prayer outcome with a scripted roll; the existing South, West and Inventory scripts still pass.
  2. **Character creation**, given scripted input and output instead of the global console (the existing starting-health tests move onto this).
- **No Prompt-module seam.** The Prompt module is tested only through the two seams above.
- **Replace the reflection test.** The Dragon-encounter test that calls a private game method by reflection is rewritten as a scripted North Path through the game entry point.
- **Shared test support.** One shared test-support file holds: run a scripted game and return its output, count occurrences, the English-Fighter creation script, load Messages for a Language, and the scripted Die (moved out of the Dragon Armor tests). The South, West and Inventory game tests and the starting-health tests use it instead of their own copies.
- **Prior art:** the South, West and Inventory game tests (scripted game runs), the starting-health tests (character creation with a scripted Die), and the Merchant and Shrine tests (parse rules).

## Out of Scope

- Turning Paths into Commands, a Crossroads module, or removing the `IsValid…Choice` validators (architecture candidate #2).
- Separating combat rules from combat text (candidate #3).
- Reusing the Inventory's Weapon and Armor lines on the stats screen and in combat (candidate #4).
- Moving HealthPoints rules onto the Player (candidate #5).
- Reading the Language list from the language data (candidate #6).
- Any new or changed player-facing text, or new Commands.
- Changing dice odds or any game rule.

## Further Notes

- Comes from the 2026-11-16 architecture review (candidate #1, "Deepen console prompting behind one seam") and the follow-up grilling session.
- Domain terms follow `GLOSSARY.md`: Player, Path, Command, Language, Merchant, Shrine, Prayer, Dragon, Inventory.
- No ADR: every decision here is easy to reverse.
- After this lands, update the README: remove the two input bugs from Known Issues and update the test count.

## Comments

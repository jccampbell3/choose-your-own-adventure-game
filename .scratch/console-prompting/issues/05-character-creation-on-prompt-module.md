# 05: Character creation on the Prompt module

**What to build:** Character creation (Race, name, Occupation, each `roll`, and the next-action menu) asks through the Prompt module instead of reading the console itself, with the same prompts, answers and error messages as before. Input running out at any of these prompts ends the adventure with the "adventure ends" message. The starting-health tests drive character creation with scripted input and captured output instead of swapping the global console. See the parent spec at `.scratch/console-prompting/spec.md`.

**Blocked by:** 03 (Prompt module: running out of input ends the adventure at the menus, the Path, the Merchant and the Shrine)

**Status:** ready-for-agent

- [ ] The Race, name, Occupation, `roll` and next-action prompts ask through the Prompt module, with the same text, accepted answers and error messages as before
- [ ] Input running out at any character-creation prompt prints the "adventure ends" message and the game returns
- [ ] The starting-health tests use scripted input and captured output, not the global console, and still pass
- [ ] Game-entry-point tests cover end of input during character creation
- [ ] After this ticket and 04, no game code reads input except through the Prompt module (the language menu's single read aside)
- [ ] All existing tests pass

## Comments

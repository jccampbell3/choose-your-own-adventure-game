# 03: Prompt module: running out of input ends the adventure at the menus, the Path, the Merchant and the Shrine

**What to build:** A Prompt module that owns asking for input: it shows a prompt, reads a line, parses it with a rule the caller gives, shows the caller's error text and asks again on invalid input, and raises an end-of-input signal when input runs out. Only the game's start method handles that signal: it prints the "adventure ends" message in the current Language and returns. The main menu, the Path prompt, the Merchant and the Shrine move onto it. The Path prompt keeps its current Path values and Inventory behaviour (show the Inventory, then ask again); the `IsValid...Choice` validators are untouched. The Merchant and Shrine keep their own choice parsing, but no longer treat end of input as Leave: the adventure ends instead. The language menu stays a single read with English as the fallback. See the parent spec at `.scratch/console-prompting/spec.md`.

**Blocked by:** 02 (The game is given its input, output and Die)

**Status:** resolved

- [ ] The main menu, the Path prompt, the Merchant and the Shrine ask through the Prompt module, with the same prompts, Commands and error messages as before
- [ ] Input running out at the main menu, the Path prompt, the Merchant or the Shrine prints the "adventure ends" message and the game returns
- [ ] Only the game's start method handles end of input
- [ ] The Inventory Command still shows the Inventory and then the Path prompt again
- [ ] Invalid input at the Merchant and Shrine still shows their error and asks again
- [ ] The language menu still reads once: 2, 3 or 4 picks Spanish, French or Italian; anything else, including end of input, picks English
- [ ] Game-entry-point tests cover end of input at each of these prompts, in English and at least one other Language
- [ ] The README no longer lists "the game loops forever if input runs out at the path prompt"
- [ ] All existing tests pass

## Comments

**2026-11-16:** Implemented in c3a9623.

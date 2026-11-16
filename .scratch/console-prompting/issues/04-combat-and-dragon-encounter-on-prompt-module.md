# 04: Combat and Dragon encounter on the Prompt module: bad combat input asks again

**What to build:** The Dragon-encounter choice and the combat prompt ask through the Prompt module. Invalid input at the combat prompt shows the error and asks again without attacking; `a`/`attack` continues the fight and `r`/`retreat` retreats, as before. Input running out at the Dragon encounter or during combat ends the adventure with the "adventure ends" message. See the parent spec at `.scratch/console-prompting/spec.md`.

**Blocked by:** 03 (Prompt module: running out of input ends the adventure at the menus, the Path, the Merchant and the Shrine)

**Status:** ready-for-agent

- [ ] Invalid input at the combat prompt shows the error and asks again; the Dragon takes no damage and the Player makes no attack because of it
- [ ] `a`/`attack` and `r`/`retreat` at the combat prompt, and the Dragon-encounter choices, behave as before
- [ ] Input running out at the Dragon encounter or the combat prompt prints the "adventure ends" message and the game returns
- [ ] Scripted North Path tests (with a scripted Die) cover invalid combat input followed by an attack, and end of input during combat
- [ ] The README no longer lists "invalid input at the combat prompt counts as an attack"
- [ ] All existing tests pass

## Comments

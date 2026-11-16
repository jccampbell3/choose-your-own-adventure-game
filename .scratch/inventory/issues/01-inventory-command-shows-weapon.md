# 01: The Inventory Command shows the Player's Weapon

**What to build:** At the crossroads, the Player can type `i` or `inventory` to see their Inventory. It shows a heading, their Weapon's translated name and its maximum damage, and then the path prompt again. Looking changes nothing. The lines come from a new stateless Inventory module with no console input or output, which reads `Player.Weapon` (nothing new is stored on the Player). The crossroads text mentions `i` and the invalid-path message lists it, in every Language, without accented letters. See the parent spec at `.scratch/inventory/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [ ] `i` and `inventory` are accepted at the crossroads by both the path prompt and `IsValidPathChoice`; `n`, `s`, `w`, `e` behave as before
- [ ] Typing `i` prints the Inventory heading, the Weapon name and its maximum damage, then shows the path prompt again
- [ ] A Magical Weapon shows its magical name and raised damage
- [ ] Looking at the Inventory leaves the Player unchanged
- [ ] `path_prompt_full` mentions `'i'` and `path_invalid` lists `n`, `s`, `w`, `i`, `e`, in English, Spanish, French and Italian
- [ ] Inventory-module tests cover a plain Weapon, a Magical Weapon, and every Occupation's Weapon, in English and one other Language
- [ ] A game-entry-point test (redirected console) covers: create a character, `i`, `e`
- [ ] The existing Messages tests check that the path prompt and path-invalid text mention `'i'` in every Language; key-parity and ASCII-only tests still pass
- [ ] All existing tests pass

## Comments

**2026-11-16:** Implemented in 73494d9.

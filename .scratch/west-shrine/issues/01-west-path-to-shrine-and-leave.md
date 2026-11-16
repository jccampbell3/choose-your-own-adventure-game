# 01: West Path leads to the Shrine, and the Player can leave

**What to build:** A Player at the crossroads can type `w` or `west` to take the West Path. They read the West Path narrative and meet the Shrine, which warns that the gods may bless or curse them (no numbers). The Shrine offers `1` Pray or `2` Leave. Invalid input shows an error and asks again, and end of input counts as Leave. Leave returns the Player to the crossroads. In this ticket Pray is accepted as a choice but has no effect yet; it is completed in 02. The rules live in a new Shrine module that has no console input or output and keeps no state, like the Merchant. It provides a "will offer" check, choice parsing, and the Left outcome. All new and changed text is in every Language, without accented letters; Commands are not translated. See the parent spec at `.scratch/west-shrine/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [ ] `w` and `west` are accepted at the crossroads by both the path prompt and path-choice validation; `n`, `s`, `e` behave as before
- [ ] The crossroads text describes three paths and lists `w`, and the invalid-path message lists `n`, `s`, `w`, `e`, in English, Spanish, French and Italian
- [ ] Going west shows the West Path narrative, then the Shrine greeting with the bless-or-curse warning
- [ ] Shrine choice parsing accepts `1` (Pray) and `2` (Leave), with surrounding whitespace allowed, and rejects everything else
- [ ] Invalid input at the Shrine prints the Shrine invalid-choice message and asks again; end of input counts as Leave
- [ ] Leave prints the leave message, leaves the Player unchanged, and returns to the crossroads
- [ ] Shrine-module tests cover parsing and the Left outcome
- [ ] A game-entry-point test (redirected console) covers: create a character, go west, enter invalid input, leave, exit
- [ ] The existing Messages tests check that the path prompt and path-invalid text mention `'w'` in every Language; key-parity and ASCII-only tests still pass
- [ ] All existing tests pass

## Comments

**2026-11-16:** Implemented in 1050de1.

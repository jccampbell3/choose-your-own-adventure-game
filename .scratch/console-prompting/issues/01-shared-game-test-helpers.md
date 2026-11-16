# 01: Shared game-test helpers

**What to build:** One shared test-support file that every game-level and character-creation test uses, instead of each test file carrying its own copy. It holds: run a scripted game and return what it printed, count occurrences of a text, the English-Fighter creation script, load Messages for a Language, and the scripted Die (moved out of the Dragon Armor tests, where it lives today). The South Path, West Path, Inventory and starting-health tests switch to it. This is a test-only prefactor: no game code changes. See the parent spec at `.scratch/console-prompting/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [ ] One shared test-support file provides: run a scripted game, count occurrences, the English-Fighter script, load Messages for a Language, and the scripted Die
- [ ] The scripted Die no longer lives inside the Dragon Armor tests; every test that used it still compiles and passes
- [ ] The South Path, West Path and Inventory game tests use the shared helpers and no longer define their own run-game, count or English-Fighter copies
- [ ] The starting-health tests and other tests that load Messages use the shared loader where it fits
- [ ] No game code changes
- [ ] All existing tests pass, with the same test count

## Comments

**2026-11-16:** Implemented in 5591014.

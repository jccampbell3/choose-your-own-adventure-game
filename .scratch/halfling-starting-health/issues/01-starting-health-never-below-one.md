# 01: Starting HealthPoints are never below 1

**What to build:** A new Player always starts the adventure with at least 1 HealthPoint. Starting HealthPoints stay a d20 roll plus the Strength modifier, but a total below 1 becomes 1. This is the same floor the Shrine's Curse uses. Only a Halfling can reach the floor, because only a Halfling has a negative Strength modifier, but the rule applies to every Race. The stats screen shows the corrected value.

To make this testable, the Player's character creation must accept a Die supplied from outside, the way the Dragon and Shrine already do. The game still supplies an ordinary Die, so play is unchanged apart from the fix. Tests can then script the Strength, Agility and health rolls through character creation (with a redirected console for the prompts) instead of relying on random rolls. See the parent spec at `.scratch/halfling-starting-health/spec.md`.

**Blocked by:** None (can start immediately)

**Status:** resolved

- [ ] Character creation can be given a scripted Die; with no Die supplied, the Player rolls with an ordinary Die as before
- [ ] A Halfling whose health roll is 1 starts with exactly 1 HealthPoint for every Strength modifier from 1 to 4
- [ ] A Halfling whose health roll plus modifier is already 1 or more keeps that value (e.g. health roll 5, modifier 2 gives 3)
- [ ] Other Races are unchanged: health roll 1 with modifier 4 gives 5 for a Human
- [ ] Strength and Agility are unaffected by the fix
- [ ] The stats screen shows the corrected starting HealthPoints
- [ ] Tests cover the above at the character-creation seam with a scripted Die; no test depends on a random roll
- [ ] All existing tests pass

## Comments

**2026-11-16:** Implemented in b666bb9.

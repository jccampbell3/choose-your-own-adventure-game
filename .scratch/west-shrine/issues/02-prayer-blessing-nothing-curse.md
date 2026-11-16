# 02: Praying rolls the die (Blessing, Nothing or Curse)

**What to build:** Choosing Pray at the Shrine rolls a d6 and shows the roll. On 1–2 the Player is Cursed: HealthPoints drop by 3 but never below 1, and a Curse never raises HealthPoints (a Player already at 1 or below is unchanged). On 3–4 nothing happens. On 5–6 the Player is Blessed: HealthPoints rise by 3, with no upper limit. Each Prayer prints the roll line, then the outcome text, then the Player's HealthPoints, and returns the Player to the crossroads. Magical Armor does not reduce a Curse. The Shrine operation takes a Player, a choice and a Die, and returns the outcome together with the roll. All new text is in every Language. See the parent spec at `.scratch/west-shrine/spec.md`.

**Blocked by:** 01 (West Path leads to the Shrine, and the Player can leave)

**Status:** resolved

- [ ] With a scripted Die: rolls 1 and 2 give Curse (−3), rolls 3 and 4 give Nothing (unchanged), rolls 5 and 6 give Blessing (+3)
- [ ] Curse edge cases: HealthPoints 4 → 1, 3 → 1, 2 → 1, 1 → 1, 0 → 0, −2 → −2
- [ ] A Player with Magical Armor loses the full 3 to a Curse
- [ ] The outcome reports the roll the Die returned
- [ ] The console prints the roll line, the outcome text, and the Player's HealthPoints, in every Language
- [ ] A game-entry-point test going west and praying sees a roll line and one of the three outcome texts (the game's own Die is not scripted)
- [ ] Key-parity and ASCII-only tests pass with the new keys; all existing tests pass

## Comments

**2026-11-16:** Implemented in 1050de1.

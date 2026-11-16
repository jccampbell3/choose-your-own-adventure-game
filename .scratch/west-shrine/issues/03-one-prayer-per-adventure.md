# 03: One Prayer per adventure (the Shrine goes Silent)

**What to build:** A Player gets one Prayer per adventure. Every Pray outcome (Blessing, Nothing or Curse) records on the Player that they have prayed; the Shrine itself keeps no state. On later trips west, the Shrine shows the Silent message instead of the offer, and the Player is returned to the crossroads unchanged. Leaving without praying does not use up the Prayer. Whether the Player has prayed is not shown in the character stats. All new text is in every Language. See the parent spec at `.scratch/west-shrine/spec.md`.

**Blocked by:** 02 (Praying rolls the die: Blessing, Nothing or Curse)

**Status:** resolved

- [ ] The Player records whether they have prayed, starting as false; every Pray outcome sets it
- [ ] After a Prayer, both Pray and Leave return Silent and leave the Player unchanged
- [ ] Leave keeps the Prayer available, and the next visit offers the Prayer again
- [ ] "Will offer" is false once the Player has prayed, and the console prints the Silent message without showing the offer
- [ ] The character stats are unchanged (nothing about praying is shown)
- [ ] The end-to-end game-entry-point test covers: create a character, west, invalid input, leave, west (offer shown again), pray, west (Silent), exit
- [ ] Key-parity and ASCII-only tests pass with the new keys; all existing tests pass

## Comments

**2026-11-16:** Implemented in 1050de1.

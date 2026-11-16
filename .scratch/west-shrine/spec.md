# Spec: West Path Shrine

Status: resolved

## Problem Statement

The crossroads has two Paths. North leads to the Dragon and ends the game. South leads to the Merchant, a safe and predictable way to prepare. There is no choice that involves risk before the fight: everything the Player can do ahead of the Dragon either helps or does nothing.

## Solution

A third Path, West, leads to a Shrine. The Shrine warns the Player that the gods may bless or curse them, and offers two choices: Pray or Leave. A Prayer rolls a d6 and shows the roll. On 1–2 the Player is Cursed and loses 3 HealthPoints, never going below 1. On 3–4 nothing happens. On 5–6 the Player is Blessed and gains 3 HealthPoints. The Player can make one Prayer per adventure; later visits find the Shrine silent. Leaving without praying does not use up the Prayer. After the Shrine, the Player always returns to the crossroads. All new text is available in every Language.

## User Stories

1. As a Player, I want a third Path at the crossroads, so that I have a risky option alongside the Dragon and the Merchant.
2. As a Player, I want to take the West Path by typing `w` or `west`, so that it works like `n` and `s`.
3. As a Player, I want the crossroads text to describe three paths and list `w`, so that I know West exists.
4. As a Player, I want the invalid-path message to list `w`, so that the hint matches the valid Commands.
5. As a Player, I want a West Path narrative that leads me to a Shrine, so that the Path has its own place in the story.
6. As a Player, I want the Shrine to warn me that the gods may bless or curse me, so that choosing to pray is an informed gamble.
7. As a Player, I want the warning to carry no numbers, so that the Shrine keeps some mystery.
8. As a Player, I want to choose `1` to Pray or `2` to Leave, so that the Shrine menu works like the Merchant menu.
9. As a Player, I want invalid input at the Shrine to show an error and ask again, so that a typo doesn't count as a Prayer.
10. As a Player, I want to see the number the die shows when I pray, so that I can tell the outcome was luck.
11. As a Player who rolls 1 or 2, I want to be Cursed and lose 3 HealthPoints, so that praying carries real risk.
12. As a Player who rolls 3 or 4, I want to be told that nothing happens, so that I know the Prayer was used without effect.
13. As a Player who rolls 5 or 6, I want to be Blessed and gain 3 HealthPoints, so that praying can pay off.
14. As a Player, I want my HealthPoints shown after every Prayer, so that I always know where I stand.
15. As a Cursed Player, I want a Curse never to take me below 1 HealthPoint, so that only the Dragon can kill me.
16. As a Player already at 1 HealthPoint or below, I want a Curse to leave my HealthPoints unchanged, so that a Curse never heals me.
17. As a Player with Magical Armor, I want my Armor not to change a Curse, so that Armor stays a protection against the Dragon only.
18. As a Player who left without praying, I want the Shrine to offer the Prayer again on my next trip west, so that leaving does not lock me out.
19. As a Player who has prayed, I want later trips west to show that the Shrine is silent, so that I understand why no Prayer is offered.
20. As a Player who has prayed, I want a repeat trip west to leave my character unchanged, so that I cannot farm Blessings.
21. As a Player, I want to return to the crossroads after the Shrine, so that I can still visit the Merchant or face the Dragon.
22. As a Player, I want a Blessing to carry into the Dragon fight, so that the combat stats show my new HealthPoints.
23. As a Player who never goes west, I want the game to behave exactly as it does today, so that the Shrine stays optional.
24. As a Spanish, French, or Italian Player, I want every Shrine message in my Language, so that the game never falls back to English or shows a `[missing_key]` placeholder.
25. As a Player in any Language, I want Commands to stay the same letters and numbers, so that the controls never change between Languages.
26. As a Player on a Windows console, I want the new text to avoid accented letters, so that it displays without garbled characters.
27. As a maintainer, I want the Shrine's rules to be callable without the console and with a supplied Die, so that every outcome can be unit-tested with fixed rolls.
28. As a maintainer, I want whether the Player has prayed stored on the Player, so that the Shrine keeps no state of its own, like the Merchant.
29. As a grader or reviewer, I want an end-to-end test of the west loop, so that I can see the whole feature working through the real game entry point.

## Implementation Decisions

- **Shrine (new module).** It holds the Shrine's rules and has no console input or output. It is a stateless module like the Merchant. Its core operation takes a Player, a choice (Pray or Leave) and a Die, and returns an outcome: **Blessing**, **Nothing**, **Curse**, **Left**, or **Silent**. The outcome also carries the die roll, so the console can show it. It returns Silent when the Player has already prayed, whatever the choice, and leaves the Player unchanged. On Pray it rolls a d6: 1–2 Curse, 3–4 Nothing, 5–6 Blessing. It marks the Player as having prayed on every Pray outcome, including Nothing. Leave changes nothing. It also provides a "will offer" check (has the Player not yet prayed?) and choice parsing (`1` = Pray, `2` = Leave, anything else is invalid), mirroring `Merchant.WillOffer` and `Merchant.TryParseChoice`.
- **Prayer amounts.** A Blessing adds **3** HealthPoints, with no upper limit (the game has no maximum health). A Curse sets HealthPoints to `max(1, hp − 3)` when hp > 1. When hp ≤ 1, HealthPoints are left unchanged.
- **Player (modified).** It gains a **HasPrayed** flag, false by default. It is not shown in the character stats.
- **Game (modified).** The path prompt accepts `w` / `west`, and `IsValidPathChoice` accepts them too. The West branch shows the West Path narrative and runs a thin console loop that mirrors `HandleMerchantEncounter`. If the Shrine will not offer, it prints the silent message and returns. Otherwise it prints the greeting and warning, reads `1`/`2`, and asks again on invalid input. End of input counts as Leave. It then calls the Shrine and prints the outcome: for Blessing, Nothing and Curse, the roll line, then the outcome text, then the Player's HealthPoints; for Left, the leave message. Afterwards it returns to the path prompt.
- **Language data (modified).** These are added to every Language (English, Spanish, French, Italian):
  - new messages: West Path narrative, Shrine greeting and warning, offer menu, invalid choice, roll line (formatted with the number), Blessing, Nothing, Curse, leave, and silent;
  - updated `path_prompt_full` (three paths, lists `w`) and `path_invalid` (lists `n`, `s`, `w`, `e`).

  No accented letters are used. Commands are not translated.
- **Not changed:** Dragon, Combat, Armor, Weapon, Merchant. A Curse is not a Dragon hit, so Magical Armor does not apply.

## Testing Decisions

- **What makes a good test here:** the same rule as the Merchant spec. Test observable behavior through public interfaces (outcomes, HealthPoints, printed text), use a scripted Die for every roll, and never assert on randomness.
- **Seams (agreed: two):**
  1. **Shrine operation with the existing scripted Die (new seam, same shape as the Merchant's).** Cases: rolls 1 and 2 → Curse, HealthPoints −3; rolls 3 and 4 → Nothing, unchanged; rolls 5 and 6 → Blessing, +3. Curse at 4 HP → 1; at 3 HP → 1; at 2 HP → 1; at 1 HP → 1; at 0 HP → 0; at −2 HP → −2. Every Pray outcome sets HasPrayed. Leave leaves the Player unchanged and HasPrayed false. After a Prayer, both Pray and Leave return Silent and leave the Player unchanged. A Player with Armor is Cursed by the full amount. The returned roll matches the Die. Choice parsing is tested here too: `1` and `2` parse, with surrounding whitespace allowed; anything else does not.
  2. **Game entry point with redirected console (existing seam).** Scripted input: create a character, go west, enter invalid input (see the invalid message), leave, go west again (the offer is shown again), pray, return to the crossroads, go west again (see the silent message), exit. The game's own Die stays unscripted, so assert only that a roll line and one of the three outcome texts appear, not which one.
- **Not a new seam:** the existing all-Language key-parity and ASCII-only tests cover the new text automatically. A check that `path_prompt_full` and `path_invalid` mention `'w'` in every Language goes into the existing Messages tests.
- **Deliberately not done:** making the game's own Die injectable. It would let seam 2 check exact outcomes, but it changes how `Game` is set up only for the sake of tests.
- **Prior art:** `MerchantTests` for the stateless-module tests; `SouthPathGameTests` for the redirected-console loop; the scripted Die subclass from the Merchant work.

## Out of Scope

- The Shrine ending the game, or the Player dying at the Shrine.
- More than one Prayer per adventure, or rising risk on repeat Prayers.
- Prayers affecting Strength, Agility, or Magical Items.
- A maximum-health concept.
- Fixing a Halfling's 0-or-below starting health. It is tracked separately as the "halfling-starting-health" bug.
- An East Path (`e` is the exit Command).
- Translated Commands or accented letters.

## Further Notes

- Domain terms follow `GLOSSARY.md`: Path, Shrine, Prayer, Blessing, Curse, Command, Language, Merchant, Magical Armor.
- Balance target: the expected gain from a Prayer is zero (one-third chance each of −3, 0, +3), so praying is a true gamble, and it is about the same size as the Merchant's ±2 items.
- No ADR: none of these decisions are hard to reverse.

## Comments

**2026-11-16 (implemented):** Done on `main` in 1050de1. Both agreed seams are tested; 191 tests pass. The two-axis review found no documented-standard breaches and no missing requirements. Open judgement calls, not blocking:

- End of input at the Shrine counts as Leave, but no game-level test covers it. After leaving, the existing `PromptForPath` loops forever on end of input (the Merchant path has the same problem), so the scripted game can't end cleanly.
- `HandleShrineEncounter` repeats the read-until-valid loop from `HandleMerchantEncounter`. `WestPathGameTests` also copies the `RunGame` and `CountOccurrences` helpers from `SouthPathGameTests`. Both could move into shared helpers.
- Path parsing is now three parallel if-cascades on string path names (`IsValidPathChoice`, `PromptForPath`, the dispatch in `StartGame`). One Path-to-Command map would keep them together.
- Not in the spec: a game-level test checks the Shrine text in Spanish, French and Italian. The old `IsValidPathChoice` invalid-input test used `"west"`; it now uses `"up"`.

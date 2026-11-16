# Spec: South Path Merchant

Status: resolved

## Problem Statement

The South Path is a dead end. A Player who goes south reads that "nothing adventurous happens" and is sent back to the path prompt with nothing gained. The only real choice in the game is whether to fight the Dragon, and nothing the Player does beforehand affects that fight. The South Path text also says the Player "returns home", but the game actually puts them back at the crossroads, so the story and the behavior disagree.

## Solution

Taking the South Path leads the Player to a Merchant who offers one Magical Item for free: Magical Armor (less damage taken from the Dragon) or a Magical Weapon (more damage dealt to the Dragon). The Player can also leave without choosing. Afterward the Player returns to the path prompt and can go north to face the Dragon with the item. The Merchant is there on every trip south. It keeps offering the choice until the Player owns a Magical Item, and after that it tells the Player they cannot get anything else. Both items are deliberately weak, so they help without deciding the fight. All new text is available in every Language.

## User Stories

1. As a Player, I want the South Path to lead somewhere meaningful, so that choosing south is a real decision and not a dead end.
2. As a Player, I want to meet a Merchant when I go south, so that the South Path has its own encounter.
3. As a Player, I want the Merchant to offer me Magical Armor or a Magical Weapon, so that I can prepare for the Dragon in the way I prefer.
4. As a Player, I want to choose by typing `1`, `2`, or `3`, so that the Merchant menu works like the other numbered menus in the game.
5. As a Player, I want a Leave option at the Merchant, so that I am not forced to take an item.
6. As a Player, I want to receive the item for free, so that I don't need money the game never gives me.
7. As a Player, I want to return to the path prompt after the Merchant, so that I can then go north and use my item against the Dragon.
8. As a Player who left without choosing, I want the Merchant to offer the choice again on my next trip south, so that leaving does not lock me out.
9. As a Player who already has a Magical Item, I want the Merchant to tell me I cannot get anything else, so that I understand why no offer is made.
10. As a Player who already has a Magical Item, I want a repeat trip south to leave my character unchanged, so that I cannot stack items.
11. As a Player, I want invalid input at the Merchant to show an error and ask again, so that a typo doesn't count as a choice.
12. As a Player who chose Magical Armor, I want every Dragon hit to do 2 less damage, so that I survive longer.
13. As a Player with Magical Armor, I want a hit's damage never to go below zero, so that armor never heals me.
14. As a Player with Magical Armor, I want to see a message saying how much damage my armor absorbed, so that I can tell it is working.
15. As a Player with Magical Armor, I want a different message when my armor absorbs the whole hit, so that a zero-damage hit makes sense in the story.
16. As a Player with Magical Armor, I still want to see my health after a hit the armor reduced, so that I always know where I stand.
17. As a Player who chose the Magical Weapon, I want my own Weapon to become its magical version, so that my Occupation's character is kept (a Fighter still has a sword).
18. As a Player with a Magical Weapon, I want its maximum damage raised by 2, so that I hit the Dragon a little harder.
19. As a Player with a Magical Weapon, I want its name to show as magical (e.g. "magical long sword") in my Language with correct grammar, so that the text reads naturally.
20. As a Player, I want my character stats to show my Armor and its damage reduction when I own Armor, so that I can see what I chose.
21. As a Player without Armor, I want my character stats to look exactly as they do now, so that nothing irrelevant is shown.
22. As a Player, I want the combat stats shown before the fight to include my Armor when I own it, so that I know what I'm bringing to the fight.
23. As a Player, I want the combat stats to show my Magical Weapon's name and boosted maximum damage, so that the upgrade is visible.
24. As a Spanish, French, or Italian Player, I want every Merchant message, item name, and armor message in my Language, so that the game never falls back to English or shows a `[missing_key]` placeholder.
25. As a Player in any Language, I want Commands to stay the same numbers and letters, so that the controls never change between Languages.
26. As a Player on a Windows console, I want the new text to avoid accented letters, so that it displays without garbled characters.
27. As a Player, I want the South Path narrative to describe meeting the Merchant instead of returning home bored, so that the story matches what happens.
28. As a Player who goes north without visiting the Merchant, I want the Dragon fight to work exactly as it does today, so that the item stays optional.
29. As a maintainer, I want Magical Armor's damage reduction applied where the Dragon deals damage, so that there is one place where the rule lives.
30. As a maintainer, I want whether a Weapon is magical stored as a property of the Weapon, not guessed from its name, so that translations or renames can't break the Merchant's refusal rule.
31. As a maintainer, I want the Merchant's rules to be callable without the console, so that every rule can be unit-tested.
32. As a maintainer, I want a test that fails if any Language is missing a key English has, so that new Merchant text can't be half-translated.
33. As a maintainer, I want a test that fails if any magical weapon name is untranslated in any Language, so that Canonical Names never reach the screen.
34. As a grader or reviewer, I want an end-to-end test of the south loop, so that I can see the whole feature working through the real game entry point.

## Implementation Decisions

- **Merchant (new module).** It holds the Merchant's rules and has no console input or output. Its core operation takes a Player and a choice and returns one of four outcomes: **Armor**, **Weapon**, **Left**, or **Refused**. When the outcome is Armor or Weapon, the Player is updated. The Merchant keeps no state of its own. It refuses when the Player already owns a Magical Item, meaning the Player has Armor or their Weapon is magical. Invalid input is not a Merchant outcome; the console loop handles it.
- **Armor (new module).** A small value type representing Magical Armor with a damage reduction of **2**. It provides the reduction rule: reduced damage = max(0, damage − reduction). It has a Canonical Name, `magical armor`, which is shown through a translated message.
- **Weapon (modified).** It gains an **IsMagical** property, false by default. Enchanting a Weapon produces its magical version: the Canonical Name becomes `magical <canonical name>`, MaxDamage goes up by **2**, the ASCII art stays the same, and IsMagical becomes true. Resulting maximums: dagger 8, long bow 10, long sword 14, lightning bolt spell 14.
- **Player (modified).** It gains an optional **Armor**, empty by default. The character stats show an Armor line and a Damage Reduction line only when Armor is present. Player's constructor and dice are unchanged.
- **Dragon (modified).** In its attack, right after the damage roll, the Player's Armor (if any) reduces the damage. If damage was reduced, it prints an "armor absorbs N damage" message. If the damage became zero, it prints an "armor absorbs the blow" message instead of the damage-dealt line. The Player's health line is always printed after a hit. The existing rule that health never goes below 0 is kept.
- **Die (modified).** Its roll operation becomes overridable so tests can supply fixed rolls. Behavior in the game is unchanged.
- **Combat (modified).** The combat stats display includes the Player's Armor lines when Armor is present.
- **Game (modified).** The South Path branch shows the new Merchant narrative and runs a thin console loop: show the offer or the refusal, read a numbered choice, ask again on invalid input, call the Merchant, print the result message, and return to the path prompt. The South Path no longer prints the "returned home bored" text.
- **Language data (modified).** These are added to every Language (English, Spanish, French, Italian):
  - new dictionary messages: the South Path / Merchant greeting, the offer menu, the confirmations for receiving each item, the leave message, the refusal message, the invalid-choice message, the armor-absorbs-damage and armor-absorbs-blow messages, the Armor name, and the stats labels for Armor and Damage Reduction;
  - the replacement South Path narrative;
  - four `displayWeaponMap` entries, one for each magical weapon Canonical Name.

  No accented letters are used. Commands are not translated.
- **Magical weapon names (see ADR 0002).** Each Magical Weapon has its own Canonical Name with an explicit translation in each Language, rather than a "magical {0}" pattern. This keeps adjectives matching the noun's gender in Spanish, Italian, and French.
- **Prerequisite.** The `Messages` fix on `fix/messages-null-maps-and-language-reload` (which sets up the display maps in the constructor and reloads on language change) should be merged into this branch before implementation, because the new display text depends on those maps.

## Testing Decisions

- **What makes a good test here:** test behavior the Player can observe through a module's public interface: outcomes, stats, damage and printed text. Don't test private fields or the order of internal calls. Use fixed dice wherever a result depends on a roll, and don't assert on randomness.
- **Seams (agreed):**
  1. **Dragon attack with a scripted Die.** A test Die subclass returns preset rolls (attack hits, dodge fails, a chosen damage value). Cases: a Player with Armor taking damage of 1, 2, 3 and 12 loses 0, 0, 1 and 10 Health Points; the absorb messages appear correctly; Health Points never go below 0; a Player without Armor takes full damage as before.
  2. **Merchant operation.** Cases: choosing Armor gives the Player Armor with reduction 2; choosing the Weapon gives the magical Canonical Name, MaxDamage +2 and IsMagical, for each of the four Occupations; Leave leaves the Player unchanged; after Leave, the next offer is accepted; after receiving either item, any further choice returns Refused and leaves the Player unchanged.
  3. **Messages.** Every Language has every key English has, extending the existing Italian-only parity test to all Languages. Each magical weapon Canonical Name translates to something other than itself in every non-English Language. The Armor name and stats labels exist in every Language.
  4. **Game entry point with redirected console.** Scripted standard input drives: create a character, go south, choose an item, return to the path prompt, go south again, see the refusal, exit. The captured output must contain the Merchant greeting, the item confirmation and the refusal, and must not contain the old "returned home bored" text. A second script checks that invalid input at the Merchant prints the invalid message and asks again.
- **Prior art:**
  - `CombatTests` builds Players and Dragons with object initializers and fixed stats; reuse that style.
  - `DragonTests` and `GameTests` load `Messages` per Language with a small helper.
  - `MessagesTests` contains the existing Italian key-parity test to generalize.
- **Not tested on their own:** the Armor reduction rule is covered through the Dragon attack seam rather than a separate unit test, and Player dice stay non-injectable.

## Out of Scope

- Gold, prices, or any currency or inventory system.
- Owning both items, or more than one of either.
- Removing, swapping, or selling back a Magical Item.
- Translated Command letters, or restoring accented letters (UTF-8 console output).
- Armor affecting anything other than Dragon hits on the Player.
- Changes to the Dragon's stats or weapon.
- Existing known issues: invalid combat input counting as an attack, Halfling starting health at or below 0, missing error handling in `ReadDictionary`, and the hard-coded language menu.
- A native-speaker review of the new translations.

## Further Notes

- Domain terms follow `GLOSSARY.md`: Merchant, Magical Item, Magical Armor, Magical Weapon, Canonical Name, Command, Language, Path.
- ADR 0002 records the per-weapon Canonical Name decision and why the Merchant gives the item away for free.
- Balance target: both items should be noticeable but never decisive. Armor blocks about 30% of average claw damage. The Weapon adds about 1 average damage per hit.

## Comments

**2026-11-16 (implemented):** Done on `feature/south-merchant` in e54bdcd and 776c067, after merging the `Messages` prerequisite (e3844e4). All four agreed seams are tested; 147 tests pass. The two-axis review found no documented-standard breaches and no missing requirements. Open judgement calls, not blocking:

- Choosing the Weapon when the Player has no Weapon reports the outcome as Left. This can't happen in play, because every Occupation starts with a Weapon.
- `Armor.Type` (`magical armor`) is used only by tests. The display uses the `armor_name` text, so the two could drift apart.
- The Armor stats lines are written out separately in `Player.DisplayStats` and `Combat.GetCombatStatsDisplay`, as the Weapon lines already were.
- Ask a native speaker to check the Spanish and Italian lightning-spell names (`hechizo de rayo magico`, `incantesimo del fulmine magico`). They can read as "spell of the magic lightning".
- Additions not in the spec: end of input at the Merchant counts as Leave, and a test keeps `language_data.json` ASCII-only.

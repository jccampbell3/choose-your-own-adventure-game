# Spec: Inventory display Command

Status: resolved

## Problem Statement

At the crossroads the Player decides whether to visit the Merchant, pray at the Shrine, or face the Dragon, but they cannot see what they are carrying. The stats screen shows the Weapon and Magical Armor, but it is only offered once, right after character creation. A Player who has visited the Merchant has no way to check what they received before choosing a Path.

## Solution

Add an **Inventory** Command, `i` / `inventory`, at the crossroads path prompt. It prints the Player's Inventory: their Weapon with its maximum damage and, if they own it, their Magical Armor with its damage reduction. Then it shows the path prompt again. Looking at the Inventory changes nothing and can be repeated.

## User Stories

1. As a Player at the crossroads, I want to type `i` or `inventory` to see what I carry, so that I can plan my next Path.
2. As a Player, I want the crossroads text to tell me about `i`, so that I can find the Command.
3. As a Player who types an invalid Path, I want the error to list `i`, so that the list of valid Commands is complete.
4. As a Player, I want my Weapon's name and maximum damage shown, so that I know how hard I hit.
5. As a Player with a Magical Weapon, I want its magical name shown (e.g. "magical long sword"), so that I can see the Merchant's enchantment.
6. As a Player with Magical Armor, I want it and its damage reduction shown, so that I know how much it protects me.
7. As a Player without Magical Armor, I want no Armor line, so that the Inventory matches the stats screen and does not hint at the Merchant.
8. As a Player, I want to return to the path prompt after looking, so that looking costs nothing.
9. As a Player, I want looking at the Inventory to leave my character unchanged, so that I can look as often as I like.
10. As a Player in any Language, I want the Inventory in my Language, with no `[missing_key]` placeholder.
11. As a Player in any Language, I want the Command to stay `i`, so that the controls never change between Languages.
12. As a Player on a Windows console, I want the new text to avoid accented letters.
13. As a maintainer, I want the Inventory built without the console, so that its contents can be unit-tested.

## Implementation Decisions

- **Inventory (new module).** A stateless module with no console input or output, like the Merchant and Shrine. It reads the Player's existing `Weapon` and `Armor` and produces the Inventory lines using `Messages`: a heading, the Weapon line and its maximum-damage line, and, only when `Armor` is not null, the Armor line and its damage-reduction line. The Weapon name goes through `TranslateWeaponForDisplay`; the Armor name is the `armor_name` message. The lines reuse the stats labels `stats_weapon`, `stats_damage`, `stats_armor` and `stats_damage_reduction`.
- **No new storage.** The Inventory is a view of `Player.Weapon` and `Player.Armor`. Nothing is added to `Player`, and the Merchant, Dragon, Combat and stats screen are untouched.
- **Game (modified).** `PromptForPath` accepts `i` / `inventory`. It prints the Inventory and loops back to the path prompt without returning a Path. `IsValidPathChoice` also accepts `i` / `inventory`.
- **Language data (modified).** In every Language (English, Spanish, French, Italian): a new `inventory_header` message; `path_prompt_full` gains "Enter 'i' for Inventory" (translated wording, untranslated Command); `path_invalid` lists `n`, `s`, `w`, `i`, `e`. No accented letters.
- **Not changed:** the stats screen, the Merchant, the Shrine, the Dragon, Combat, and the Dragon-encounter and combat prompts. The Inventory is available at the crossroads only.

## Testing Decisions

- **What makes a good test here:** test observable behavior through public interfaces (the lines produced, printed text), as in the Merchant and Shrine specs.
- **Seams (agreed: two):**
  1. **Inventory module, without the console.** Cases: a plain Weapon only (no Armor lines); a Magical Weapon (magical name, raised damage); a Weapon plus Magical Armor (Armor name and reduction 2); each Occupation's Weapon. Check English and at least one other Language.
  2. **Game entry point with redirected console.** Script: create a character, `i` (see the Weapon, no Armor line, the path prompt again), `s` and take the Armor, `inventory` (Armor line appears), `e`. Check the Player is returned to the path prompt after each look.
- **Not a new seam:** key-parity and ASCII-only tests cover the new text. A check that `path_prompt_full` and `path_invalid` mention `'i'` in every Language goes into the existing Messages tests.
- **Prior art:** `MerchantTests` / `ShrineTests` for the stateless module; `WestPathGameTests` / `SouthPathGameTests` for the redirected console. Game-level scripts must end with `e`, because `PromptForPath` loops forever on end of input.

## Out of Scope

- An Inventory Command at the Dragon-encounter prompt or during combat.
- Showing Prayer or Merchant progress in the Inventory.
- Changing the stats screen.
- Changing how the Weapon and Armor are stored on the Player, or a general item list.
- Using, dropping or swapping items.

## Further Notes

- Domain terms follow `GLOSSARY.md`: Inventory, Command, Path, Language, Magical Weapon, Magical Armor, Merchant.
- No ADR: none of these decisions are hard to reverse. If more item types arrive later, moving to an Inventory object on the Player is the expected next step.

## Comments

**2026-11-16 (implemented):** Done on `main` in 73494d9. Both agreed seams are tested; 213 tests pass. The two-axis review found no documented-standard breaches and no missing requirements. Open judgement calls, not blocking:

- The Weapon/Armor lines in `Inventory.cs` repeat `Combat.GetCombatStatsDisplay` and `Player.DisplayStats` (now three copies). Left alone because the spec keeps the stats screen and Combat unchanged; a shared helper would remove it.
- Path Commands are still checked in parallel if-cascades (`IsValidPathChoice`, `PromptForPath`); `i` / `inventory` was added to both.
- `InventoryGameTests` copies the `RunGame`, `CountOccurrences` and character-script helpers from `WestPathGameTests`.
- Test coverage is thin in two places: the Messages test checks only that `path_invalid` mentions `'i'`, not the full `n`, `s`, `w`, `i`, `e` list; every Occupation's Weapon is tested only in English.
- Not in the spec: a game-level test checks the Inventory in Spanish and Italian, and `Inventory.Lines` skips the Weapon lines if the Player has no Weapon.

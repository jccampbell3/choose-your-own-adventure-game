# 02: The Inventory shows Magical Armor

**What to build:** When the Player owns Magical Armor, the Inventory also shows the Armor's translated name and its damage reduction, using the existing stats labels. A Player without Magical Armor sees no Armor line, matching the stats screen. See the parent spec at `.scratch/inventory/spec.md`.

**Blocked by:** 01 (The Inventory Command shows the Player's Weapon)

**Status:** resolved

- [ ] With Magical Armor, the Inventory shows the Armor name and damage reduction 2
- [ ] Without Magical Armor, no Armor line appears
- [ ] Inventory-module tests cover a Weapon plus Magical Armor in English and one other Language
- [ ] The end-to-end game-entry-point test covers: create a character, `i` (no Armor line), `s` and take the Armor, `inventory` (Armor line appears), `e`, with the path prompt shown again after each look
- [ ] Key-parity and ASCII-only tests pass; all existing tests pass

## Comments

**2026-11-16:** Implemented in 73494d9.

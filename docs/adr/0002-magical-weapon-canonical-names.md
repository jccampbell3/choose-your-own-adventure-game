# ADR 0002: Give Each Magical Weapon Its Own Canonical Name

- **Status:** Accepted
- **Date:** 2026-11-16
- **Decider:** John Campbell

## Context

The South Path Merchant can enchant the Player's Weapon into a Magical Weapon. The game shows weapon names by looking up their Canonical Name (e.g. `long sword`) in each Language's `displayWeaponMap`. The obvious approach is to build the magical name at display time with a pattern like `"magical {0}"`. That approach breaks in gendered Languages, because the adjective must agree with the noun: Italian *spada magica* but *arco magico*, and Spanish *espada magica* but *arco magico*.

## Decision

Each Magical Weapon has its own Canonical Name (`magical long sword`, `magical lightning bolt spell`, `magical dagger`, `magical long bow`). Each one has an explicit entry in every Language's `displayWeaponMap`. Whether a Weapon is magical is stored as a property of the Weapon, never inferred from its name.

We accept 4 extra entries per Language (16 in total) in exchange for grammatically correct names and a weapon-name lookup that stays a plain map lookup with no per-Language grammar rules.

## Considered Options

- **A `"magical {0}"` pattern per Language.** Fewer entries, but the adjective would be wrong for one gender in Spanish, Italian and French.
- **A pattern per Language plus a gender table.** This adds grammar logic to `Messages` for four items. Rejected as more complex than the entries it saves.

## Note: the Merchant gives the item away

The Merchant is described as "selling" items, but the Player gets one Magical Item for free. A Gold stat would add a new stat, a roll, display lines and messages in every Language, for a choice that is still "pick one". If the game ever needs a currency, this decision is the one to revisit.

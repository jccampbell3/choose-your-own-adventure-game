# Bug: A Halfling can start with zero or negative HealthPoints

Status: resolved

## Problem

A Player's starting HealthPoints are a d20 roll plus the Strength modifier (`Player.RollHealthPoints`). A Halfling's Strength modifier is negative (−1 to −4), so a Halfling who rolls a 1 for health starts with 0 to −3 HealthPoints. That Player is already "dead" before meeting the Dragon.

## Repro

Create a Halfling and let the health roll be 1 while the Strength modifier roll is 1 or more. Starting HealthPoints will be 0 or less.

## Notes

Found while designing the West Path Shrine (see `.scratch/west-shrine/`). The Shrine's Curse never takes a Player below 1 HealthPoint, but that rule can't protect a Player whose health starts below 1. The fix for this bug decides what the minimum starting health is.

## Comments

**2026-11-16 (implemented):** Fixed on `main` in b666bb9, via ticket 01. The minimum starting health is 1: starting HealthPoints are a d20 roll plus the Strength modifier, and a total below 1 becomes 1, for every Race. The Player now takes its Die through the constructor so tests can script character creation. 223 tests pass. The two-axis review found no documented-standard breaches and no missing requirements. Open judgement calls, not blocking:

- The floor of 1 is now written twice: a constant on the Player and a literal `1` in the Shrine's Curse. One shared constant would keep them together.
- The Player's Die is optional (an ordinary Die when none is given), while the Dragon requires one.
- The new tests copy the console-redirect and Messages-loading helpers again (about four copies now).
- The scripted rolls rely on the current roll order in character creation; if that order changes, the tests would check the wrong rolls.
- The glossary mentions the floor of 1 only under Curse.

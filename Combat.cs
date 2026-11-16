/// <summary>A Command the Player can give at the Dragon encounter or in combat.</summary>
public enum CombatChoice
{
    Attack,
    Retreat
}

public class Combat
{
    public Player player { get; set; }
    public Dragon dragon { get; set; }
    public Messages messages { get; set; }

    private readonly Prompt _prompt;
    private readonly TextWriter _output;

    public bool PlayerRetreated { get; private set; }

    public Combat(Player player, Dragon dragon, Messages messages, Prompt prompt, TextWriter output)
    {
        _prompt = prompt;
        _output = output;
        this.player = player;
        this.dragon = dragon;
        this.messages = messages;
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        _output.WriteLine(GetCombatStatsDisplay());

        while (true)
        {
            PlayerAttacksDragonSequence();

            if (dragon.HealthPoints <= 0)
                return true;

            DragonAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            // Invalid input asks again; only 'a' starts another round.
            CombatChoice choice = _prompt.Ask<CombatChoice>(messages.GetMessage("attack_prompt"), TryParseChoice,
                messages.GetMessage("invalid"));

            if (choice == CombatChoice.Retreat)
            {
                PlayerRetreated = true;
                _output.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }
        }
    }

    /// <summary>Reads a Command typed at the Dragon encounter or in combat: "a"/"attack" or "r"/"retreat".</summary>
    public static bool TryParseChoice(string? input, out CombatChoice choice)
    {
        switch (input?.Trim().ToLower())
        {
            case "a": case "attack": choice = CombatChoice.Attack; return true;
            case "r": case "retreat": choice = CombatChoice.Retreat; return true;
            default: choice = default; return false;
        }
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(messages.GetMessage("stats_health") + player.HealthPoints);

        if (player.Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon.Type));
            output.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        if (player.Armor != null)
        {
            output.Add(messages.GetMessage("stats_armor") + messages.GetMessage("armor_name"));
            output.Add(messages.GetMessage("stats_damage_reduction") + player.Armor.DamageReduction);
        }

        output.Add("");
        output.Add(messages.GetMessage("combat_dragon_header"));
        output.Add(messages.GetMessage("stats_name") + dragon.Name);
        output.Add(messages.GetMessage("stats_strength") + dragon.Strength);
        output.Add(messages.GetMessage("stats_agility") + dragon.Agility);
        output.Add(messages.GetMessage("stats_health") + dragon.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(dragon.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + dragon.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksDragonSequence()
    {
        _output.WriteLine(player.Attack(dragon, messages));
    }

    public void DragonAttacksPlayerSequence()
    {
        _output.WriteLine(dragon.Attack(player, messages));
    }
}
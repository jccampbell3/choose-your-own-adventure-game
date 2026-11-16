public class Game
{
    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Scegli la lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string? langInput = Console.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Player player = new Player(die);
                player.CreateCharacter(messages);

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    string selectedPath = PromptForPath(player, messages);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        HandleMerchantEncounter(player, messages);
                    }
                    else if (selectedPath == "west")
                    {
                        HandleShrineEncounter(player, die, messages);
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "w" || trimmed == "west" ||
               trimmed == "i" || trimmed == "inventory" ||
               trimmed == "e" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private string PromptForPath(Player player, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("path_prompt_full"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "west" || input == "w")
                return "west";

            if (input == "exit" || input == "e")
                return "exit";

            if (input == "inventory" || input == "i")
            {
                foreach (string line in Inventory.Lines(player, messages))
                    Console.WriteLine(line);
                continue;
            }

            Console.WriteLine(messages.GetMessage("path_invalid"));
        }
    }

    // Thin console loop around the Merchant's rules; afterwards the Player is back at the path prompt.
    private void HandleMerchantEncounter(Player player, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("south_path_narrative"));

        if (!Merchant.WillOffer(player))
        {
            Console.WriteLine(messages.GetMessage("merchant_refusal"));
            return;
        }

        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        MerchantChoice choice;
        while (true)
        {
            Console.WriteLine(messages.GetMessage("merchant_offer"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine();

            // End of input: treat as leaving so a closed console can't loop forever.
            if (input == null)
            {
                choice = MerchantChoice.Leave;
                break;
            }

            if (Merchant.TryParseChoice(input, out choice))
                break;

            Console.WriteLine(messages.GetMessage("merchant_invalid"));
        }

        switch (Merchant.Choose(player, choice))
        {
            case MerchantOutcome.Armor:
                Console.WriteLine(messages.GetMessage("merchant_received_armor"));
                break;
            case MerchantOutcome.Weapon:
                Console.WriteLine(string.Format(
                    messages.GetMessage("merchant_received_weapon"),
                    messages.TranslateWeaponForDisplay(player.Weapon!.Type)));
                break;
            case MerchantOutcome.Left:
                Console.WriteLine(messages.GetMessage("merchant_leave"));
                break;
            case MerchantOutcome.Refused:
                Console.WriteLine(messages.GetMessage("merchant_refusal"));
                break;
        }
    }

    // Thin console loop around the Shrine's rules; afterwards the Player is back at the path prompt.
    private void HandleShrineEncounter(Player player, Die die, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("west_path_narrative"));

        if (!Shrine.WillOffer(player))
        {
            Console.WriteLine(messages.GetMessage("shrine_silent"));
            return;
        }

        Console.WriteLine(messages.GetMessage("shrine_greeting"));
        Console.WriteLine(messages.GetMessage("shrine_warning"));

        ShrineChoice choice;
        while (true)
        {
            Console.WriteLine(messages.GetMessage("shrine_offer"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine();

            // End of input: treat as leaving so a closed console can't loop forever.
            if (input == null)
            {
                choice = ShrineChoice.Leave;
                break;
            }

            if (Shrine.TryParseChoice(input, out choice))
                break;

            Console.WriteLine(messages.GetMessage("shrine_invalid"));
        }

        ShrineResult result = Shrine.Choose(player, choice, die);
        switch (result.Outcome)
        {
            case ShrineOutcome.Blessing:
                PrintPrayer(result.Roll, "shrine_blessing", player, messages);
                break;
            case ShrineOutcome.Nothing:
                PrintPrayer(result.Roll, "shrine_nothing", player, messages);
                break;
            case ShrineOutcome.Curse:
                PrintPrayer(result.Roll, "shrine_curse", player, messages);
                break;
            case ShrineOutcome.Left:
                Console.WriteLine(messages.GetMessage("shrine_leave"));
                break;
            case ShrineOutcome.Silent:
                Console.WriteLine(messages.GetMessage("shrine_silent"));
                break;
        }
    }

    // A Prayer prints the roll, then what the gods did, then the Player's HealthPoints.
    private static void PrintPrayer(int? roll, string outcomeKey, Player player, Messages messages)
    {
        Console.WriteLine(string.Format(messages.GetMessage("shrine_roll"), roll));
        Console.WriteLine(messages.GetMessage(outcomeKey));
        Console.WriteLine(messages.GetMessage("stats_health") + player.HealthPoints);
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);
        while (true)
        {
            Console.WriteLine(messages.GetMessage("dragon_encounter_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
            {
                Combat combat = new Combat(player, dragon, messages);
                bool playerWon = combat.StartCombat();

                if (combat.PlayerRetreated)
                {
                    return; //combat already handled retreat message
                }
                else
                {
                   EndGame(playerWon, false, messages); 
                }
                
                return;
            }

        if (input == "r" || input == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Console.WriteLine(messages.GetMessage("invalid"));
    }
}
}

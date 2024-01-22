using System;

public enum CharacterClass
{
    Warrior,
    Assassin,
    Mage,
    Skirmisher
}

public class Hero
{
    public string Name { get; private set; }
    public CharacterClass Class { get; private set; }
    public int Strength { get; private set; }
    public int Agility { get; private set; }
    public int Intelligence { get; private set; }
    public int MagickAbility { get; private set; }
    public double HP { get; private set; }
    private double ShieldStrength;
    private double EvasionBonus;
    public double Mana { get; private set; }
    public double MaxMana { get; private set; } = 100; 
    public double HPRegenerationRate { get; private set; } = 2; 
    public double ManaRegenerationRate { get; private set; } = 5; 
    public Random Random { get; } = new Random();

    public Hero(string name, CharacterClass characterClass)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Class = characterClass;
        InitializeAttributes();
        HP = 100 + (Strength * 2);
        Mana = MaxMana;
    }

    private void InitializeAttributes()
    {
        switch (Class)
        {
            case CharacterClass.Warrior: SetAttributes(10, 8, 2, 0); break;
            case CharacterClass.Assassin: SetAttributes(8, 10, 5, 0); break;
            case CharacterClass.Mage: SetAttributes(4, 3, 7, 8); break;
            case CharacterClass.Skirmisher: SetAttributes(7, 9, 4, 0); break;
            default: SetAttributes(); break;
        }
    }


    private void SetAttributes(int strength = 10, int agility = 10, int intelligence = 10, int magickAbility = 0)
    {
        Strength = strength;
        Agility = agility;
        Intelligence = intelligence;
        MagickAbility = magickAbility;
        ShieldStrength = 0;
        EvasionBonus = 0;
    }

    public void Attack(Hero enemy)
    {
        double damage = CalculateDamage(enemy);

        int critChance = Intelligence / 5;
        if (Random.Next(0, 100) < critChance)
        {
            Console.WriteLine($"CRITICAL HIT! \u001b[1;31m{damage * 2}\u001b[0m damage dealt.");
            damage *= 2;
        }
        else
        {
            Console.Write("\u001b[1;33mAttack\u001b[0m ");
        }

        int hitChance = Random.Next(0, 100);
        int dodgeChance = enemy.Agility;

        dodgeChance += (int)EvasionBonus;

        if (hitChance < dodgeChance)
        {
            Console.WriteLine("\u001b[1;32mmissed!\u001b[0m");
        }
        else
        {
            Console.WriteLine($"\u001b[1;34msuccessful!\u001b[0m {damage} damage dealt.");

            enemy.TakeDamage(damage);
            Console.WriteLine($"Enemy HP: {enemy.HP}");
        }
    }

    public void TakeDamage(double damage)
    {
        if (ShieldStrength > 0)
        {
            double blockedDamage = Math.Min(damage, ShieldStrength);
            ShieldStrength -= blockedDamage;
            damage -= blockedDamage;

            Console.WriteLine($"{Name}'s shield blocked \u001b[1;36m{blockedDamage}\u001b[0m damage.");
        }

        HP -= Math.Max(0, damage);
        HP = Math.Max(0, HP);
    }

    public void RegenerateHP()
    {
        double regenerationAmount = 2;
        HP += regenerationAmount;
        Console.WriteLine($"{Name} regenerates \u001b[1;32m{regenerationAmount}\u001b[0m HP.");
    }

    public void RegenerateMana()
    {
        Mana += ManaRegenerationRate;

        Mana = Math.Min(MaxMana, Mana);

        Console.WriteLine($"{Name} regenerates \u001b[1;34m{ManaRegenerationRate}\u001b[0m Mana.");
    }

    public string ShowStats()
    {
        return $"{Name}, Str:{Strength}, Agi:{Agility}, Int:{Intelligence}, Mag:{MagickAbility}, HP:{HP}, Mana:{Mana}/{MaxMana}";
    }

    public void LevelUp()
    {
        Console.Write("Choose an attribute to level up: 1.\u001b[1;31mStrength\u001b[0m, 2.\u001b[1;33mAgility\u001b[0m, 3.\u001b[1;34mIntelligence\u001b[0m, 4.\u001b[1;35mMagickAbility\u001b[0m ... ");
        if (int.TryParse(Console.ReadLine(), out int option))
        {
            switch (option)
            {
                case 1: UpStrength(); break;
                case 2: UpAgility(); break;
                case 3: UpIntelligence(); break;
                case 4: UpMagickAbility(); break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }

        Console.WriteLine();
    }

    public void CastSpell(Hero target)
    {
        if (Mana >= 20)
        {
            switch (Class)
            {
                case CharacterClass.Warrior:
                    CastWarriorSpell(target);
                    break;
                case CharacterClass.Assassin:
                    CastAssassinSpell(target);
                    break;
                case CharacterClass.Mage:
                    CastMageSpell(target);
                    break;
                case CharacterClass.Skirmisher:
                    CastSkirmisherSpell(target);
                    break;
                default:
                    Console.WriteLine("No special spell for this class.");
                    break;
            }

            Mana -= 20;
        }
        else
        {
            Console.WriteLine($"Not enough mana to cast a spell!");

        }

        RegenerateHP();
        RegenerateMana();
        Console.WriteLine($"{ShowStats()} | {target.ShowStats()}");
        Console.WriteLine();
    }




    private void CastWarriorSpell(Hero target)
    {
        ShieldStrength = Strength * 0.5;
        Console.WriteLine($"{Name} casts Shield! Gains \u001b[1;36m{ShieldStrength}\u001b[0m shield.");
    }

    private void CastAssassinSpell(Hero target)
    {
        EvasionBonus = 0.8;
        Console.WriteLine($"{Name} activates Evasion! Dodge chance significantly increased.");
    }

    private void CastMageSpell(Hero target)
    {
        double fireballDamage = Math.Round((MagickAbility * 1.5) * GetRandomMultiplier(5, 10));
        target.TakeDamage(fireballDamage);
        Console.WriteLine($"{Name} hurls a Fireball! Deals \u001b[1;31m{fireballDamage}\u001b[0m damage.");
    }

    private void CastSkirmisherSpell(Hero target)
    {
        double quickStrikeDamage = Math.Round(CalculateDamage(target));
        target.TakeDamage(quickStrikeDamage);
        Console.WriteLine($"{Name} performs a Quick Strike! Deals \u001b[1;33m{quickStrikeDamage}\u001b[0m damage.");
    }

    private void UpStrength() { Strength += 5; HP += 5; }
    private void UpAgility() { Agility += 5; }
    private void UpIntelligence() { Intelligence += 5; }
    private void UpMagickAbility() { MagickAbility += 5; }

    private double CalculateDamage(Hero enemy)
    {
        double baseDamage = 0;

        switch (Class)
        {
            case CharacterClass.Warrior: baseDamage = Strength * GetRandomMultiplier(6, 12); break;
            case CharacterClass.Assassin: baseDamage = Agility * GetRandomMultiplier(8, 15); break;
            case CharacterClass.Mage: baseDamage = MagickAbility * GetRandomMultiplier(7, 14); break;
            case CharacterClass.Skirmisher: baseDamage = Agility * GetRandomMultiplier(7, 14); break;
            default: break;
        }

        return Math.Ceiling(baseDamage / 2) * 2;
    }

    private double GetRandomMultiplier(int min, int max)
    {
        return Random.Next(min, max) / 10.0;
    }
}

public class Program
{
    static Hero ChooseCharacter(int playerNumber)
    {
        Console.WriteLine($"Player {playerNumber}, choose your character:");
        Console.WriteLine("1. \u001b[1;31mWarrior\u001b[0m  2. \u001b[1;33mAssassin\u001b[0m  3. \u001b[1;34mMage\u001b[0m  4. \u001b[1;35mSkirmisher\u001b[0m");

        int choice;

        do
        {
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter your hero's name: ");
                        return new Hero(Console.ReadLine(), CharacterClass.Warrior);
                    case 2:
                        Console.Write("Enter your hero's name: ");
                        return new Hero(Console.ReadLine(), CharacterClass.Assassin);
                    case 3:
                        Console.Write("Enter your hero's name: ");
                        return new Hero(Console.ReadLine(), CharacterClass.Mage);
                    case 4:
                        Console.Write("Enter your hero's name: ");
                        return new Hero(Console.ReadLine(), CharacterClass.Skirmisher);
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option (1-4).");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid option.");
            }

        } while (true);
    }

    static void Main()
    {
        Console.WriteLine("\u001b[1;37mWelcome to the Two-Player Battle Arena!\u001b[0m");

        Hero player1 = ChooseCharacter(1);
        Hero player2 = ChooseCharacter(2);

        Console.WriteLine($"Player 1: {player1.Name}");
        Console.WriteLine($"Player 2: {player2.Name}");

        Console.WriteLine("\u001b[1;37mBattle Start!\u001b[0m");

        while (player1.HP > 0 && player2.HP > 0)
        {
            Console.WriteLine($"{player1.Name}'s turn:");
            Console.WriteLine("1. \u001b[1;32mAttack\u001b[0m  2. \u001b[1;33mLevel Up\u001b[0m  3. \u001b[1;34mSpell\u001b[0m");

            bool validInput = false;
            while (!validInput)
            {
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            player1.Attack(player2);
                            break;
                        case 2:
                            player1.LevelUp();
                            break;
                        case 3:
                            if (player1.Mana >= 20)
                            {
                                player1.CastSpell(player2);
                            }
                            else
                            {
                                Console.WriteLine($"Not enough mana to cast a spell! {player1.Name}'s turn continues.");
                                player1.RegenerateHP();
                                player1.RegenerateMana();
                                Console.WriteLine($"{player1.ShowStats()} | {player2.ShowStats()}");
                                Console.WriteLine();
                                continue;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select a valid option (1-3).");
                            break;
                    }
                    validInput = true; 
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }

            player1.RegenerateHP();
            player1.RegenerateMana();
            Console.WriteLine($"{player1.ShowStats()} | {player2.ShowStats()}");
            Console.WriteLine();

            if (player1.HP <= 0 || player2.HP <= 0)
                break;

            Console.WriteLine($"{player2.Name}'s turn:");
            Console.WriteLine("1. \u001b[1;32mAttack\u001b[0m  2. \u001b[1;33mLevel Up\u001b[0m  3. \u001b[1;34mSpell\u001b[0m");

            validInput = false;
            while (!validInput)
            {
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            player2.Attack(player1);
                            break;
                        case 2:
                            player2.LevelUp();
                            break;
                        case 3:
                            if (player2.Mana >= 20)
                            {
                                player2.CastSpell(player1);
                            }
                            else
                            {
                                Console.WriteLine($"Not enough mana to cast a spell! {player2.Name}'s turn continues.");
                                player2.RegenerateHP();
                                Console.WriteLine($"{player1.ShowStats()} | {player2.ShowStats()}");
                                Console.WriteLine();
                                continue;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select a valid option (1-3).");
                            break;
                    }
                    validInput = true; 
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }

            player2.RegenerateHP();
            Console.WriteLine($"{player1.ShowStats()} | {player2.ShowStats()}");
            Console.WriteLine();
        }

        if (player1.HP <= 0)
        {
            Console.WriteLine($"{player2.Name} \u001b[1;31mwins!\u001b[0m");
        }
        else
        {
            Console.WriteLine($"{player1.Name} \u001b[1;31mwins!\u001b[0m");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using EldenRingDamageCalculator.Enums;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class AshOfWarCompatibilityService
{
    private readonly List<AshCompatibilityInfo>
        _ashes = new();


    public AshOfWarCompatibilityService()
    {
        LoadAshes();
    }


    // ============================================================
    // RETORNAR ASHES COMPATÍVEIS COM A ARMA
    // ============================================================

    public List<AshOfWar> GetCompatibleAshes(
        Weapon weapon)
    {
        var result =
            new List<AshOfWar>();


        if (weapon is null)
        {
            return result;
        }


        // Armas que não aceitam afinidades customizadas
        // normalmente não aceitam troca de Ash of War.
        if (!weapon.AllowsCustomAffinity)
        {
            return result;
        }


        foreach (
            AshCompatibilityInfo info
            in _ashes)
        {
            if (!info.AllowedCategories.Contains(
                    weapon.Category,
                    StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }


            result.Add(
                new AshOfWar
                {
                    Name =
                        info.Name,

                    NativeAffinity =
                        info.NativeAffinity,

                    EffectDescription =
                        info.Description
                });
        }


        return result
            .OrderBy(
                ash =>
                    ash.Name,
                StringComparer.OrdinalIgnoreCase)
            .ToList();
    }


    // ============================================================
    // CADASTRAR TODAS AS ASHES
    // ============================================================

    private void LoadAshes()
    {
        // ========================================================
        // GRUPOS
        // ========================================================

        string[] allMelee =
        {
            "Dagger",
            "Straight Sword",
            "Light Greatsword",
            "Greatsword",
            "Colossal Sword",
            "Thrusting Sword",
            "Heavy Thrusting Sword",
            "Curved Sword",
            "Curved Greatsword",
            "Backhand Blade",
            "Katana",
            "Great Katana",
            "Twinblade",
            "Axe",
            "Greataxe",
            "Hammer",
            "Great Hammer",
            "Flail",
            "Colossal Weapon",
            "Spear",
            "Great Spear",
            "Halberd",
            "Reaper",
            "Whip",
            "Fist",
            "Claw",
            "Beast Claw",
            "Hand-to-Hand",
            "Throwing Blade",
            "Thrusting Shield"
        };


        string[] swords =
        {
            "Dagger",
            "Straight Sword",
            "Light Greatsword",
            "Greatsword",
            "Colossal Sword",
            "Thrusting Sword",
            "Heavy Thrusting Sword",
            "Curved Sword",
            "Curved Greatsword",
            "Backhand Blade",
            "Katana",
            "Great Katana",
            "Twinblade"
        };


        string[] largeWeapons =
        {
            "Greatsword",
            "Colossal Sword",
            "Curved Greatsword",
            "Greataxe",
            "Great Hammer",
            "Colossal Weapon"
        };


        string[] polearms =
        {
            "Spear",
            "Great Spear",
            "Halberd",
            "Reaper"
        };


        string[] thrustWeapons =
        {
            "Dagger",
            "Straight Sword",
            "Light Greatsword",
            "Greatsword",
            "Colossal Sword",
            "Thrusting Sword",
            "Heavy Thrusting Sword",
            "Spear",
            "Great Spear",
            "Halberd",
            "Katana",
            "Great Katana"
        };


        string[] slashWeapons =
        {
            "Straight Sword",
            "Light Greatsword",
            "Greatsword",
            "Colossal Sword",
            "Curved Sword",
            "Curved Greatsword",
            "Backhand Blade",
            "Katana",
            "Great Katana",
            "Twinblade",
            "Axe",
            "Greataxe",
            "Halberd",
            "Reaper"
        };


        string[] smallWeapons =
        {
            "Dagger",
            "Fist",
            "Claw",
            "Beast Claw",
            "Hand-to-Hand"
        };


        string[] shields =
        {
            "Small Shield",
            "Medium Shield",
            "Greatshield"
        };


        string[] bows =
        {
            "Light Bow",
            "Bow"
        };


        // ========================================================
        // HEAVY
        // ========================================================

        Add(
            "Stamp (Upward Cut)",
            WeaponAffinity.Heavy,
            "Postura seguida de corte ascendente.",
            largeWeapons
                .Concat(
                    new[]
                    {
                        "Straight Sword",
                        "Light Greatsword",
                        "Thrusting Sword",
                        "Heavy Thrusting Sword"
                    })
                .ToArray());


        Add(
            "Stamp (Sweep)",
            WeaponAffinity.Heavy,
            "Postura seguida de golpe horizontal.",
            largeWeapons
                .Concat(
                    new[]
                    {
                        "Straight Sword",
                        "Light Greatsword"
                    })
                .ToArray());


        Add(
            "Wild Strikes",
            WeaponAffinity.Heavy,
            "Sequência contínua de golpes.",
            new[]
            {
                "Axe",
                "Greataxe",
                "Hammer",
                "Great Hammer",
                "Flail",
                "Greatsword",
                "Curved Greatsword",
                "Halberd"
            });


        Add(
            "Lion's Claw",
            WeaponAffinity.Heavy,
            "Ataque acrobático com grande impacto.",
            new[]
            {
                "Straight Sword",
                "Light Greatsword",
                "Greatsword",
                "Colossal Sword",
                "Curved Greatsword",
                "Katana",
                "Great Katana",
                "Axe",
                "Greataxe",
                "Hammer",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Savage Lion's Claw",
            WeaponAffinity.Heavy,
            "Versão avançada de Lion's Claw.",
            new[]
            {
                "Straight Sword",
                "Light Greatsword",
                "Greatsword",
                "Colossal Sword",
                "Curved Greatsword",
                "Katana",
                "Great Katana",
                "Axe",
                "Greataxe",
                "Hammer",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Cragblade",
            WeaponAffinity.Heavy,
            "Reveste a arma com pedra e aumenta impacto.",
            allMelee);


        Add(
            "Spinning Gravity Thrust",
            WeaponAffinity.Heavy,
            "Avanço giratório usando força gravitacional.",
            largeWeapons
                .Concat(
                    thrustWeapons)
                .Distinct()
                .ToArray());


        Add(
            "Kick",
            WeaponAffinity.Heavy,
            "Chute eficaz contra inimigos defendendo.",
            allMelee);


        Add(
            "Endure",
            WeaponAffinity.Heavy,
            "Aumenta temporariamente a resistência.",
            allMelee);


        Add(
            "Ground Slam",
            WeaponAffinity.Heavy,
            "Salta e atinge o chão com força.",
            allMelee);


        Add(
            "Earthshaker",
            WeaponAffinity.Heavy,
            "Cria uma onda de choque no chão.",
            new[]
            {
                "Greataxe",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Hoarah Loux's Earthshaker",
            WeaponAffinity.Heavy,
            "Versão poderosa de Earthshaker.",
            allMelee);


        Add(
            "War Cry",
            WeaponAffinity.Heavy,
            "Aumenta o ataque e altera ataques fortes.",
            allMelee);


        Add(
            "Barbaric Roar",
            WeaponAffinity.Heavy,
            "Aumenta o ataque e altera o combo pesado.",
            allMelee);


        Add(
            "Braggart's Roar",
            WeaponAffinity.Heavy,
            "Aumenta ataque, defesa e recuperação de stamina.",
            allMelee);


        Add(
            "Troll's Roar",
            WeaponAffinity.Heavy,
            "Solta um rugido seguido por um ataque forte.",
            largeWeapons);


        Add(
            "Aspects of the Crucible: Wings",
            WeaponAffinity.Heavy,
            "Avanço aéreo inspirado nos Crucible Knights.",
            new[]
            {
                "Straight Sword",
                "Light Greatsword",
                "Greatsword",
                "Colossal Sword",
                "Curved Sword",
                "Curved Greatsword",
                "Katana",
                "Great Katana",
                "Twinblade"
            });


        // ========================================================
        // KEEN
        // ========================================================

        Add(
            "Spinning Slash",
            WeaponAffinity.Keen,
            "Ataque giratório em sequência.",
            slashWeapons);


        Add(
            "Impaling Thrust",
            WeaponAffinity.Keen,
            "Estocada capaz de atravessar defesa.",
            thrustWeapons);


        Add(
            "Piercing Fang",
            WeaponAffinity.Keen,
            "Estocada longa que atravessa escudos.",
            thrustWeapons);


        Add(
            "Repeating Thrust",
            WeaponAffinity.Keen,
            "Sequência rápida de estocadas.",
            thrustWeapons);


        Add(
            "Double Slash",
            WeaponAffinity.Keen,
            "Dois cortes rápidos seguidos.",
            slashWeapons);


        Add(
            "Sword Dance",
            WeaponAffinity.Keen,
            "Avanço com dois cortes giratórios.",
            slashWeapons);


        Add(
            "Unsheathe",
            WeaponAffinity.Keen,
            "Postura de saque rápido para Katanas.",
            new[]
            {
                "Katana"
            });


        Add(
            "Quickstep",
            WeaponAffinity.Keen,
            "Esquiva rápida em qualquer direção.",
            allMelee);


        Add(
            "Bloodhound's Step",
            WeaponAffinity.Keen,
            "Esquiva extremamente rápida.",
            allMelee);


        Add(
            "Raptor of the Mists",
            WeaponAffinity.Keen,
            "Evita um ataque saltando para o ar.",
            allMelee);


        Add(
            "Beast's Roar",
            WeaponAffinity.Keen,
            "Projeta um rugido cortante à distância.",
            allMelee);


        Add(
            "Thunderbolt",
            WeaponAffinity.Lightning,
            "Invoca um raio sobre o alvo.",
            swords
                .Concat(
                    polearms)
                .Distinct()
                .ToArray());


        Add(
            "Lightning Ram",
            WeaponAffinity.Lightning,
            "Rola envolto em eletricidade.",
            allMelee);


        Add(
            "Lightning Slash",
            WeaponAffinity.Lightning,
            "Golpe elétrico que também eletrifica a arma.",
            slashWeapons);


        Add(
            "Blinkbolt",
            WeaponAffinity.Lightning,
            "Avanço instantâneo envolto em raio.",
            allMelee);


        // ========================================================
        // QUALITY / STANDARD
        // ========================================================

        Add(
            "Storm Blade",
            WeaponAffinity.Quality,
            "Lança uma lâmina de vento.",
            swords);


        Add(
            "Storm Assault",
            WeaponAffinity.Quality,
            "Salto giratório seguido de impacto.",
            polearms);


        Add(
            "Stormcaller",
            WeaponAffinity.Quality,
            "Cria uma tempestade giratória ao redor.",
            slashWeapons
                .Concat(
                    polearms)
                .Distinct()
                .ToArray());


        Add(
            "Storm Stomp",
            WeaponAffinity.Quality,
            "Cria uma rajada de vento ao redor.",
            allMelee
                .Concat(
                    new[]
                    {
                        "Perfume Bottle"
                    })
                .ToArray());


        Add(
            "Vacuum Slice",
            WeaponAffinity.Quality,
            "Projeta uma lâmina de ar.",
            largeWeapons);


        Add(
            "Phantom Slash",
            WeaponAffinity.Quality,
            "Cria um fantasma que ataca junto.",
            new[]
            {
                "Twinblade",
                "Spear",
                "Great Spear",
                "Halberd",
                "Reaper"
            });


        Add(
            "Determination",
            WeaponAffinity.Quality,
            "Fortalece o próximo ataque.",
            allMelee);


        Add(
            "Royal Knight's Resolve",
            WeaponAffinity.Quality,
            "Fortalece muito o próximo ataque.",
            allMelee);


        // ========================================================
        // MAGIC / COLD
        // ========================================================

        Add(
            "Glintstone Pebble",
            WeaponAffinity.Magic,
            "Dispara um projétil mágico.",
            swords
                .Concat(
                    polearms)
                .Distinct()
                .ToArray());


        Add(
            "Glintblade Phalanx",
            WeaponAffinity.Magic,
            "Cria lâminas mágicas que atacam automaticamente.",
            swords
                .Concat(
                    polearms)
                .Distinct()
                .ToArray());


        Add(
            "Carian Greatsword",
            WeaponAffinity.Magic,
            "Cria uma grande espada mágica.",
            swords);


        Add(
            "Carian Grandeur",
            WeaponAffinity.Magic,
            "Versão carregável de Carian Greatsword.",
            swords);


        Add(
            "Carian Sovereignty",
            WeaponAffinity.Magic,
            "Grande ataque mágico carregável.",
            swords);


        Add(
            "Gravitas",
            WeaponAffinity.Magic,
            "Puxa inimigos próximos com gravidade.",
            allMelee);


        Add(
            "Waves of Darkness",
            WeaponAffinity.Magic,
            "Cria múltiplas ondas gravitacionais.",
            new[]
            {
                "Greataxe",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Loretta's Slash",
            WeaponAffinity.Magic,
            "Golpe aéreo mágico.",
            polearms);


        Add(
            "Ice Spear",
            WeaponAffinity.Cold,
            "Projeta uma lança de gelo.",
            new[]
            {
                "Spear",
                "Great Spear",
                "Halberd",
                "Twinblade"
            });


        Add(
            "Chilling Mist",
            WeaponAffinity.Cold,
            "Cria névoa gelada e aplica Frost.",
            allMelee);


        Add(
            "Hoarfrost Stomp",
            WeaponAffinity.Cold,
            "Espalha gelo pelo chão.",
            allMelee);


        Add(
            "Divine Beast Frost Stomp",
            WeaponAffinity.Cold,
            "Versão poderosa do Frost Stomp.",
            allMelee);


        Add(
            "Ghostflame Call",
            WeaponAffinity.Cold,
            "Invoca Ghostflame.",
            new[]
            {
                "Spear",
                "Great Spear",
                "Halberd",
                "Reaper"
            });


        // ========================================================
        // FIRE / FLAME ART
        // ========================================================

        Add(
            "Flaming Strike",
            WeaponAffinity.Fire,
            "Espalha fogo e permite um golpe flamejante.",
            slashWeapons);


        Add(
            "Flame of the Redmanes",
            WeaponAffinity.Fire,
            "Projeta uma grande onda de fogo.",
            allMelee);


        Add(
            "Prelate's Charge",
            WeaponAffinity.Fire,
            "Avança continuamente criando chamas.",
            new[]
            {
                "Greataxe",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Black Flame Tornado",
            WeaponAffinity.FlameArt,
            "Cria um tornado de chama negra.",
            new[]
            {
                "Twinblade",
                "Spear",
                "Great Spear",
                "Halberd",
                "Reaper"
            });


        Add(
            "Flame Skewer",
            WeaponAffinity.FlameArt,
            "Estocada flamejante seguida de explosão.",
            thrustWeapons);


        Add(
            "Flame Spear",
            WeaponAffinity.FlameArt,
            "Projeta chama durante uma estocada.",
            thrustWeapons);


        // ========================================================
        // SACRED
        // ========================================================

        Add(
            "Sacred Blade",
            WeaponAffinity.Sacred,
            "Projeta uma lâmina sagrada e fortalece a arma.",
            swords
                .Concat(
                    polearms)
                .Distinct()
                .ToArray());


        Add(
            "Prayerful Strike",
            WeaponAffinity.Sacred,
            "Golpe pesado que recupera HP.",
            new[]
            {
                "Axe",
                "Greataxe",
                "Hammer",
                "Great Hammer",
                "Colossal Weapon"
            });


        Add(
            "Sacred Ring of Light",
            WeaponAffinity.Sacred,
            "Projeta um anel de luz.",
            new[]
            {
                "Spear",
                "Halberd",
                "Reaper"
            });


        Add(
            "Sacred Order",
            WeaponAffinity.Sacred,
            "Fortalece a arma contra mortos-vivos.",
            allMelee);


        Add(
            "Shared Order",
            WeaponAffinity.Sacred,
            "Fortalece o usuário e aliados próximos.",
            allMelee);


        Add(
            "Golden Land",
            WeaponAffinity.Sacred,
            "Golpe no chão seguido de projéteis dourados.",
            largeWeapons);


        Add(
            "Golden Slam",
            WeaponAffinity.Sacred,
            "Salta e atinge o chão com energia dourada.",
            allMelee);


        // ========================================================
        // BLOOD / POISON / OCCULT
        // ========================================================

        Add(
            "Bloody Slash",
            WeaponAffinity.Blood,
            "Usa HP para lançar um corte de sangue.",
            swords);


        Add(
            "Blood Blade",
            WeaponAffinity.Blood,
            "Projeta lâminas de sangue.",
            new[]
            {
                "Dagger",
                "Straight Sword",
                "Light Greatsword",
                "Greatsword",
                "Curved Sword",
                "Katana",
                "Great Katana",
                "Twinblade"
            });


        Add(
            "Blood Tax",
            WeaponAffinity.Blood,
            "Sequência de estocadas que recupera HP.",
            thrustWeapons);


        Add(
            "Seppuku",
            WeaponAffinity.Blood,
            "Aumenta fortemente o acúmulo de sangramento.",
            new[]
            {
                "Straight Sword",
                "Light Greatsword",
                "Greatsword",
                "Curved Sword",
                "Curved Greatsword",
                "Katana",
                "Great Katana",
                "Twinblade",
                "Spear",
                "Great Spear",
                "Halberd"
            });


        Add(
            "Poisonous Mist",
            WeaponAffinity.Poison,
            "Cria névoa venenosa e cobre a arma com veneno.",
            allMelee);


        Add(
            "Poison Moth Flight",
            WeaponAffinity.Poison,
            "Golpe que interage com Poison acumulado.",
            slashWeapons);


        Add(
            "The Poison Flower Blooms Twice",
            WeaponAffinity.Poison,
            "Ataque acrobático que detona Poison ou Rot.",
            allMelee);


        Add(
            "Spectral Lance",
            WeaponAffinity.Occult,
            "Arremessa uma lança espectral.",
            new[]
            {
                "Spear",
                "Great Spear",
                "Halberd"
            });


        Add(
            "Lifesteal Fist",
            WeaponAffinity.Occult,
            "Agarra inimigos humanoides e recupera HP.",
            new[]
            {
                "Fist",
                "Claw",
                "Beast Claw"
            });


        Add(
            "Assassin's Gambit",
            WeaponAffinity.Occult,
            "Reduz som e visibilidade.",
            allMelee);


        Add(
            "White Shadow's Lure",
            WeaponAffinity.Occult,
            "Cria uma sombra que atrai inimigos.",
            allMelee);


        Add(
            "Shriek of Sorrow",
            WeaponAffinity.Occult,
            "Aumenta dano conforme o HP perdido.",
            allMelee);


        // ========================================================
        // BACKHAND BLADE
        // ========================================================

        Add(
            "Blind Spot",
            WeaponAffinity.Keen,
            "Desloca para o ponto cego do inimigo.",
            new[]
            {
                "Backhand Blade"
            });


        Add(
            "Swift Slash",
            WeaponAffinity.Keen,
            "Avanço extremamente rápido com corte.",
            new[]
            {
                "Backhand Blade"
            });


        // ========================================================
        // GREAT KATANA
        // ========================================================

        Add(
            "Overhead Stance",
            WeaponAffinity.Quality,
            "Postura exclusiva de Great Katanas.",
            new[]
            {
                "Great Katana"
            });


        // ========================================================
        // BEAST CLAW
        // ========================================================

        Add(
            "Savage Claws",
            WeaponAffinity.Keen,
            "Avanço feroz com múltiplos golpes.",
            new[]
            {
                "Beast Claw"
            });


        Add(
            "Raging Beast",
            WeaponAffinity.Keen,
            "Ataque móvel exclusivo de Beast Claws.",
            new[]
            {
                "Beast Claw"
            });


        // ========================================================
        // HAND-TO-HAND
        // ========================================================

        Add(
            "Palm Blast",
            WeaponAffinity.Standard,
            "Concentra energia na palma da mão.",
            new[]
            {
                "Hand-to-Hand"
            });


        Add(
            "Dryleaf Whirlwind",
            WeaponAffinity.Standard,
            "Sequência aérea de chutes giratórios.",
            new[]
            {
                "Hand-to-Hand"
            });


        // ========================================================
        // PERFUME BOTTLES
        // ========================================================

        Add(
            "Rolling Sparks",
            WeaponAffinity.Standard,
            "Espalha uma sequência de explosões.",
            new[]
            {
                "Perfume Bottle"
            });


        Add(
            "Wall of Sparks",
            WeaponAffinity.Standard,
            "Cria uma parede de partículas explosivas.",
            new[]
            {
                "Perfume Bottle"
            });


        // ========================================================
        // BOWS
        // ========================================================

        Add(
            "Mighty Shot",
            WeaponAffinity.Standard,
            "Disparo poderoso carregado.",
            bows);


        Add(
            "Barrage",
            WeaponAffinity.Standard,
            "Dispara flechas rapidamente.",
            bows);


        Add(
            "Sky Shot",
            WeaponAffinity.Standard,
            "Dispara uma flecha em arco para o céu.",
            bows);


        Add(
            "Enchanted Shot",
            WeaponAffinity.Standard,
            "Flecha guiada magicamente.",
            bows);


        Add(
            "Rain of Arrows",
            WeaponAffinity.Standard,
            "Faz várias flechas caírem sobre o alvo.",
            bows);


        Add(
            "Through and Through",
            WeaponAffinity.Standard,
            "Disparo poderoso para Greatbows.",
            new[]
            {
                "Greatbow"
            });


        Add(
            "Igon's Drake Hunt",
            WeaponAffinity.Standard,
            "Disparo especializado contra dragões.",
            new[]
            {
                "Greatbow"
            });


        // ========================================================
        // SHIELDS
        // ========================================================

        Add(
            "Parry",
            WeaponAffinity.Standard,
            "Apara ataques inimigos.",
            new[]
            {
                "Small Shield",
                "Medium Shield"
            });


        Add(
            "Golden Parry",
            WeaponAffinity.Sacred,
            "Parry com alcance aumentado.",
            new[]
            {
                "Small Shield",
                "Medium Shield"
            });


        Add(
            "Storm Wall",
            WeaponAffinity.Standard,
            "Parry que também repele projéteis.",
            new[]
            {
                "Small Shield",
                "Medium Shield"
            });


        Add(
            "Shield Bash",
            WeaponAffinity.Standard,
            "Avança protegendo-se com o escudo.",
            shields);


        Add(
            "Shield Crash",
            WeaponAffinity.Standard,
            "Ataque contínuo usando o escudo.",
            shields);


        Add(
            "Barricade Shield",
            WeaponAffinity.Standard,
            "Aumenta muito a capacidade de bloqueio.",
            shields);


        Add(
            "Thops's Barrier",
            WeaponAffinity.Magic,
            "Parry mágico.",
            new[]
            {
                "Small Shield",
                "Medium Shield"
            });


        Add(
            "Carian Retaliation",
            WeaponAffinity.Magic,
            "Parry mágico que cria Glintblades.",
            new[]
            {
                "Small Shield",
                "Medium Shield"
            });


        Add(
            "Shield Strike",
            WeaponAffinity.Standard,
            "Golpe ofensivo de escudo.",
            shields);


        Add(
            "No Skill",
            WeaponAffinity.Standard,
            "Remove a skill do escudo.",
            shields);
    }


    // ============================================================
    // HELPER
    // ============================================================

    private void Add(
        string name,
        WeaponAffinity affinity,
        string description,
        string[] categories)
    {
        _ashes.Add(
            new AshCompatibilityInfo
            {
                Name =
                    name,

                NativeAffinity =
                    affinity,

                Description =
                    description,

                AllowedCategories =
                    categories.ToList()
            });
    }
}


// ============================================================
// DADOS INTERNOS DE COMPATIBILIDADE
// ============================================================

public class AshCompatibilityInfo
{
    public string Name
        { get; set; } = "";


    public WeaponAffinity NativeAffinity
        { get; set; }


    public string Description
        { get; set; } = "";


    public List<string>
        AllowedCategories
        { get; set; } = new();
}   
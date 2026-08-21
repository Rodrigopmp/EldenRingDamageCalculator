using System;
using System.Collections.Generic;

namespace EldenRingDamageCalculator.Services;

public class WeaponSkillService
{
    private readonly Dictionary<string, WeaponSkillInfo>
        _weaponSkills =
            new(
                StringComparer.OrdinalIgnoreCase)
            {
                // ========================================================
                // DAGGERS
                // ========================================================

                ["Reduvia"] =
                    Fixed("Reduvia Blood Blade"),

                ["Blade of Calling"] =
                    Fixed("Blade of Gold"),

                ["Black Knife"] =
                    Fixed("Blade of Death"),

                ["Glintstone Kris"] =
                    Fixed("Glintstone Dart"),

                ["Cinquedea"] =
                    Fixed("Quickstep"),

                ["Scorpion's Stinger"] =
                    Fixed("Repeating Thrust"),


                // ========================================================
                // STRAIGHT SWORDS
                // ========================================================

                ["Sword of Night and Flame"] =
                    Fixed("Night-and-Flame Stance"),

                ["Regalia of Eochaid"] =
                    Fixed("Eochaid's Dancing Blade"),

                ["Coded Sword"] =
                    Fixed("Unblockable Blade"),

                ["Golden Epitaph"] =
                    Fixed("Last Rites"),

                ["Sword of St. Trina"] =
                    Fixed("Mists of Slumber"),

                ["Carian Knight's Sword"] =
                    Fixed("Carian Grandeur"),

                ["Lazuli Glintstone Sword"] =
                    Fixed("Glintstone Pebble"),

                ["Crystal Sword"] =
                    Fixed("Spinning Slash"),

                ["Rotten Crystal Sword"] =
                    Fixed("Spinning Slash"),


                // ========================================================
                // GREATSWORDS
                // ========================================================

                ["Sword of Milos"] =
                    Fixed("Shriek of Milos"),

                ["Marais Executioner's Sword"] =
                    Fixed("Eochaid's Dancing Blade"),

                ["Ordovis's Greatsword"] =
                    Fixed("Ordovis's Vortex"),

                ["Alabaster Lord's Sword"] =
                    Fixed("Alabaster Lords' Pull"),

                ["Helphen's Steeple"] =
                    Fixed("Ruinous Ghostflame"),

                ["Blasphemous Blade"] =
                    Fixed("Taker's Flames"),

                ["Golden Order Greatsword"] =
                    Fixed("Establish Order"),

                ["Dark Moon Greatsword"] =
                    Fixed("Moonlight Greatsword"),

                ["Sacred Relic Sword"] =
                    Fixed("Wave of Gold"),


                // DLC

                ["Greatsword of Damnation"] =
                    Fixed("Golden Crux"),

                ["Greatsword of Solitude"] =
                    Fixed("Solitary Moon Slash"),

                ["Moonrithyll's Knight Sword"] =
                    Fixed("Tremendous Phalanx"),


                // ========================================================
                // LIGHT GREATSWORDS
                // ========================================================

                ["Leda's Sword"] =
                    Fixed("Needle Piercer"),

                ["Rellana's Twin Blades"] =
                    Fixed("Moon-and-Fire Stance"),


                // ========================================================
                // COLOSSAL SWORDS
                // ========================================================

                ["Grafted Blade Greatsword"] =
                    Fixed("Oath of Vengeance"),

                ["Ruins Greatsword"] =
                    Fixed("Wave of Destruction"),

                ["Starscourge Greatsword"] =
                    Fixed("Starcaller Cry"),

                ["Godslayer's Greatsword"] =
                    Fixed("The Queen's Black Flame"),

                ["Maliketh's Black Blade"] =
                    Fixed("Destined Death"),

                ["Royal Greatsword"] =
                    Fixed("Wolf's Assault"),


                // DLC

                ["Ancient Meteoric Ore Greatsword"] =
                    Fixed("White Light Charge"),

                ["Greatsword of Radahn (Light)"] =
                    Fixed("Lightspeed Slash"),

                ["Greatsword of Radahn (Lord)"] =
                    Fixed("Promised Consort"),


                // ========================================================
                // THRUSTING SWORDS
                // ========================================================

                ["Frozen Needle"] =
                    Fixed("Impaling Thrust"),


                // ========================================================
                // HEAVY THRUSTING SWORDS
                // ========================================================

                ["Bloody Helice"] =
                    Fixed("Dynast's Finesse"),

                ["Dragon King's Cragblade"] =
                    Fixed("Thundercloud Form"),


                // ========================================================
                // CURVED SWORDS
                // ========================================================

                ["Magma Blade"] =
                    Fixed("Magma Shower"),

                ["Wing of Astel"] =
                    Fixed("Nebula"),

                ["Eclipse Shotel"] =
                    Fixed("Death Flare"),

                ["Nox Flowing Sword"] =
                    Fixed("Flowing Form"),


                // DLC

                ["Dancing Blade of Ranah"] =
                    Fixed("Unending Dance"),

                ["Falx"] =
                    Fixed("Revenger's Blade"),

                ["Horned Warrior's Sword"] =
                    Fixed("Horn Calling"),

                ["Spirit Sword"] =
                    Fixed("Rancor Slash"),


                // ========================================================
                // CURVED GREATSWORDS
                // ========================================================

                ["Bloodhound's Fang"] =
                    Fixed("Bloodhound's Finesse"),

                ["Onyx Lord's Greatsword"] =
                    Fixed("Onyx Lord's Repulsion"),

                ["Zamor Curved Sword"] =
                    Fixed("Zamor Ice Storm"),

                ["Magma Wyrm's Scalesword"] =
                    Fixed("Magma Guillotine"),

                ["Morgott's Cursed Sword"] =
                    Fixed("Cursed-Blood Slice"),


                // ========================================================
                // KATANAS
                // ========================================================

                ["Moonveil"] =
                    Fixed("Transient Moonlight"),

                ["Rivers of Blood"] =
                    Fixed("Corpse Piler"),

                ["Dragonscale Blade"] =
                    Fixed("Ice Lightning Sword"),

                ["Hand of Malenia"] =
                    Fixed("Waterfowl Dance"),

                ["Meteoric Ore Blade"] =
                    Fixed("Gravitas"),


                // DLC

                ["Star-Lined Sword"] =
                    Fixed("Onze's Line of Stars"),

                ["Sword of Night"] =
                    Fixed("Witching Hour Slash"),


                // ========================================================
                // GREAT KATANAS
                // ========================================================

                ["Dragon-Hunter's Great Katana"] =
                    Fixed("Dragonwound Slash"),


                // ========================================================
                // TWINBLADES
                // ========================================================

                ["Eleonora's Poleblade"] =
                    Fixed("Bloodblade Dance"),

                ["Gargoyle's Black Blades"] =
                    Fixed("Spinning Slash"),


                // DLC

                ["Euporia"] =
                    Fixed("Euporia Vortex"),


                // ========================================================
                // AXES
                // ========================================================

                ["Rosus' Axe"] =
                    Fixed("Rosus's Summons"),

                ["Stormhawk Axe"] =
                    Fixed("Thunderstorm"),

                ["Icerind Hatchet"] =
                    Fixed("Hoarfrost Stomp"),


                // DLC

                ["Death Knight's Twin Axes"] =
                    Fixed("Blinkbolt: Twinaxe"),

                ["Forked-Tongue Hatchet"] =
                    Fixed("Dragonform Flame"),


                // ========================================================
                // GREATAXES
                // ========================================================

                ["Winged Greathorn"] =
                    Fixed("Soul Stifler"),

                ["Axe of Godrick"] =
                    Fixed("I Command Thee, Kneel!"),


                // DLC

                ["Death Knight's Longhaft Axe"] =
                    Fixed("Blinkbolt: Long-hafted Axe"),

                ["Putrescence Cleaver"] =
                    Fixed("Spinning Guillotine"),


                // ========================================================
                // HAMMERS
                // ========================================================

                ["Ringed Finger"] =
                    Fixed("Claw Flick"),

                ["Scepter of the All-Knowing"] =
                    Fixed("Knowledge Above All"),

                ["Marika's Hammer"] =
                    Fixed("Gold Breaker"),

                ["Nox Flowing Hammer"] =
                    Fixed("Flowing Form"),


                // DLC

                ["Flowerstone Gavel"] =
                    Fixed("Flower Dragonbolt"),


                // ========================================================
                // FLAILS
                // ========================================================

                ["Family Heads"] =
                    Fixed("Familial Rancor"),

                ["Bastard's Stars"] =
                    Fixed("Nebula"),


                // DLC

                ["Serpent Flail"] =
                    Fixed("Flare, O Serpent"),


                // ========================================================
                // GREAT HAMMERS
                // ========================================================

                ["Envoy's Long Horn"] =
                    Fixed("Bubble Shower"),

                ["Cranial Vessel Candlestand"] =
                    Fixed("Surge of Faith"),

                ["Beastclaw Greathammer"] =
                    Fixed("Regal Beastclaw"),

                ["Devourer's Scepter"] =
                    Fixed("Devourer of Worlds"),


                // DLC

                ["Devonia's Hammer"] =
                    Fixed("Devonia's Vortex"),


                // ========================================================
                // COLOSSAL WEAPONS
                // ========================================================

                ["Watchdog's Staff"] =
                    Fixed("Sorcery of the Crozier"),

                ["Staff of the Avatar"] =
                    Fixed("Erdtree Slam"),

                ["Rotten Staff"] =
                    Fixed("Erdtree Slam"),

                ["Envoy's Greathorn"] =
                    Fixed("Great Oracular Bubble"),

                ["Ghiza's Wheel"] =
                    Fixed("Spinning Wheel"),

                ["Fallingstar Beast Jaw"] =
                    Fixed("Gravity Bolt"),

                ["Axe of Godfrey"] =
                    Fixed("Regal Roar"),


                // DLC

                ["Anvil Hammer"] =
                    Fixed("Smithing Art Spears"),

                ["Gazing Finger"] =
                    Fixed("Kowtower's Resentment"),

                ["Shadow Sunflower Blossom"] =
                    Fixed("Shadow Sunflower Headbutt"),


                // ========================================================
                // SPEARS
                // ========================================================

                ["Bolt of Gransax"] =
                    Fixed("Ancient Lightning Spear"),

                ["Death Ritual Spear"] =
                    Fixed("Spearcall Ritual"),

                ["Cleanrot Spear"] =
                    Fixed("Sacred Phalanx"),


                // DLC

                ["Bloodfiend's Fork"] =
                    Fixed("Barbaric Roar"),


                // ========================================================
                // GREAT SPEARS
                // ========================================================

                ["Serpent-Hunter"] =
                    Fixed("Great-Serpent Hunt"),

                ["Siluria's Tree"] =
                    Fixed("Siluria's Woe"),

                ["Vyke's War Spear"] =
                    Fixed("Frenzyflame Thrust"),

                ["Mohgwyn's Sacred Spear"] =
                    Fixed("Bloodboon Ritual"),


                // DLC

                ["Barbed Staff-Spear"] =
                    Fixed("Jori's Inquisition"),

                ["Bloodfiend's Sacred Spear"] =
                    Fixed("Bloodfiend's Bloodboon"),

                ["Spear of the Impaler"] =
                    Fixed("Messmer's Assault"),


                // ========================================================
                // HALBERDS
                // ========================================================

                ["Commander's Standard"] =
                    Fixed("Rallying Standard"),

                ["Dragon Halberd"] =
                    Fixed("Spinning Slash (Ice Lightning)"),

                ["Loretta's War Sickle"] =
                    Fixed("Loretta's Slash"),

                ["Golden Halberd"] =
                    Fixed("Golden Vow"),


                // DLC

                ["Poleblade of the Bud"] =
                    Fixed("Romina's Purification"),

                ["Spirit Glaive"] =
                    Fixed("Rancor Slash"),


                // ========================================================
                // REAPERS
                // ========================================================

                ["Halo Scythe"] =
                    Fixed("Miquella's Ring of Light"),

                ["Winged Scythe"] =
                    Fixed("Angel's Wings"),


                // DLC

                ["Obsidian Lamina"] =
                    Fixed("Dynastic Sickleplay"),


                // ========================================================
                // WHIPS
                // ========================================================

                ["Magma Whip Candlestick"] =
                    Fixed("Sea of Magma"),

                ["Giant's Red Braid"] =
                    Fixed("Flame Dance"),


                // DLC

                ["Tooth Whip"] =
                    Fixed("Painful Strike"),


                // ========================================================
                // FISTS
                // ========================================================

                ["Cipher Pata"] =
                    Fixed("Unblockable Blade"),

                ["Grafted Dragon"] =
                    Fixed("Bear Witness!"),

                ["Clinging Bone"] =
                    Fixed("Lifesteal Fist"),

                ["Veteran's Prosthesis"] =
                    Fixed("Storm Kick"),


                // DLC

                ["Madding Hand"] =
                    Fixed("Madding Spear-Hand Strike"),

                ["Poisoned Hand"] =
                    Fixed("Poison Spear-Hand Strike"),

                ["Thiollier's Hidden Needle"] =
                    Fixed("Sleep Evermore"),


                // ========================================================
                // BEAST CLAWS
                // ========================================================

                ["Red Bear's Claw"] =
                    Fixed("Red Bear Hunt"),


                // ========================================================
                // CLAWS
                // ========================================================

                ["Claws of Night"] =
                    Fixed("Scattershot Throw"),


                // ========================================================
                // BOWS
                // ========================================================

                ["Black Bow"] =
                    Fixed("Barrage"),

                ["Horn Bow"] =
                    Fixed("Mighty Shot"),

                ["Erdtree Bow"] =
                    Fixed("Mighty Shot"),


                // ========================================================
                // GREATBOWS
                // ========================================================

                ["Lion Greatbow"] =
                    Fixed("Radahn's Rain"),


                // DLC

                ["Igon's Greatbow"] =
                    Fixed("Igon's Drake Hunt"),


                // ========================================================
                // CROSSBOWS
                // ========================================================

                ["Pulley Crossbow"] =
                    Fixed("Kick"),


                // DLC

                ["Repeating Crossbow"] =
                    Fixed("Repeating Fire"),


                // ========================================================
                // TORCHES
                // ========================================================

                ["Steel-Wire Torch"] =
                    Fixed("Firebreather"),

                ["Sentry's Torch"] =
                    Fixed("Torch Attack"),

                ["Ghostflame Torch"] =
                    Fixed("Torch Attack"),

                ["St. Trina's Torch"] =
                    Fixed("Fires of Slumber"),


                // DLC

                ["Lamenting Visage"] =
                    Fixed("Blindfold of Happiness"),

                ["Nanaya's Torch"] =
                    Fixed("Feeble Lord's Frenzied Flame"),


                // ========================================================
                // SMALL / MEDIUM / GREAT SHIELDS
                // ========================================================

                ["One-Eyed Shield"] =
                    Fixed("Flame Spit"),

                ["Visage Shield"] =
                    Fixed("Tongues of Fire"),

                ["Jellyfish Shield"] =
                    Fixed("Contagious Fury"),

                ["Erdtree Greatshield"] =
                    Fixed("Golden Retaliation"),


                // DLC

                ["Golden Lion Shield"] =
                    Fixed("Roaring Bash"),

                ["Shield of Night"] =
                    Fixed("Revenge of the Night"),

                ["Smithscript Shield"] =
                    Fixed("Discus Hurl"),

                ["Verdigris Greatshield"] =
                    Fixed("Moore's Charge"),


                // ========================================================
                // OUTRAS ARMAS DLC
                // ========================================================

                ["Stone-Sheathed Sword"] =
                    Fixed("Square Off"),

                ["Sword of Light"] =
                    Fixed("Light"),

                ["Sword of Darkness"] =
                    Fixed("Darkness"),

                ["Velvet Sword of St. Trina"] =
                    Fixed("Mists of Eternal Sleep"),


                // ========================================================
                // ARMAS NORMAIS - SKILL PADRÃO, MAS PODE TROCAR
                //
                // Estamos cadastrando algumas importantes já agora.
                // Na etapa de compatibilidade vamos expandir isso.
                // ========================================================

                ["Uchigatana"] =
                    Replaceable("Unsheathe"),

                ["Nagakiba"] =
                    Replaceable("Unsheathe"),

                ["Greatsword"] =
                    Replaceable("Stamp (Upward Cut)"),

                ["Claymore"] =
                    Replaceable("Lion's Claw"),

                ["Milady"] =
                    Replaceable("Impaling Thrust"),

                ["Backhand Blade"] =
                    Replaceable("Blind Spot"),

                ["Dryleaf Arts"] =
                    Replaceable("Palm Blast"),

                ["Dane's Footwork"] =
                    Replaceable("Palm Blast"),

                ["Great Katana"] =
                    Replaceable("Overhead Stance")
            };


    // ============================================================
    // BUSCAR A SKILL DE UMA ARMA
    // ============================================================

    public WeaponSkillInfo GetSkillInfo(
        string weaponName)
    {
        if (string.IsNullOrWhiteSpace(
                weaponName))
        {
            return new WeaponSkillInfo();
        }


        if (_weaponSkills.TryGetValue(
                weaponName,
                out WeaponSkillInfo? skillInfo))
        {
            return skillInfo;
        }


        return new WeaponSkillInfo();
    }


    // ============================================================
    // CRIAR SKILL FIXA
    // ============================================================

    private static WeaponSkillInfo Fixed(
        string skillName)
    {
        return new WeaponSkillInfo
        {
            SkillName =
                skillName,

            IsFixed =
                true
        };
    }


    // ============================================================
    // CRIAR SKILL SUBSTITUÍVEL
    // ============================================================

    private static WeaponSkillInfo Replaceable(
        string skillName)
    {
        return new WeaponSkillInfo
        {
            SkillName =
                skillName,

            IsFixed =
                false
        };
    }
}


// ============================================================
// INFORMAÇÕES DA SKILL DA ARMA
// ============================================================

public class WeaponSkillInfo
{
    public string SkillName
        { get; set; } = "";


    public bool IsFixed
        { get; set; }
}
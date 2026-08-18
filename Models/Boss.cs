using System.Collections.Generic;

namespace EldenRingDamageCalculator.Models;

public class Boss
{
    public string Name { get; set; } = "";

    public List<BossPhase> Phases { get; set; } = new();

    public override string ToString()
    {
        return Name;
    }
}

public class BossPhase
{
    public string Name { get; set; } = "";

    public decimal StandardNegation { get; set; }

    public decimal SlashNegation { get; set; }

    public decimal StrikeNegation { get; set; }

    public decimal PierceNegation { get; set; }

    public decimal MagicNegation { get; set; }

    public decimal FireNegation { get; set; }

    public decimal LightningNegation { get; set; }

    public decimal HolyNegation { get; set; }

    public bool HasVerifiedResistanceData { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
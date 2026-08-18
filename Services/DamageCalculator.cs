using System.Collections.Generic;
using System.Linq;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class DamageCalculator
{
    public decimal CalculateQuickDamage(
        decimal baseDamage,
        IEnumerable<Buff> buffs)
    {
        decimal damage = baseDamage;

        var selectedBuffs = buffs.Where(buff => buff.IsSelected);

        foreach (var buff in selectedBuffs)
        {
            decimal multiplier =
                1m + (buff.DamageBonusPercent / 100m);

            damage *= multiplier;
        }

        return decimal.Round(damage, 2);
    }
}
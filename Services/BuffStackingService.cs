using System.Collections.Generic;
using System.Linq;
using EldenRingDamageCalculator.Models;

namespace EldenRingDamageCalculator.Services;

public class BuffStackingService
{
    public string FindConflicts(IEnumerable<Buff> buffs)
    {
        var conflicts = buffs
            .Where(buff => buff.IsSelected)
            .Where(buff => !string.IsNullOrWhiteSpace(buff.StackGroup))
            .GroupBy(buff => buff.StackGroup)
            .Where(group => group.Count() > 1)
            .ToList();

        if (conflicts.Count == 0)
        {
            return "";
        }

        var messages = conflicts.Select(group =>
        {
            var names = string.Join(" + ", group.Select(buff => buff.Name));

            return $"{names} não stackam porque pertencem ao grupo {group.Key}.";
        });

        return string.Join("\n", messages);
    }
}
using MapGenerator.Domain.Enums;
using MapGenerator.Domain.Models;

namespace MapGenerator.Application.Services;

public static class BuffService
{
    public static void ApplyBuff(Player player, BuffDefinition buff)
    {
        player.ActiveBuffs.RemoveAll(b => b.Type == buff.Type);
        player.ActiveBuffs.Add(new ActiveBuff
        {
            Type             = buff.Type,
            Magnitude        = buff.Magnitude,
            RemainingCharges = buff.Charges,
        });
    }

    // Peek: returns the current magnitude without spending a charge.
    public static double PeekCooldownMultiplier(Player player) =>
        player.ActiveBuffs.FirstOrDefault(b => b.Type == BuffType.CooldownReduction)?.Magnitude ?? 1.0;

    // Each Consume method returns the buff magnitude and spends one charge.
    // Returns 1.0 (identity) if no active buff of that type.

    public static double ConsumeCooldownMultiplier(Player player) =>
        ConsumeCharge(player, BuffType.CooldownReduction);

    public static double ConsumeGatherMultiplier(Player player) =>
        ConsumeCharge(player, BuffType.GatherBonus);

    public static double ConsumeHungerDrainMultiplier(Player player) =>
        ConsumeCharge(player, BuffType.HungerDrainReduction);

    public static double PeekFishingRarityMultiplier(Player player) =>
        player.ActiveBuffs.FirstOrDefault(b => b.Type == BuffType.FishingRarityBonus)?.Magnitude ?? 1.0;

    public static double PeekFishingStrikeMultiplier(Player player) =>
        player.ActiveBuffs.FirstOrDefault(b => b.Type == BuffType.FishingStrikeBonus)?.Magnitude ?? 1.0;

    public static double ConsumeFishingRarityCharge(Player player) =>
        ConsumeCharge(player, BuffType.FishingRarityBonus);

    public static double ConsumeFishingStrikeCharge(Player player) =>
        ConsumeCharge(player, BuffType.FishingStrikeBonus);

    private static double ConsumeCharge(Player player, BuffType type)
    {
        var buff = player.ActiveBuffs.FirstOrDefault(b => b.Type == type);
        if (buff == null) return 1.0;
        double magnitude = buff.Magnitude;
        if (--buff.RemainingCharges <= 0)
            player.ActiveBuffs.Remove(buff);
        return magnitude;
    }
}

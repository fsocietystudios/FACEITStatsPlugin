using System.Collections.Concurrent;

namespace FACEITStatsPlugin.Utils;

public static class CooldownManager
{
    private static readonly ConcurrentDictionary<ulong, DateTime> _playerCooldowns = new();

    public static bool IsOnCooldown(ulong steamId, int cooldownMinutes)
    {
        if (_playerCooldowns.TryGetValue(steamId, out DateTime lastUsed))
        {
            return DateTime.UtcNow < lastUsed.AddMinutes(cooldownMinutes);
        }
        return false;
    }

    public static void SetCooldown(ulong steamId)
    {
        _playerCooldowns.AddOrUpdate(steamId, DateTime.UtcNow, (key, oldValue) => DateTime.UtcNow);
    }

    public static TimeSpan GetRemainingCooldown(ulong steamId, int cooldownMinutes)
    {
        if (_playerCooldowns.TryGetValue(steamId, out DateTime lastUsed))
        {
            var cooldownEnd = lastUsed.AddMinutes(cooldownMinutes);
            var remaining = cooldownEnd - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
        return TimeSpan.Zero;
    }

    public static void ClearCooldown(ulong steamId)
    {
        _playerCooldowns.TryRemove(steamId, out _);
    }

    public static void ClearAllCooldowns()
    {
        _playerCooldowns.Clear();
    }
} 
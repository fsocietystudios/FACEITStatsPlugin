using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using FACEITStatsPlugin.Models;

namespace FACEITStatsPlugin.Utils;

public static class ChatUtils
{
    public static string GetSkillLevelColor(string skillLevel)
    {
        if (!int.TryParse(skillLevel, out int level)) return "\x01";
        
        return level switch
        {
            10 => "\x02",
            >= 8 => "\x04",
            >= 4 => "\x09",
            >= 1 => "\x03",
            _ => "\x01"
        };
    }

    public static void DisplayStatsInChat(CCSPlayerController player, PlayerStats stats, string chatPrefix)
    {
        var color = GetSkillLevelColor(stats.SkillLevel ?? "0");
        
        player.PrintToChat($"{chatPrefix} {ChatColors.Orange}Nickname: {color}{stats.Nickname ?? "Unknown"} {ChatColors.Orange}Skill Level: {color}{stats.SkillLevel ?? "Unknown"} {ChatColors.Orange}ELO: {color}{stats.ELO?.ToString() ?? "Unknown"}");
        
        if (stats.AdditionalStats != null)
        {
            player.PrintToChat($"{chatPrefix} {ChatColors.Orange}Avg K/D: {ChatColors.Default}{stats.AverageKDRatio ?? "Unknown"} {ChatColors.Orange}Avg ADR: {ChatColors.Default}{stats.AverageADR ?? "Unknown"} {ChatColors.Orange}Avg HS%: {ChatColors.Default}{stats.AverageHeadshots ?? "Unknown"}%");
            player.PrintToChat($"{chatPrefix} {ChatColors.Orange}Win Rate: {ChatColors.Default}{stats.WinRate ?? "Unknown"}% {ChatColors.Orange}Matches: {ChatColors.Default}{stats.MatchesCount ?? "Unknown"}");
        }
        
        player.PrintToChat($"{chatPrefix} {ChatColors.Orange}Country: {ChatColors.Default}{stats.Country?.ToUpper() ?? "Unknown"} {ChatColors.Orange}Region: {ChatColors.Default}{stats.Region ?? "Unknown"}");
        player.PrintToChat($"{chatPrefix} {ChatColors.Orange}Link: {ChatColors.Default}https://faceit.com/en/players/{stats.Nickname}");
    }
}
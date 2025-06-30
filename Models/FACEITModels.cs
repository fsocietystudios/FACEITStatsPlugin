using System.Text.Json.Serialization;

namespace FACEITStatsPlugin.Models;

public class FACEITResponse
{
    [JsonPropertyName("data")]
    public FACEITData? Data { get; set; }
}

public class FACEITData
{
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    
    [JsonPropertyName("games")]
    public GamesData? Games { get; set; }
    
    [JsonPropertyName("id")]
    public string? PlayerId { get; set; }
}

public class GamesData
{
    [JsonPropertyName("cs2")]
    public CS2Data? CS2 { get; set; }
}

public class CS2Data
{
    [JsonPropertyName("game_name")]
    public string? GameName { get; set; }
    
    [JsonPropertyName("skill_level_label")]
    public string? SkillLevelLabel { get; set; }
    
    [JsonPropertyName("faceit_elo")]
    public int? FACEITElo { get; set; }
    
    [JsonPropertyName("region")]
    public string? Region { get; set; }
}

public class CS2StatsResponse
{
    [JsonPropertyName("player_id")]
    public string? PlayerId { get; set; }
    
    [JsonPropertyName("lifetime")]
    public LifetimeStats? Lifetime { get; set; }
}

public class LifetimeStats
{
    [JsonPropertyName("Average K/D Ratio")]
    public string? AverageKDRatio { get; set; }
    
    [JsonPropertyName("ADR")]
    public string? ADR { get; set; }
    
    [JsonPropertyName("Win Rate %")]
    public string? WinRate { get; set; }
    
    [JsonPropertyName("Average Headshots %")]
    public string? AverageHeadshots { get; set; }
    
    [JsonPropertyName("Matches")]
    public string? Matches { get; set; }
}

public class PlayerStats
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Nickname { get; set; }
    public string? SkillLevel { get; set; }
    public int? ELO { get; set; }
    public string? Country { get; set; }
    public string? Region { get; set; }
    public string? PlayerId { get; set; }
    public string? AdditionalStats { get; set; }
    public string? AverageKDRatio { get; set; }
    public string? AverageADR { get; set; }
    public string? WinRate { get; set; }
    public string? AverageHeadshots { get; set; }
    public string? MatchesCount { get; set; }
}
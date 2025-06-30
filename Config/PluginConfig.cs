using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace FACEITStatsPlugin.Config;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("ChatPrefix")] 
    public string ChatPrefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Orange}FACEIT Stats{ChatColors.Default}]";

    [JsonPropertyName("ChatInterval")] 
    public float ChatInterval { get; set; } = 60;

    [JsonPropertyName("APIKey")] 
    public string APIKey { get; set; } = "572eb8be-bf58-47b0-aad1-92774ae9e5a4";
} 
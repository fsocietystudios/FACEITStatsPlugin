using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace FACEITStatsPlugin.Config;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("ChatPrefix")] 
    public string ChatPrefix { get; set; } = $" {ChatColors.Default}[{ChatColors.Orange}FACEIT Stats{ChatColors.Default}]";

    [JsonPropertyName("APIKey")] 
    public string APIKey { get; set; } = "YOUR_API_KEY_GOES_HERE";
}
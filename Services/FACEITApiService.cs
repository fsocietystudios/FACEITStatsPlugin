using System.Text.Json;
using FACEITStatsPlugin.Config;
using FACEITStatsPlugin.Models;
using Microsoft.Extensions.Logging;

namespace FACEITStatsPlugin.Services;

public class FACEITApiService
{
    private readonly PluginConfig _config;
    private readonly ILogger _logger;

    public FACEITApiService(PluginConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<PlayerStats> FetchAllPlayerStats(ulong steamId)
    {
        var result = new PlayerStats();

        if (steamId == 0)
        {
            result.Success = false;
            result.ErrorMessage = "Invalid SteamID provided!";
            return result;
        }

        string APIurl = $"https://faceitanalyser.com/api/playerDetails/{steamId}";
        using HttpClient client = new HttpClient();

        try
        {
            HttpResponseMessage response = await client.GetAsync(APIurl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseBody))
            {
                result.Success = false;
                result.ErrorMessage = "Empty response received from FACEIT API!";
                return result;
            }

            try
            {
                var data = JsonSerializer.Deserialize<FACEITResponse>(responseBody);
                
                if (data?.Data?.Games?.CS2 != null)
                {
                    result.Nickname = data.Data.Games.CS2.GameName ?? "Unknown";
                    result.SkillLevel = data.Data.Games.CS2.SkillLevelLabel ?? "Unknown";
                    result.ELO = data.Data.Games.CS2.FACEITElo;
                    result.Country = data.Data.Country ?? "Unknown";
                    result.Region = data.Data.Games.CS2.Region ?? "Unknown";
                    result.PlayerId = data.Data.PlayerId;

                    if (!string.IsNullOrEmpty(result.PlayerId))
                    {
                        try
                        {
                            result.AdditionalStats = await FetchAdditionalCS2Stats(result.PlayerId);
                            
                            if (!string.IsNullOrEmpty(result.AdditionalStats))
                            {
                                try
                                {
                                    var cs2Stats = JsonSerializer.Deserialize<CS2StatsResponse>(result.AdditionalStats);
                                    if (cs2Stats?.Lifetime != null)
                                    {
                                        result.AverageKDRatio = cs2Stats.Lifetime.AverageKDRatio ?? "Unknown";
                                        result.AverageADR = cs2Stats.Lifetime.ADR ?? "Unknown";
                                        result.WinRate = cs2Stats.Lifetime.WinRate ?? "Unknown";
                                        result.AverageHeadshots = cs2Stats.Lifetime.AverageHeadshots ?? "Unknown";
                                        result.MatchesCount = cs2Stats.Lifetime.Matches ?? "Unknown";
                                    }
                                }
                                catch (JsonException ex)
                                {
                                    _logger.LogWarning($"{_config.ChatPrefix} Error parsing additional CS2 stats: {ex.Message}");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            result.Success = false;
                            result.ErrorMessage = $"Failed to fetch additional CS2 stats!";
                        }
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = "No CS2 data found for this player!";
                }
            }
            catch (JsonException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error parsing FACEIT response: {ex.Message}";
            }

            return result;
        }
        catch (HttpRequestException)
        {
            result.Success = false;
            result.ErrorMessage = $"HTTP error!";
            return result;
        }
        catch (TaskCanceledException)
        {
            result.Success = false;
            result.ErrorMessage = "Request timeout!";
            return result;
        }
        catch (Exception)
        {
            result.Success = false;
            result.ErrorMessage = $"Unexpected error!";
            return result;
        }
    }

    private async Task<string> FetchAdditionalCS2Stats(string playerId)
    {
        if (string.IsNullOrEmpty(_config.APIKey))
        {
            _logger.LogError($"{_config.ChatPrefix} API key is not configured");
            return string.Empty;
        }

        string APIurl = $"https://open.faceit.com/data/v4/players/{playerId}/stats/cs2";
        using HttpClient client = new HttpClient();

        _logger.LogInformation(APIurl);

        try
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.APIKey}");

            HttpResponseMessage response = await client.GetAsync(APIurl);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{_config.ChatPrefix} FACEIT API returned error status: {response.StatusCode} - {response.ReasonPhrase}");
                return string.Empty;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseBody))
            {
                _logger.LogError($"{_config.ChatPrefix} Empty response received from FACEIT CS2 stats API");
                return string.Empty;
            }

            return responseBody;
        }
        catch (HttpRequestException)
        {
            _logger.LogError($"{_config.ChatPrefix} HTTP error occurred while trying to fetch CS2 stats");
            return string.Empty;
        }
        catch (TaskCanceledException)
        {
            _logger.LogError($"{_config.ChatPrefix} Request timeout while trying to fetch CS2 stats");
            return string.Empty;
        }
        catch (Exception)
        {
            _logger.LogError($"{_config.ChatPrefix} Unexpected error occurred while trying to fetch CS2 stats");
            return string.Empty;
        }
    }
} 
using Microsoft.Extensions.Logging;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using FACEITStatsPlugin.Config;
using FACEITStatsPlugin.Models;
using FACEITStatsPlugin.Services;
using FACEITStatsPlugin.Utils;

namespace FACEITStatsPlugin;

[MinimumApiVersion(80)]
public class FACEITStatsPlugin : BasePlugin, IPluginConfig<PluginConfig>
{
    public override string ModuleName => "[CS] FACEIT Stats Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "FSOCIETY Studios";
    public override string ModuleDescription => "A simple FACEIT stats retriever using FACEITAnalyser's API.";

    public required PluginConfig Config { get; set; }
    private FACEITApiService? _apiService;

    public void OnConfigParsed(PluginConfig config)
    {
        if (config.ChatPrefix.Length > 25)
        {
            throw new Exception($"Invalid value has been set to config value 'ChatPrefix': {config.ChatPrefix}");
        }

        Config = config;
        _apiService = new FACEITApiService(config, Logger);
    }

    public override void Load(bool hotReload)
    {
        Logger.LogInformation($"{Config.ChatPrefix} Loaded!");
    }

    public override void Unload(bool hotReload)
    {
        Logger.LogInformation($"{Config.ChatPrefix} Unloaded!");
    }

    [ConsoleCommand("css_fs", "Responds with the FACEIT stats menu")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnFSCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player != null) 
            OpenFSMenu(player);
    }

    public void OpenFSMenu(CCSPlayerController player)
    {
        var menu = new ChatMenu($"{Config.ChatPrefix} FACEIT Stats Menu")
        {
            TitleColor = ChatColors.Orange,
            ExitButton = true
        };

        menu.AddMenuOption("Your FACEIT stats", (p, o) => { DisplayPlayerStats(player, player.SteamID); MenuManager.CloseActiveMenu(player); }, false);
        menu.AddMenuOption("Another player's FACEIT stats", (p, o) => { MenuManager.CloseActiveMenu(player); OpenPlayerSelectionMenu(player); }, false);

        MenuManager.OpenChatMenu(player, menu);
    }

    public void OpenPlayerSelectionMenu(CCSPlayerController player)
    {
        var menu = new ChatMenu($"{Config.ChatPrefix} Select a player")
        {
            TitleColor = ChatColors.Orange,
            ExitButton = true
        };

        foreach (var targetPlayer in Utilities.GetPlayers().Where(p => p?.PlayerName != null))
        {
            menu.AddMenuOption(targetPlayer.PlayerName, (p, o) => { DisplayPlayerStats(player, targetPlayer.SteamID); MenuManager.CloseActiveMenu(player); }, false);
        }

        MenuManager.OpenChatMenu(player, menu);
    }

    private void DisplayPlayerStats(CCSPlayerController requester, ulong targetSteamId)
    {
        _apiService.FetchAllPlayerStats(targetSteamId).ContinueWith(task =>
        {
            Server.NextFrame(() =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    requester.PrintToChat($"{Config.ChatPrefix} Failed to fetch stats. Please try again later.");
                    return;
                }

                PlayerStats stats = task.Result;
                if (!stats.Success)
                {
                    requester.PrintToChat($"{Config.ChatPrefix} {stats.ErrorMessage}");
                    return;
                }

                ChatUtils.DisplayStatsInChat(requester, stats, Config.ChatPrefix);
            });
        });
    }
}
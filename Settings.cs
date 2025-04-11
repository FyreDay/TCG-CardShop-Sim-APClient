using BepInEx.Configuration;
using UnityEngine;
namespace ApClient;

public class Settings
{
    public static Settings Instance
    {
        get
        {
            Settings.m_instance ??= new Settings();
            return Settings.m_instance;
        }
    }
    private static Settings m_instance;

    private Plugin plugin;

    //public static ConfigEntry<bool> EnableFeature;
    public ConfigEntry<int> StartingMoney;
    public ConfigEntry<int> XpMultiplier;
    public ConfigEntry<KeyCode> MyHotkey;
    public ConfigEntry<string> LastUsedIP;
    public ConfigEntry<string> LastUsedPassword;
    public ConfigEntry<string> LastUsedSlot;

    public void Load(Plugin plugin)
    {
        this.plugin = plugin;

        this.StartingMoney = plugin.Config.Bind<int>("1. GamePlay", "Starting Money", 1000, new ConfigDescription("The Amount of Money you Start With", new AcceptableValueRange<int>(1, int.MaxValue)));//, new ConfigurationManagerAttributes { Order = 1 }));

        MyHotkey = plugin.Config.Bind(
            "2. Hotkeys",
            "Toggle Connection Window",
            KeyCode.F8, // Default key
            "Press this key to op AP Connection GUI"
);

        LastUsedIP = plugin.Config.Bind("Connection", "LastUsedIP", "", "The last server IP entered.");
        LastUsedPassword = plugin.Config.Bind("Connection", "LastUsedPassword", "", "The last server password entered.");
        LastUsedSlot = plugin.Config.Bind("Connection", "LastUsedSlot", "", "The last player slot name entered.");

    }

    public void SaveNewConnectionInfo(string ip, string password, string slot)
    {
        LastUsedIP.Value = ip;
        LastUsedPassword.Value = password;
        LastUsedSlot.Value = slot;

        // Optional: force save right away (usually not needed)
        this.plugin.Config.Save();
    }
}

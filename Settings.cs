﻿using BepInEx.Configuration;
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
    public ConfigEntry<KeyCode> ConsoleHotkey;
    public ConfigEntry<bool> disabledeathlink;
    public ConfigEntry<string> LastUsedIP;
    public ConfigEntry<string> LastUsedPassword;
    public ConfigEntry<string> LastUsedSlot;

    public void Load(Plugin plugin)
    {
        this.plugin = plugin;

        //this.StartingMoney = plugin.Config.Bind<int>("1. GamePlay", "Starting Money", 1000, new ConfigDescription("The Amount of Money you Start With", new AcceptableValueRange<int>(1, int.MaxValue)));//, new ConfigurationManagerAttributes { Order = 1 }));
        
        disabledeathlink = plugin.Config.Bind(
            "1. GamePlay",
            "Force Deathlink Off",
            false, 
            "If this is on, Deathlink is forced off"
        );

        MyHotkey = plugin.Config.Bind(
            "2. Hotkeys",
            "Toggle Connection Window",
            KeyCode.F8, // Default key
            "Press this key to toggle AP Connection GUI"
        );
        ConsoleHotkey = plugin.Config.Bind(
            "2. Hotkeys",
            "Toggle AP Console",
            KeyCode.F9, // Default key
            "Press this key to toggle AP Console Output"
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

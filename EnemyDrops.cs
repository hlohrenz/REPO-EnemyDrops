using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace EnemyDrops;

[BepInPlugin("HappyCats.EnemyDrops", "EnemyDrops", "1.0")]
public class EnemyDrops : BaseUnityPlugin
{
    internal static EnemyDrops Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }

    internal static ConfigEntry<float> HealthPackChance = null!;

    private void Awake()
    {
        Instance = this;

        HealthPackChance = Config.Bind("Drops", "HealthPackChance", 0.5f, "The chance for an enemy to drop a health pack. 0.0 to 1.0");
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}
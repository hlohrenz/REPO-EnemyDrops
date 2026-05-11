using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace EnemyDrops;

[HarmonyPatch(typeof(EnemyParent), nameof(EnemyParent.Despawn))]
public static class EnemyParentPatch
{
    [HarmonyPostfix]
    public static void Despawn_Postfix(EnemyParent __instance)
    {
        if (!SemiFunc.IsMasterClientOrSingleplayer())
        {
            return;
        }

        if (__instance == null || __instance.Enemy == null || __instance.Enemy.Health == null)
        {
            return;
        }

        if (__instance.Enemy.Health.healthCurrent > 0)
        {
            return;
        }

        if (Random.Range(0f, 1f) > global::EnemyDrops.EnemyDrops.HealthPackChance.Value)
        {
            return;
        }

        Item? healthPackItem = GetRandomHealthPackItem();
        if (healthPackItem == null)
        {
            global::EnemyDrops.EnemyDrops.Logger.LogWarning("No health pack item was found in the item dictionary.");
            return;
        }

        Transform spawnTransform = __instance.Enemy.CustomValuableSpawnTransform ? __instance.Enemy.CustomValuableSpawnTransform : __instance.Enemy.CenterTransform;
        if (!spawnTransform)
        {
            return;
        }

        Vector3 spawnPosition = spawnTransform.position + Vector3.up * 0.5f;
        if (SemiFunc.IsMultiplayer())
        {
            PhotonNetwork.InstantiateRoomObject(healthPackItem.prefab.ResourcePath, spawnPosition, Quaternion.identity, 0);
        }
        else
        {
            Object.Instantiate(healthPackItem.prefab.Prefab, spawnPosition, Quaternion.identity);
        }
    }

    private static Item? GetRandomHealthPackItem()
    {
        List<Item> healthPacks = new List<Item>();

        foreach (Item item in StatsManager.instance.itemDictionary.Values)
        {
            if (item != null && item.itemType == SemiFunc.itemType.healthPack)
            {
                healthPacks.Add(item);
            }
        }

        if (healthPacks.Count == 0)
        {
            return null;
        }

        return healthPacks[Random.Range(0, healthPacks.Count)];
    }
}
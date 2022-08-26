// -----------------------------------------------------------------------
// <copyright file="CategoryLimitPatch.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.Patches
{
#pragma warning disable SA1313
    using HarmonyLib;
    using InventorySystem.Configs;
    using ScpTeamDeathmatch.Configs;

    /// <summary>
    /// Patches <see cref="InventoryLimits.GetCategoryLimit(ItemCategory,ReferenceHub)"/> to implement <see cref="InventoryLimitsConfig.CategoryLimits"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetCategoryLimit), typeof(ItemCategory), typeof(ReferenceHub))]
    internal static class CategoryLimitPatch
    {
        private static bool Prefix(ItemCategory category, ref sbyte __result) => !Plugin.Instance.Config.InventoryLimits.CategoryLimits.TryGetValue(category, out __result);
    }
}
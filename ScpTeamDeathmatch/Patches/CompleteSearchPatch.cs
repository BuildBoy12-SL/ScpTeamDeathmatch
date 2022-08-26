// -----------------------------------------------------------------------
// <copyright file="CompleteSearchPatch.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.Patches
{
    using HarmonyLib;
    using Hints;
    using InventorySystem.Configs;
    using InventorySystem.Searching;

    /// <summary>
    /// Checks if the item limit hint should display.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class CompleteSearchPatch
    {
        private static void Postfix()
        {
        }

        private static void CheckItemLimitHint(ItemSearchCompletor searchCompletor)
        {
            sbyte categoryLimit = InventoryLimits.GetCategoryLimit(searchCompletor._category, searchCompletor.Hub);
            if (searchCompletor._category == ItemCategory.None || categoryLimit < 0 || searchCompletor.CategoryCount < categoryLimit)
                return;

            HintEffect[] effects = HintEffectPresets.FadeInAndOut(0.25f);
            searchCompletor.Hub.hints.Show(new TranslationHint(HintTranslations.MaxItemCategoryReached, new HintParameter[]
            {
                new ItemCategoryHintParameter(searchCompletor._category),
                new ByteHintParameter((byte)categoryLimit)
            }, effects, 1.5f));
        }
    }
}
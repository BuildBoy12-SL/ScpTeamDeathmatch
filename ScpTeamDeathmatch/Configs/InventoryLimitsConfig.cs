// -----------------------------------------------------------------------
// <copyright file="InventoryLimitsConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Mirror;

    public class InventoryLimitsConfig
    {
        private readonly SyncList<sbyte> categoryLimits = new();
        private Dictionary<ItemCategory, sbyte> rawCategoryLimits = new()
        {
            { ItemCategory.Armor, -1 },
            { ItemCategory.Grenade, 2 },
            { ItemCategory.Keycard, 3 },
            { ItemCategory.Medical, 3 },
            { ItemCategory.MicroHID, -1 },
            { ItemCategory.Radio, -1 },
            { ItemCategory.SCPItem, 3 },
            { ItemCategory.Firearm, 1 },
        };

        /// <summary>
        /// Gets or sets the limit to how many of a specific item a player can hold.
        /// </summary>
        [Description("The limit to how many of a specific item a player can hold. This takes precedence over category limits.")]
        public Dictionary<ItemType, sbyte> ItemLimits { get; set; } = new()
        {
            { ItemType.Adrenaline, 1 },
        };

        /// <summary>
        /// Gets or sets the limit to how many of items in a specific category a player can hold.
        /// </summary>
        [Description("The limit to how many of items in a specific category a player can hold.")]
        public Dictionary<ItemCategory, sbyte> CategoryLimits
        {
            get => rawCategoryLimits;
            set
            {
                rawCategoryLimits = value;
                categoryLimits.Clear();
                for (int index = 0; Enum.IsDefined(typeof(ItemCategory), (ItemCategory)index); ++index)
                {
                    ItemCategory key = (ItemCategory)index;
                    if (rawCategoryLimits.TryGetValue(key, out sbyte def) && def >= 0)
                        categoryLimits.Add(def);
                }
            }
        }

        /// <summary>
        /// Returns the parsed category limits from the <see cref="CategoryLimits"/> config.
        /// </summary>
        /// <returns>The configured category limits.</returns>
        public SyncList<sbyte> CategoryLimitsSync() => categoryLimits;
    }
}
// -----------------------------------------------------------------------
// <copyright file="CustomDamageConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomDamage.Configs
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScpTeamDeathmatch.API.Attributes;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Handles the configuration of custom damage values for ammunition.
    /// </summary>
    [Serializable]
    public class CustomDamageConfig
    {
        private Dictionary<ItemType, PropertyInfo> cachedProperties;

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunCOM15"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunCOM15 { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunE11SR"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunE11SR { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunCrossvec"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunCrossvec { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunFSP9"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunFSP9 { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunLogicer"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunLogicer { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunCOM18"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunCOM18 { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunRevolver"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunRevolver { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunAK"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunAK { get; set; }

        /// <summary>
        /// Gets or sets the configs for the ammo subtypes for the <see cref="ItemType.GunShotgun"/>.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public AmmoSubtypeConfig GunShotgun { get; set; }

        /// <summary>
        /// Gets the damage config for the firearm.
        /// </summary>
        /// <param name="firearm">The firearm to get the damage output of.</param>
        /// <returns>The firearm's ammo to damage config, or null if one is not found.</returns>
        public AmmoSubtypeConfig GetConfig(Firearm firearm)
        {
            cachedProperties ??= GenerateCache();

            if (cachedProperties.TryGetValue(firearm.Type, out PropertyInfo property))
                return property.GetValue(this) as AmmoSubtypeConfig;

            return null;
        }

        private Dictionary<ItemType, PropertyInfo> GenerateCache()
        {
            Dictionary<ItemType, PropertyInfo> cache = new();

            ItemType[] attachmentTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(AmmoSubtypeConfig))
                    continue;

                foreach (ItemType type in attachmentTypes)
                {
                    if (type.ToString().Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        cache.Add(type, property);
                        break;
                    }
                }
            }

            return cache;
        }
    }
}
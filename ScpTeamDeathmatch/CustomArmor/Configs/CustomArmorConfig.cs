// -----------------------------------------------------------------------
// <copyright file="CustomArmorConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomArmor.Configs
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using ScpTeamDeathmatch.Models;

    /// <summary>
    /// Handles the config for custom armor values.
    /// </summary>
    [Serializable]
    public class CustomArmorConfig
    {
        private Dictionary<ItemType, BodyArmorConfig> bodyArmor = new()
        {
            { ItemType.ArmorLight, new BodyArmorConfig(1f, 1f, 0, 0) },
            { ItemType.ArmorCombat, new BodyArmorConfig(1f, 1f, 0, 0) },
            { ItemType.ArmorHeavy, new BodyArmorConfig(0.8f, 1f, 0, 0) },
        };

        private Dictionary<ItemType, ConfiguredAhp> shields = new()
        {
            { ItemType.ArmorLight, new ConfiguredAhp(30f, 30f, -1f, 1f, 10f, true) },
            { ItemType.ArmorCombat, new ConfiguredAhp(40f, 40f, -1f, 1f, 10f, true) },
            { ItemType.ArmorHeavy, new ConfiguredAhp(60f, 60f, -1f, 1f, 10f, true) },
        };

        private Dictionary<ItemType, List<ConfiguredEffect>> effects = new()
        {
            {
                ItemType.ArmorLight,
                new List<ConfiguredEffect>
                {
                    new(EffectType.MovementBoost, 20, -1),
                }
            },
        };

        /// <summary>
        /// Gets or sets the armor types and their respective configured properties.
        /// </summary>
        [Description("The armor types and their respective configured properties.")]
        public Dictionary<ItemType, BodyArmorConfig> BodyArmor
        {
            get => bodyArmor;
            set => bodyArmor = StripDictionary(value);
        }

        /// <summary>
        /// Gets or sets the armor types and their respective shields.
        /// </summary>
        [Description("The armor types and their respective shields.")]
        public Dictionary<ItemType, ConfiguredAhp> Shields
        {
            get => shields;
            set => shields = StripDictionary(value);
        }

        /// <summary>
        /// Gets or sets the armor types and the effects to apply when equipped.
        /// </summary>
        [Description("The armor types and the effects to apply when equipped.")]
        public Dictionary<ItemType, List<ConfiguredEffect>> Effects
        {
            get => effects;
            set => effects = StripDictionary(value);
        }

        private static Dictionary<ItemType, T> StripDictionary<T>(Dictionary<ItemType, T> dictionary)
        {
            if (dictionary == null)
                return null;

            foreach (KeyValuePair<ItemType, T> kvp in dictionary.ToList())
            {
                if (!kvp.Key.IsArmor())
                    dictionary.Remove(kvp.Key);
            }

            return dictionary;
        }
    }
}
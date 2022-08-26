// -----------------------------------------------------------------------
// <copyright file="AmmoSubtypeConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomDamage.Configs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;
    using ScpTeamDeathmatch.CustomDamage.Enums;

    /// <summary>
    /// Handles the configuration of ammo subtypes.
    /// </summary>
    public class AmmoSubtypeConfig
    {
        private Dictionary<AmmoSubtype, PropertyInfo> cachedProperties;

        /// <summary>
        /// Gets or sets the config for ap rounds.
        /// </summary>
        public ArmorTypeConfig Ap { get; set; } = new();

        /// <summary>
        /// Gets or sets the config for default rounds.
        /// </summary>
        public ArmorTypeConfig Default { get; set; } = new();

        /// <summary>
        /// Gets or sets the config for fmj rounds.
        /// </summary>
        public ArmorTypeConfig Fmj { get; set; } = new();

        /// <summary>
        /// Gets or sets the config for jhp rounds.
        /// </summary>
        public ArmorTypeConfig Jhp { get; set; } = new();

        /// <summary>
        /// Gets the damage config for the firearm.
        /// </summary>
        /// <param name="firearm">The firearm to get the damage output of.</param>
        /// <returns>The firearm's ammo to damage config, or null if one is not found.</returns>
        public ArmorTypeConfig GetConfig(Firearm firearm)
        {
            cachedProperties ??= GenerateCache();

            AmmoSubtype ammoSubtype = AmmoSubtype.Default;
            foreach (Attachment attachment in firearm.Attachments.Where(attachment => attachment.IsEnabled))
            {
                ammoSubtype = AttachmentToSubtype(attachment.Name);
                if (ammoSubtype != AmmoSubtype.Default)
                    break;
            }

            if (cachedProperties.TryGetValue(ammoSubtype, out PropertyInfo property))
                return property.GetValue(this) as ArmorTypeConfig;

            return null;
        }

        private static AmmoSubtype AttachmentToSubtype(AttachmentName ammoSubtype)
        {
            switch (ammoSubtype)
            {
                case AttachmentName.StandardMagFMJ:
                case AttachmentName.ExtendedMagFMJ:
                case AttachmentName.DrumMagFMJ:
                case AttachmentName.LowcapMagFMJ:
                    return AmmoSubtype.Fmj;

                case AttachmentName.StandardMagJHP:
                case AttachmentName.ExtendedMagJHP:
                case AttachmentName.DrumMagJHP:
                case AttachmentName.LowcapMagJHP:
                    return AmmoSubtype.Jhp;

                case AttachmentName.StandardMagAP:
                case AttachmentName.ExtendedMagAP:
                case AttachmentName.DrumMagAP:
                case AttachmentName.LowcapMagAP:
                    return AmmoSubtype.Ap;

                default:
                    return AmmoSubtype.Default;
            }
        }

        private static string AmmoSubtypeName(AmmoSubtype ammoSubtype)
        {
            return ammoSubtype switch
            {
                AmmoSubtype.Ap => nameof(AmmoSubtype.Ap),
                AmmoSubtype.Fmj => nameof(AmmoSubtype.Fmj),
                AmmoSubtype.Jhp => nameof(AmmoSubtype.Jhp),
                _ => nameof(AmmoSubtype.Default)
            };
        }

        private Dictionary<AmmoSubtype, PropertyInfo> GenerateCache()
        {
            Dictionary<AmmoSubtype, PropertyInfo> cache = new();

            AmmoSubtype[] attachmentTypes = (AmmoSubtype[])Enum.GetValues(typeof(AmmoSubtype));
            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(ArmorTypeConfig))
                    continue;

                foreach (AmmoSubtype type in attachmentTypes)
                {
                    if (AmmoSubtypeName(type).Equals(property.Name))
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
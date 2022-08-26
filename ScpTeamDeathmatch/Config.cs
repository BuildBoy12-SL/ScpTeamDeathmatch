// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using ScpTeamDeathmatch.API.Attributes;
    using ScpTeamDeathmatch.Configs;
    using ScpTeamDeathmatch.CustomArmor.Configs;
    using ScpTeamDeathmatch.CustomDamage.Configs;
    using ScpTeamDeathmatch.Models;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    [Serializable]
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config() => Load();

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the folder containing miscellaneous config files.
        /// </summary>
        [Description("The folder containing miscellaneous config files.")]
        public string Folder { get; set; } = Path.Combine(Paths.Configs, "ScpTeamDeathmatch");

        /// <summary>
        /// Gets or sets the effects to apply to players when they spawn.
        /// </summary>
        [Description("The effects to apply to players when they spawn.")]
        public List<ConfiguredEffect> SpawnEffects { get; set; } = new()
        {
            new ConfiguredEffect(EffectType.Scp1853, 255, -1),
        };

        /// <summary>
        /// Gets or sets the configs for custom armor.
        /// </summary>
        [YamlIgnore]
        public CustomArmorConfig CustomArmor { get; set; }

        /// <summary>
        /// Gets or sets the configs for custom damage.
        /// </summary>
        [YamlIgnore]
        [NestedConfig]
        public CustomDamageConfig CustomDamage { get; set; }

        /// <summary>
        /// Gets or sets the configs for inventory limits.
        /// </summary>
        [YamlIgnore]
        public InventoryLimitsConfig InventoryLimits { get; set; }

        private static void LoadProperty(string path, PropertyInfo property, object parentClass)
        {
            try
            {
                object value = File.Exists(path)
                    ? Loader.Deserializer.Deserialize(File.ReadAllText(path), property.PropertyType)
                    : DefaultPropertyValue(property, parentClass);

                property.SetValue(parentClass, value);
                File.WriteAllText(path, Loader.Serializer.Serialize(property.GetValue(parentClass)));
            }
            catch (Exception e)
            {
                Log.Error($"Error while attempting to reload config file '{property.Name}', defaults will be loaded instead!\n{e.Message}");
                property.SetValue(parentClass, Activator.CreateInstance(property.PropertyType));
            }
        }

        private static object DefaultPropertyValue(PropertyInfo property, object parentClass)
            => property.GetValue(parentClass) ?? Activator.CreateInstance(property.PropertyType);

        private void Load()
        {
            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);

            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!Attribute.IsDefined(property, typeof(YamlIgnoreAttribute)))
                    continue;

                if (Attribute.IsDefined(property, typeof(NestedConfigAttribute)))
                {
                    LoadNested(property, this, Path.Combine(Folder, property.Name));
                    continue;
                }

                string path = Path.Combine(Folder, property.Name + ".yml");
                LoadProperty(path, property, this);
            }
        }

        private void LoadNested(PropertyInfo property, object parentType, string currentPath)
        {
            if (!Directory.Exists(currentPath))
                Directory.CreateDirectory(currentPath);

            object parentTypeInstance = Activator.CreateInstance(property.PropertyType);
            property.SetValue(parentType, parentTypeInstance);
            object value = property.GetValue(parentType);
            foreach (PropertyInfo nestedProperty in property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(nestedProperty, typeof(NestedConfigAttribute)))
                {
                    LoadNested(nestedProperty, parentTypeInstance, Path.Combine(currentPath, nestedProperty.Name));
                    continue;
                }

                string path = Path.Combine(currentPath, nestedProperty.Name + ".yml");
                LoadProperty(path, nestedProperty, value);
            }
        }
    }
}
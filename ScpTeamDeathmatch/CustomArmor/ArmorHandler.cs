// -----------------------------------------------------------------------
// <copyright file="ArmorHandler.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomArmor
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using PlayerStatsSystem;
    using ScpTeamDeathmatch.API.Events.EventArgs;
    using ScpTeamDeathmatch.CustomArmor.Configs;
    using ScpTeamDeathmatch.Models;

    /// <inheritdoc />
    public class ArmorHandler : Subscribable
    {
        private readonly Dictionary<ushort, AhpStat.AhpProcess> ahpProcesses = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmorHandler"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public ArmorHandler(Plugin plugin)
            : base(plugin)
        {
        }

        /// <inheritdoc />
        public override void Subscribe()
        {
            API.Events.Handlers.Player.ItemAdded += OnItemAdded;
            API.Events.Handlers.Player.RemovingItem += OnRemovingItem;
            PlayerStats.OnAnyPlayerDamaged += OnAnyPlayerDamaged;
        }

        /// <inheritdoc />
        public override void Unsubscribe()
        {
            API.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            API.Events.Handlers.Player.RemovingItem -= OnRemovingItem;
            PlayerStats.OnAnyPlayerDamaged -= OnAnyPlayerDamaged;
        }

        private static bool TryGetArmor(Player player, out Armor armor, Item toExclude = null)
        {
            foreach (Item item in player.Items)
            {
                if (toExclude != item && item is Armor foundArmor)
                {
                    armor = foundArmor;
                    return true;
                }
            }

            armor = null;
            return false;
        }

        private void OnItemAdded(ItemAddedEventArgs ev)
        {
            if (!TryGetArmor(ev.Player, out Armor bodyArmor))
                return;

            UpdateShield(ev.Player, bodyArmor);
            if (Plugin.Config.CustomArmor.BodyArmor.TryGetValue(bodyArmor.Type, out BodyArmorConfig config))
                config.ApplyTo(bodyArmor);

            if (Plugin.Config.CustomArmor.Effects.TryGetValue(bodyArmor.Type, out List<ConfiguredEffect> effects))
                effects.ForEach(effect => effect.Apply(ev.Player));
        }

        private void OnRemovingItem(RemovingItemEventArgs ev)
        {
            if (ev.Item is null)
                return;

            if (ahpProcesses.TryGetValue(ev.Item.Serial, out AhpStat.AhpProcess ahpProcess))
            {
                ev.Player.ReferenceHub.playerStats.GetModule<AhpStat>().ServerKillProcess(ahpProcess.KillCode);
                ahpProcesses.Remove(ev.Item.Serial);

                if (Plugin.Config.CustomArmor.Effects.TryGetValue(ev.Item.Type, out List<ConfiguredEffect> effects))
                    effects.ForEach(effect => ev.Player.DisableEffect(effect.Type));
            }

            if (TryGetArmor(ev.Player, out Armor armor, ev.Item))
                UpdateShield(ev.Player, armor);
        }

        private void OnAnyPlayerDamaged(ReferenceHub target, DamageHandlerBase handler)
        {
            Player player = Player.Get(target);
            if (!TryGetArmor(player, out Armor armor) ||
                !ahpProcesses.TryGetValue(armor.Serial, out AhpStat.AhpProcess ahpProcess) ||
                !Plugin.Config.CustomArmor.Shields.TryGetValue(armor.Type, out ConfiguredAhp configuredAhp))
                return;

            ahpProcess.SustainTime = configuredAhp.Sustain;
        }

        private void UpdateShield(Player player, Armor armor)
        {
            if (!ahpProcesses.ContainsKey(armor.Serial) && Plugin.Config.CustomArmor.Shields.TryGetValue(armor.Type, out ConfiguredAhp configuredAhp))
                ahpProcesses.Add(armor.Serial, configuredAhp.AddTo(player));
        }
    }
}
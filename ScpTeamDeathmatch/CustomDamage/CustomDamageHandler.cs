// -----------------------------------------------------------------------
// <copyright file="CustomDamageHandler.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomDamage
{
    using System.Collections.Generic;
    using System.Reflection;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using MEC;
    using PlayerStatsSystem;
    using ScpTeamDeathmatch.CustomDamage.Configs;
    using ScpTeamDeathmatch.Models;

    /// <inheritdoc />
    public class CustomDamageHandler : Subscribable
    {
        private readonly List<Player> verifyingDeath = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public CustomDamageHandler(Plugin plugin)
            : base(plugin)
        {
        }

        /// <inheritdoc />
        public override void Subscribe()
        {
            Exiled.Events.Handlers.Player.Shot += OnShot;
        }

        /// <inheritdoc />
        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Shot -= OnShot;
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (ev.Shooter is null || ev.Target is null || ev.Target.IsScp)
                return;

            if (!Server.FriendlyFire && ev.Shooter.Role.Side == ev.Target.Role.Side)
                return;

            if (ev.Shooter.CurrentItem is not Firearm firearm)
                return;

            if (Plugin.Config.CustomDamage.GetConfig(firearm) is not AmmoSubtypeConfig ammoSubtypeConfig ||
                ammoSubtypeConfig.GetConfig(firearm) is not ArmorTypeConfig ammoTypeConfig)
                return;

            float damage = ammoTypeConfig.GetDamage(ev.Target, ev.Hitbox._dmgMultiplier);
            if (damage <= 0)
                return;

            RoleType oldRole = ev.Target.Role.Type;

            ev.Damage = 0;
            UniversalDamageHandler damageHandler = new UniversalDamageHandler(damage, DeathTranslations.BulletWounds);
            ev.Target.Hurt(damageHandler.Damage, string.Format(DeathTranslations.BulletWounds.LogLabel, firearm.AmmoType.GetItemType().ToString().Substring(4)));
            if (verifyingDeath.Contains(ev.Target))
                return;

            verifyingDeath.Add(ev.Target);
            Timing.CallDelayed(0.1f, () =>
            {
                if (!ev.Target.IsDead)
                {
                    verifyingDeath.Remove(ev.Target);
                    return;
                }

                DiedEventArgs diedEv = new DiedEventArgs(ev.Target, oldRole, damageHandler);
                typeof(DiedEventArgs).GetField("<Killer>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(diedEv, ev.Shooter);
                Exiled.Events.Handlers.Player.OnDied(diedEv);
                verifyingDeath.Remove(ev.Target);
            });
        }
    }
}
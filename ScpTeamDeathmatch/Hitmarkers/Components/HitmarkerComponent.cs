// -----------------------------------------------------------------------
// <copyright file="HitmarkerComponent.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.HitMarkers.Components
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Armor;
    using UnityEngine;

    /// <summary>
    /// A component that sends damage hitmarkers to players.
    /// </summary>
    public class HitmarkerComponent : MonoBehaviour
    {
        private Player player;

        private float recentDamage;
        private ItemType lastArmorHit = ItemType.None;

        private float internalTimer;

        private void Awake()
        {
            player = Player.Get(gameObject);
            Exiled.Events.Handlers.Player.Shot += OnShot;
        }

        private void OnDestroy()
        {
            Exiled.Events.Handlers.Player.Shot -= OnShot;
        }

        private void Update()
        {
            internalTimer += Time.time;
            if (internalTimer > 0.5f)
            {
                internalTimer = 0f;
                CheckDamage();
            }
        }

        private void OnShot(ShotEventArgs ev)
        {
            if (ev.Shooter != player || ev.Target is null)
                return;

            recentDamage += ev.Damage;
            lastArmorHit = ev.Target.ReferenceHub.inventory.TryGetBodyArmor(out BodyArmor armor) ? armor.ItemTypeId : ItemType.None;
        }

        private void CheckDamage()
        {
            if (recentDamage > 0f)
            {
                recentDamage = 0f;
            }
        }
    }
}
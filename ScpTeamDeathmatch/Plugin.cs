// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem;
    using ScpTeamDeathmatch.Models;

    /// <inheritdoc />
    public class Plugin : Plugin<Config>
    {
        private readonly List<Subscribable> subscribed = new();
        private Harmony harmony;

        /// <summary>
        /// Gets a static instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc/>
        public override string Author => "Build";

        /// <inheritdoc/>
        public override string Name => "ScpTeamDeathmatch";

        /// <inheritdoc/>
        public override string Prefix => "ScpTeamDeathmatch";

        /// <inheritdoc/>
        public override Version Version { get; } = new(1, 0, 0);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new(5, 3, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            harmony = new Harmony($"scpTeamDeathmatch.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();

            InventoryExtensions.OnItemAdded += API.Events.Handlers.Player.OnItemAdded;

            Subscribe();
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Unsubscribe();

            InventoryExtensions.OnItemAdded -= API.Events.Handlers.Player.OnItemAdded;

            harmony.UnpatchAll(harmony.Id);
            harmony = null;

            Instance = null;
            base.OnDisabled();
        }

        private void Subscribe()
        {
            foreach (Type type in Assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(Subscribable)))
                    continue;

                try
                {
                    Subscribable subscribable = (Subscribable)Activator.CreateInstance(type, args: this);
                    subscribable.Subscribe();
                    subscribed.Add(subscribable);
                }
                catch (Exception e)
                {
                    Log.Error($"Error while registering the subscribable class '{type.FullName}': {e}");
                }
            }
        }

        private void Unsubscribe()
        {
            foreach (Subscribable subscribable in subscribed)
            {
                try
                {
                    subscribable.Unsubscribe();
                }
                catch (Exception e)
                {
                    Log.Error($"Error while unregistering the subscribable class '{subscribable.GetType().FullName}': {e}");
                }
            }

            subscribed.Clear();
        }
    }
}
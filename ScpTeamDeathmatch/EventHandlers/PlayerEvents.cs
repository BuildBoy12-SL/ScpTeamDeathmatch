// -----------------------------------------------------------------------
// <copyright file="PlayerEvents.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.EventHandlers
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Mirror;
    using ScpTeamDeathmatch.Models;

    /// <inheritdoc />
    public class PlayerEvents : Subscribable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerEvents"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public PlayerEvents(Plugin plugin)
            : base(plugin)
        {
        }

        /// <inheritdoc />
        public override void Subscribe()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
        }

        /// <inheritdoc />
        public override void Unsubscribe()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

        private static void SyncCategoryLimits(Player player, SyncList<sbyte> limits)
        {
            MirrorExtensions.SendFakeSyncObject(player, ServerConfigSynchronizer.Singleton.netIdentity, typeof(ServerConfigSynchronizer), (writer) =>
            {
                writer.WriteUInt64(1ul);
                writer.WriteUInt32((uint)limits.Count);
                for (int i = 0; i < limits.Count; i++)
                {
                    writer.WriteByte((byte)SyncList<byte>.Operation.OP_SET);
                    writer.WriteUInt32((uint)i);
                    writer.WriteSByte(limits[i]);
                }
            });
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            Plugin.Config.SpawnEffects.ForEach(effect => effect.Apply(ev.Player));
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            SyncCategoryLimits(ev.Player, Plugin.Config.InventoryLimits.CategoryLimitsSync());
        }
    }
}
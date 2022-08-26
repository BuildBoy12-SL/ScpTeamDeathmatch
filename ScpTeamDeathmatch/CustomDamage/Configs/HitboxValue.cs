// -----------------------------------------------------------------------
// <copyright file="HitboxValue.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomDamage.Configs
{
    using System;
    using Exiled.API.Features;

    /// <summary>
    /// Assigns damage values to different hitbox types.
    /// </summary>
    [Serializable]
    public class HitboxValue
    {
        /// <summary>
        /// Gets or sets the damage to deal to the body of a target.
        /// </summary>
        public float Body { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the damage to deal to limbs of a target.
        /// </summary>
        public float Limb { get; set; } = -1f;

        /// <summary>
        /// Gets or sets the damage to deal to the head of a target.
        /// </summary>
        public float Head { get; set; } = -1f;

        /// <summary>
        /// Gets the damage value for the corresponding armor type and hitbox.
        /// </summary>
        /// <param name="hitbox">The hitbox.</param>
        /// <returns>The damage value to use.</returns>
        public float GetValue(HitboxType hitbox)
        {
            return hitbox switch
            {
                HitboxType.Body => Body,
                HitboxType.Limb => Limb,
                HitboxType.Headshot => Head,
                _ => 0f
            };
        }
    }
}
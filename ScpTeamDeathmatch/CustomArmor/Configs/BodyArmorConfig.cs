// -----------------------------------------------------------------------
// <copyright file="BodyArmorConfig.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.CustomArmor.Configs
{
    using System;
    using Exiled.API.Features.Items;
    using UnityEngine;

    /// <summary>
    /// Handles configs for body armor.
    /// </summary>
    [Serializable]
    public class BodyArmorConfig
    {
        private float movementMultiplier;
        private float staminaMultiplier;
        private int helmetEfficacy;
        private int vestEfficacy;

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyArmorConfig"/> class.
        /// </summary>
        public BodyArmorConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyArmorConfig"/> class.
        /// </summary>
        /// <param name="movementMultiplier"><inheritdoc cref="MovementMultiplier"/></param>
        /// <param name="staminaMultiplier"><inheritdoc cref="StaminaMultiplier"/></param>
        /// <param name="helmetEfficacy"><inheritdoc cref="HelmetEfficacy"/></param>
        /// <param name="vestEfficacy"><inheritdoc cref="VestEfficacy"/></param>
        public BodyArmorConfig(float movementMultiplier, float staminaMultiplier, int helmetEfficacy, int vestEfficacy)
        {
            MovementMultiplier = movementMultiplier;
            StaminaMultiplier = staminaMultiplier;
            HelmetEfficacy = helmetEfficacy;
            VestEfficacy = vestEfficacy;
        }

        /// <summary>
        /// Gets or sets the movement speed multiplier to apply to the player.
        /// </summary>
        public float MovementMultiplier
        {
            get => movementMultiplier;
            set => movementMultiplier = Mathf.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Gets or sets the stamina usage multiplier to apply to the player.
        /// </summary>
        public float StaminaMultiplier
        {
            get => staminaMultiplier;
            set => staminaMultiplier = Mathf.Clamp(value, 1f, 2f);
        }

        /// <summary>
        /// Gets or sets the efficacy of the helmet armor.
        /// </summary>
        public int HelmetEfficacy
        {
            get => helmetEfficacy;
            set => helmetEfficacy = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// Gets or sets the efficacy of the vest armor.
        /// </summary>
        public int VestEfficacy
        {
            get => vestEfficacy;
            set => vestEfficacy = Mathf.Clamp(value, 0, 100);
        }

        /// <summary>
        /// Applies the configs to the specified body armor.
        /// </summary>
        /// <param name="armor">The body armor to apply the configs to.</param>
        public void ApplyTo(Armor armor)
        {
            armor.MovementSpeedMultiplier = MovementMultiplier;
            armor.StaminaUseMultiplier = StaminaMultiplier;
            armor.HelmetEfficacy = HelmetEfficacy;
            armor.VestEfficacy = VestEfficacy;
        }
    }
}
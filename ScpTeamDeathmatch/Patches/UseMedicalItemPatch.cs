// -----------------------------------------------------------------------
// <copyright file="UseMedicalItemPatch.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpTeamDeathmatch.Patches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using CustomPlayerEffects;
    using Exiled.API.Extensions;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerEffectsController.UseMedicalItem"/> to prevent the healing of <see cref="Config.SpawnEffects"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerEffectsController), nameof(PlayerEffectsController.UseMedicalItem))]
    internal static class UseMedicalItemPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br_S);
            Label continueLabel = (Label)newInstructions[index].operand;

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_2);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, Method(typeof(UseMedicalItemPatch), nameof(IsSpawnEffect))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool IsSpawnEffect(PlayerEffect playerEffect) => Plugin.Instance.Config.SpawnEffects.Any(effect => effect.Type.Type() == playerEffect.GetType());
    }
}
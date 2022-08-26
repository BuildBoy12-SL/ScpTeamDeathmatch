// -----------------------------------------------------------------------
// <copyright file="ValidateAnyPatch.cs" company="Build">
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
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Searching;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.ValidateAny))]
    internal static class ValidateAnyPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse) + offset;

            Label continueLabel = generator.DefineLabel();
            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(ValidateAnyPatch), nameof(ExceedsItemLimit))),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool ExceedsItemLimit(ItemSearchCompletor searchCompletor)
        {
            if (!Plugin.Instance.Config.InventoryLimits.ItemLimits.TryGetValue(searchCompletor.TargetItem.ItemTypeId, out sbyte limit))
                return false;

            Player player = Player.Get(searchCompletor.Hub);
            return player.Items.Count(item => item.Type == searchCompletor.TargetItem.ItemTypeId) >= limit;
        }
    }
}
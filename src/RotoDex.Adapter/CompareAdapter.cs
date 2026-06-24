using System;
using System.Collections.Generic;
using Roto.Core;

namespace RotoDex.Adapter;

public static class CompareAdapter
{
    public static List<string>? Compare(byte[] data1, byte[] data2)
    {
        var pk1 = EntityFormat.GetFromBytes(data1);
        var pk2 = EntityFormat.GetFromBytes(data2);

        if (pk1 == null || pk2 == null)
            return null; // Invalid files

        var diff = new List<string>();

        if (pk1.Species != pk2.Species)
            diff.Add($"Species: {pk1.Species} -> {pk2.Species}");

        if (pk1.Nickname != pk2.Nickname)
            diff.Add($"Nickname: '{pk1.Nickname}' -> '{pk2.Nickname}'");

        if (pk1.CurrentLevel != pk2.CurrentLevel)
            diff.Add($"Level: {pk1.CurrentLevel} -> {pk2.CurrentLevel}");

        if (pk1.Nature != pk2.Nature)
            diff.Add($"Nature: {pk1.Nature} -> {pk2.Nature}");

        if (pk1.Gender != pk2.Gender)
            diff.Add($"Gender: {pk1.Gender} -> {pk2.Gender}");

        if (pk1.IsShiny != pk2.IsShiny)
            diff.Add($"Shiny: {pk1.IsShiny} -> {pk2.IsShiny}");

        if (pk1.HeldItem != pk2.HeldItem)
            diff.Add($"Held Item: {pk1.HeldItem} -> {pk2.HeldItem}");

        if (pk1.Ability != pk2.Ability)
            diff.Add($"Ability: {pk1.Ability} -> {pk2.Ability}");

        CompareStats("HP IV", pk1.IV_HP, pk2.IV_HP, diff);
        CompareStats("ATK IV", pk1.IV_ATK, pk2.IV_ATK, diff);
        CompareStats("DEF IV", pk1.IV_DEF, pk2.IV_DEF, diff);
        CompareStats("SPA IV", pk1.IV_SPA, pk2.IV_SPA, diff);
        CompareStats("SPD IV", pk1.IV_SPD, pk2.IV_SPD, diff);
        CompareStats("SPE IV", pk1.IV_SPE, pk2.IV_SPE, diff);

        CompareStats("HP EV", pk1.EV_HP, pk2.EV_HP, diff);
        CompareStats("ATK EV", pk1.EV_ATK, pk2.EV_ATK, diff);
        CompareStats("DEF EV", pk1.EV_DEF, pk2.EV_DEF, diff);
        CompareStats("SPA EV", pk1.EV_SPA, pk2.EV_SPA, diff);
        CompareStats("SPD EV", pk1.EV_SPD, pk2.EV_SPD, diff);
        CompareStats("SPE EV", pk1.EV_SPE, pk2.EV_SPE, diff);

        if (pk1.Move1 != pk2.Move1 || pk1.Move2 != pk2.Move2 || pk1.Move3 != pk2.Move3 || pk1.Move4 != pk2.Move4)
        {
            diff.Add($"Moves: [{pk1.Move1}, {pk1.Move2}, {pk1.Move3}, {pk1.Move4}] -> [{pk2.Move1}, {pk2.Move2}, {pk2.Move3}, {pk2.Move4}]");
        }

        return diff;
    }

    private static void CompareStats(string name, int val1, int val2, List<string> diff)
    {
        if (val1 != val2)
            diff.Add($"{name}: {val1} -> {val2}");
    }
}

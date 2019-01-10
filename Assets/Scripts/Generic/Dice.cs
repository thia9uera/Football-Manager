using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class Dice
{
    public enum RollType
    {
        None,
        GetMax,
        DropMin,
    }

    public static int Roll(int _sides, int _amount = 1, int _rollType = 0, int _bonus = 0, int _bonusChance = 100)
    {
        int n = 0;
        int roll;
        List<int> rolls = new List<int>();

        while (n < _amount)
        {
            roll = 1 + Random.Range(0, _sides);
            if (1 + Random.Range(0, 100) < _bonusChance) roll += _bonus;
            rolls.Add(roll);
            n++;
        }


        if ((RollType)_rollType == RollType.GetMax)
        {
            return rolls.Max();
        }
        else if ((RollType)_rollType == RollType.DropMin)
        {
            rolls.Remove(rolls.Min());
            roll = 1 + Random.Range(0, _sides);
            if (1 + Random.Range(0, 100) < _bonusChance) roll += _bonus;
            rolls.Add(roll);
            return rolls.Sum();
        }
        else return rolls.Sum();
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public static class StackCalculator
    {
        private const string ChipSpritesPath = "Sprites/Chips";
        private static readonly List<int> ChipValues = new List<int>();

        static StackCalculator()
        {
            foreach (var sprite in Resources.LoadAll<Sprite>(ChipSpritesPath))
            {
                ChipValues.Add(int.Parse(sprite.name));
            }

            ChipValues.Sort((i1, i2) => i2.CompareTo(i1));
        }
        
        public static List<ChipValueAmountPair> CalculateChipDistribution(int stack)
        {
            if(stack <= 0) throw new ArgumentException("Stack must be greater than zero.");
            
            var chipDistribution = new List<ChipValueAmountPair>();

            for(int index = 0; stack > 0; index++)
            {
                while (ChipValues[index] > stack) index++;

                int amount = stack / ChipValues[index];
                chipDistribution.Add(new ChipValueAmountPair(ChipValues[index], amount));
                
                stack -= ChipValues[index] * amount;
            }
            
            chipDistribution.Reverse();
            return chipDistribution;
        }

        public class ChipValueAmountPair
        {
            public int Value { get; }
            public int Amount { get; }

            public ChipValueAmountPair(int value, int amount)
            {
                Value = value;
                Amount = amount;
            }

            public override string ToString()
            {
                return Value + " x" + Amount;
            }
        }
    }
}

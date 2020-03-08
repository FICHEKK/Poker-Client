using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Table
{
    public static class StackCalculator
    {
        private const float MinStackCoverage = 0.5f;
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

                int amount = GetAmountFor(ChipValues[index], stack);
                chipDistribution.Add(new ChipValueAmountPair(ChipValues[index], amount));
                
                stack -= ChipValues[index] * amount;
            }
            
            chipDistribution.Reverse();
            return chipDistribution;
        }
        
        private static int GetAmountFor(int chipValue, int stack)
        {
            if (chipValue == 1) return stack;

            int minAmount = (int) Math.Ceiling(stack * MinStackCoverage / chipValue);
            int maxAmount = stack / chipValue;

            return Random.Range(minAmount, maxAmount + 1);
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

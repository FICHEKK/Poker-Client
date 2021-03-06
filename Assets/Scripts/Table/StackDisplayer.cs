using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Table
{
    public class StackDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text stackValueText;
        [SerializeField] private GameObject chipsRoot;
        
        [SerializeField] private float widthSpacing;
        [SerializeField] private float heightSpacing;

        private const int StackChipCount = 10;
        private const string ChipSpritesPath = "Sprites/Chips";
        private static Dictionary<int, Sprite> _chipValueToSprite;
        
        public int Value { get; private set; }
        public Vector3 OriginalPosition { get; private set; }

        public void Awake()
        {
            OriginalPosition = transform.localPosition;
            
            if (_chipValueToSprite == null)
            {
                _chipValueToSprite = new Dictionary<int, Sprite>();
            
                foreach (var sprite in Resources.LoadAll<Sprite>(ChipSpritesPath))
                {
                    sprite.texture.filterMode = FilterMode.Point;
                    _chipValueToSprite.Add(int.Parse(sprite.name), sprite);
                }
            }
        }
        
        public void UpdateStack(int stack)
        {
            DeleteAllChips();
            Value = stack;
            
            if(stack == 0)
            {
                stackValueText.text = string.Empty;
                return;
            }

            var chipDistribution = StackCalculator.CalculateChipDistribution(stack);
            int sum = chipDistribution.Sum(pair => pair.Amount);

            int n = (int) Math.Ceiling((double) sum / StackChipCount);
            float x = n % 2 == 0 ? -(n / 2 - 0.5f) * widthSpacing : -n / 2 * widthSpacing;
            float y = 0f;
            int chipCounter = 0;

            foreach (var chipValueAmountPair in chipDistribution)
            {
                GameObject singleStackParent = new GameObject(chipValueAmountPair.Value.ToString());
                singleStackParent.transform.SetParent(chipsRoot.transform, false);

                for (int i = 0; i < chipValueAmountPair.Amount; i++)
                {
                    var chip = CreateChip(chipValueAmountPair.Value);

                    chip.transform.SetParent(singleStackParent.transform, false);
                    chip.transform.localPosition = new Vector3(x, y);

                    y += heightSpacing;
                    
                    if (++chipCounter % StackChipCount == 0)
                    {
                        x += widthSpacing;
                        y = 0f;
                    }
                }
            }

            stackValueText.text = stack.ToString();
        }

        private static GameObject CreateChip(int chipValue)
        {
            var chip = new GameObject("Chip " + chipValue);

            Image image = chip.AddComponent<Image>();
            image.sprite = _chipValueToSprite[chipValue];
            image.SetNativeSize();

            return chip;
        }
        
        private void DeleteAllChips()
        {
            foreach (Transform child in chipsRoot.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

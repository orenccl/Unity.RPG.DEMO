using GameDevTV.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField]
        DropConfig[] potentialDrops;
        [SerializeField] float[] dropChancePercentage;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;

        [System.Serializable]
        class DropConfig
        {
            public InventoryItem item;
            public float[] relativeChance;
            public int[] minNumber;
            public int[] maxNumber;
            public int GetRandomNumber(int level)
            {
                if (!item.IsStackable())
                {
                    return 1;
                }
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }

            for (int i = 0; i < GetRandomNumberIfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }

        }

        private Dropped GetRandomDrop(int level)
        {
            DropConfig dropConfig = SelectRandomItem(level);
            Dropped drop = new Dropped();
            drop.item = dropConfig.item;
            drop.number = dropConfig.GetRandomNumber(level);
            return drop;
        }

        private int GetRandomNumberIfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return Random.Range(min, max + 1);
        }

        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (var drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if(chanceTotal > randomRoll)
                {
                    return drop;
                }
            }
            return null;
        }

        private float GetTotalChance(int level)
        {
            float totalChance = 0;
            foreach (var drop in potentialDrops)
            {
                totalChance += GetByLevel(drop.relativeChance, level);
            }
            return totalChance;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if(values.Length == 0)
            {
                return default;
            }
            if (level > values.Length)
            {
                return values[values.Length - 1];
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }
    }
}
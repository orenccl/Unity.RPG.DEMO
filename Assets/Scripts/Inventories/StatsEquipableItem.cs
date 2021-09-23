using GameDevTV.Inventories;
using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName =("RPG/Inventory/Stats Equitable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField]
        Modifier[] addtiveModifiers;
        [SerializeField]
        Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            foreach (Modifier modifier in addtiveModifiers)
            {
                if (modifier.stat != stat) continue;
                yield return modifier.value;
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            foreach (Modifier modifier in percentageModifiers)
            {
                if (modifier.stat != stat) continue;
                yield return modifier.value;
            }
        }
    }
}
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    class Mana : MonoBehaviour, ISaveable
    {
        LazyValue<float> mana;
        BaseStats baseStats = null;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            mana = new LazyValue<float>(GetMaxMana);
        }

        private void Update()
        {
            if(mana.value < GetMaxMana())
            {
                mana.value += GetRegebRate() * Time.deltaTime;
                if (mana.value > GetMaxMana()) { mana.value = GetMaxMana(); }
            }
        }

        private float GetRegebRate()
        {
            return baseStats.GetStat(Stat.ManaRegenRate);
        }

        public float GetMana()
        {
            return mana.value;
        }

        public float GetMaxMana()
        {
            return baseStats.GetStat(Stat.Mana);
        }

        public bool UseMana(float manaToUse)
        {
            if(manaToUse > mana.value) { return false; }

            mana.value -= manaToUse;
            return true;
        }

        public float GetPercentage()
        {
            return 100 * (mana.value / GetMaxMana());
        }

        public object CaptureState()
        {
            return mana.value;
        }

        public void RestoreState(object state)
        {
            mana.value = (float)state;
        }
    }
}

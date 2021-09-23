using GameDevTV.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> cooldownTimers = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> initialCooldownTimes = new Dictionary<InventoryItem, float>();

        private void Update()
        {
            var keys = new List<InventoryItem>(cooldownTimers.Keys);
            foreach (InventoryItem item in keys)
            {
                cooldownTimers[item] -= Time.deltaTime;
                if(cooldownTimers[item] <= 0)
                {
                    cooldownTimers.Remove(item);
                    initialCooldownTimes.Remove(item); 
                }
            }    
        }

        public void StartCoolDown(InventoryItem item, float cooldownTime)
        {
            cooldownTimers[item] = cooldownTime;
            initialCooldownTimes[item] = cooldownTime;
        }

        public float GetTimeRemaining(InventoryItem item)
        {
            if(!cooldownTimers.ContainsKey(item)) { return 0; }

            return cooldownTimers[item];
        }

        public float GetFractionRemaining(InventoryItem item)
        {
            if(item == null) { return 0; }
            if (!cooldownTimers.ContainsKey(item)) { return 0; }

            return cooldownTimers[item] / initialCooldownTimes[item];
        }
    }
}

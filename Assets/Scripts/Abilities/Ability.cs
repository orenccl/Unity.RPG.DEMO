using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities 
{
    [CreateAssetMenu(fileName = "My Ability", menuName = ("Abilities/Ability"))]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float manaCost = 0;
        [SerializeField] float cooldownTime = 0;

        public override bool Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if(mana.GetMana() < manaCost) { return false; }

            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0) { return false; }

            AbilityData data = new AbilityData(user);

            ActionScheduler actionScheduler = user.GetComponent<ActionScheduler>();
            actionScheduler.StartAction(data);

            targetingStrategy.StartTargeting(data, () => { TargetAquired(data); });
            return true;
        }

        private void TargetAquired(AbilityData data)
        {
            if(data.IsCancelled()) { return; }

            Mana mana = data.GetUser().GetComponent<Mana>();
            if(!mana.UseMana(manaCost)) { return; }

            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCoolDown(this, cooldownTime);

            Debug.Log("TargetAquired");
            foreach (var filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }

            foreach (var effectStrategy in effectStrategies)
            {
                effectStrategy.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {
            Debug.Log("EffectFinished");
        }
    }
}
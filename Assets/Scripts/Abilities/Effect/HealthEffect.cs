using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effect
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = ("Abilities/Effect/Health"))]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] float healthChange;

        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach (var target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();
                if(health != null)
                {
                    if(healthChange < 0)
                    {
                        health.TakeDamage(data.GetUser(), -healthChange);
                    }
                    else
                    {
                        health.Heal(healthChange);
                    }
                }
            }
            finished();
        }
    }
}
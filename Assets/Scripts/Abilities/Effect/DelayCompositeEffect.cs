using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effect
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = ("Abilities/Effect/Delay Composite"))]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float dalay = 0;
        [SerializeField] EffectStrategy[] delayEffects;
        [SerializeField] bool abortIfCancelled = false;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayEffect(data, finished));
        }

        private IEnumerator DelayEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(dalay);

            if(abortIfCancelled && data.IsCancelled()) { yield break; }

            foreach (EffectStrategy effect in delayEffects)
            {
                effect.StartEffect(data, finished);
            }
        }
    }
}
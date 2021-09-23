using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effect
{
    [CreateAssetMenu(fileName = "Trigger Animation Effect", menuName = ("Abilities/Effect/Trigger Animation"))]
    class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animatorTrigger;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Animator animator = data.GetUser().GetComponent<Animator>();
            animator.SetTrigger(animatorTrigger);
            finished();
        }
    }
}

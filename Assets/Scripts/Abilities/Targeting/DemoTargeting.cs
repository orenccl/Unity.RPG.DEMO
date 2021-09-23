using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Demo Targeting", menuName = ("Abilities/Targeting/Demo"))]
    public class DemoTargeting : TargetingStrategy
    {
        public override void StartTargeting(AbilityData data, Action finished)
        {
            Debug.Log("DemoTargeting started");
            finished();
        }
    }
}
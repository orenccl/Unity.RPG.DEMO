using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = ("Abilities/Targeting/Directional"))]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] float groundOffset = 1;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            Ray ray = PlayerController.GetMouseRay();
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 1000, layerMask))
            {
                data.SetTargetedPoint(raycastHit.point + ray.direction * groundOffset / ray.direction.y);
            }
            finished();
        }
    }
}
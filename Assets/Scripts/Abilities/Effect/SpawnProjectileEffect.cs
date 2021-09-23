using RPG.Attributes;
using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effect
{
    [CreateAssetMenu(fileName = "Spawn Projectile Effect", menuName = ("Abilities/Effect/Spawn Projectile"))]
    public class SpawnProjectileEffect : EffectStrategy
    {
        [SerializeField] Projectile projectileToSpawn;
        [SerializeField] float damage;
        [SerializeField] bool isRightHand = true;
        [SerializeField] bool useTargetPoint = true;

        public override void StartEffect(AbilityData data, Action finished)
        {
            Fighter fighter = data.GetUser().GetComponent<Fighter>();
            Vector3 spawnPosition = fighter.GetHandTransform(isRightHand).position;

            if (useTargetPoint)
            {
                SpawnProjectileForTargetPoint(data, spawnPosition);
            }
            else
            {
                SpawnProjectileForTargets(data, spawnPosition);
            }
            finished();
        }

        private void SpawnProjectileForTargetPoint(AbilityData data, Vector3 spawnPosition)
        {
            Projectile projectile = Instantiate(projectileToSpawn, spawnPosition, Quaternion.identity);

            projectile.SetTarget(data.GetTargetedPoint(), damage, data.GetUser());
        }

        private void SpawnProjectileForTargets(AbilityData data, Vector3 spawnPosition)
        {
            foreach (GameObject target in data.GetTargets())
            {
                Health health = target.GetComponent<Health>();
                if (health)
                {
                    Projectile projectile = Instantiate(projectileToSpawn, spawnPosition, Quaternion.identity);
                    projectile.SetTarget(health, damage, data.GetUser());
                }
            }
        }
    }
}
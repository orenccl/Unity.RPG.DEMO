using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effect
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = ("Abilities/Effect/Spawn Target Prefab"))]
    class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] Transform prefabToSpawn;
        [SerializeField] float destroyDelay = -1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Effect(data, finished));
        }

        private IEnumerator Effect(AbilityData data, Action finished)
        {
            Transform instance = Instantiate<Transform>(prefabToSpawn, data.GetTargetedPoint(), Quaternion.identity);
            if(destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(instance.gameObject);
            }
            finished();
        }
    }
}
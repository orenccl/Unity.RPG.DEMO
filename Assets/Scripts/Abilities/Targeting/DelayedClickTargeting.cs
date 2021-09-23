using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = ("Abilities/Targeting/Delayed Click"))]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotSpot;
        [SerializeField] private Transform targetingPrefab;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float areaAffectRadius;

        Transform targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController, finished));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            playerController.enabled = false;
            if(targetingPrefabInstance == null)
            {
                targetingPrefabInstance = Instantiate(targetingPrefab);
            }
            else
            {
                targetingPrefabInstance.gameObject.SetActive(true);
            }
            targetingPrefabInstance.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);

            while (!data.IsCancelled())
            {
                // Run every frame;
                Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
                if (Physics.Raycast(PlayerController.GetMouseRay(), out RaycastHit raycastHit, 1000, layerMask))
                {
                    targetingPrefabInstance.position = raycastHit.point;

                    if (Input.GetMouseButton(0))
                    {
                        // Absorb the whole click
                        yield return new WaitWhile(() => Input.GetMouseButton(0));
                        data.SetTargetedPoint(raycastHit.point);
                        data.SetTargets(GetGameObjectInRadius(raycastHit.point));
                        break; 
                    }
                }       
                yield return null;
            }
            targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }
        }
    }
}
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Movement;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        ActionStore actionStore;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMesProjectionDistance = 1.0f;
        [SerializeField] float raycastRadius = 1.0f;
        [SerializeField] int numberOfAbilities = 6;

        bool isDraggingUI = false;

        private void Awake()
        {
            health = GetComponent<Health>();
            actionStore = GetComponent<ActionStore>();
        }

        private void Update()
        {
            if (InteractWithUI()) { return; }

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            UseAbilities();

            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }
            //print("Nothing to do!");

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }

        private void UseAbilities()
        {
            for (int i = 0; i < numberOfAbilities; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    actionStore.Use(i, gameObject);
                }
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach(RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            // Get all hits
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            // Build array distances
            float[] distances = new float[hits.Length];
            for(int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            // Sort by distance
            Array.Sort(distances, hits);

            // Return
            return hits;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out Vector3 target);

            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                SetCursor(CursorType.Movement);
                return true;
            }
            SetCursor(CursorType.None);
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMesProjectionDistance, NavMesh.AllAreas);

            if (!navMeshHit.hit) return false;

            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.ForceSoftware);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        public static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
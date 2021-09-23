using RPG.Control;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float respawnTime = 3;
        //[SerializeField] float pickupRange = 15;

        private void PickUp(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            //Destroy(gameObject);
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float secs)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(secs);
            ShowPickup(true);
        }

        private void ShowPickup(bool shouldShow)
        {
            gameObject.GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            //if (!callingController.GetComponent<Fighter>().CanPickup(gameObject)) return false;

            if (Input.GetMouseButtonDown(0))
            {
                //if(Vector3.Distance(transform.position, callingController.transform.position) < pickupRange)
                //{
                //}
                    //callingController.GetComponent<Fighter>().PickUp(gameObject);
                    PickUp(callingController.GetComponent<Fighter>());
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}

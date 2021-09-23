using GameDevTV.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class RunOverPickup : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(other.gameObject == player)
            {
                GetComponent<Pickup>().PickupItem();
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health = null;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            if (health)
            {
                GetComponent<Text>().text = String.Format("{0:0}%" + " = " + "{1:0}/{2:0}", health.GetPercentage(), health.GetHealthPoints(), health.GetMaxHealthPoints());
            }
        }
    }
}
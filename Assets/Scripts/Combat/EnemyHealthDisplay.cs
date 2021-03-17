using RPG.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health enemyHealth = null;
        Fighter player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            enemyHealth = player.GetTarget();
            if (enemyHealth)
            {
                GetComponent<Text>().text = String.Format("{0:0}%" + " = " + "{1:0}/{2:0}", enemyHealth.GetPercentage(), enemyHealth.GetHealthPoints(), enemyHealth.GetMaxHealthPoints());

            }
            else
            {
                GetComponent<Text>().text = "N/A";
            }
        }
    }
}
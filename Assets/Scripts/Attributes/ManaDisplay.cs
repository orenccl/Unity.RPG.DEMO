using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        Mana mana = null;

        private void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        private void Update()
        {
            if (mana)
            {
                GetComponent<Text>().text = String.Format("{0:0}%" + " = " + "{1:0}/{2:0}", mana.GetPercentage(), mana.GetMana(), mana.GetMaxMana());
            }
        }
    }
}
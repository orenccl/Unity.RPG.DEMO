using GameDevTV.Inventories;
using GameDevTV.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        [SerializeField] float startingBalance = 400f;

        float balance = 0;

        public event Action OnChange;

        private void Awake()
        {
            balance = startingBalance;
        }

        public float GetBlance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
            OnChange?.Invoke();
        }

        public object CaptureState()
        {
            return balance;
        }

        public void RestoreState(object state)
        {
            balance = (float)state;
        }

        public int AddItems(InventoryItem item, int number)
        {
            if(item is CurrencyItem)
            {
                UpdateBalance(item.GetPrice() * number);
                return number;
            }

            return 0;
        }
    }
}
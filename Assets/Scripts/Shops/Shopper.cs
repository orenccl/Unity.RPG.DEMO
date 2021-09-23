using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        Shop activeShop = null;

        public event Action ActiveShopChange;

        public void SetActiveShop(Shop shop)
        {
            if(activeShop != null)
            {
                activeShop.SetShopper(null);
            }

            activeShop = shop;
            if (activeShop != null)
            {
                activeShop.SetShopper(this);
            }

            ActiveShopChange?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return activeShop;
        }
    }
}
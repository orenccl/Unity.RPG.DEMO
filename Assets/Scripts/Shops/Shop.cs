using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName;
        [Range(0, 100)]
        [SerializeField] float sellingPercentage = 85f;
        [SerializeField] float maxmumBarterDiscount = 80f;

        // Shop Config
        // Item:
        // InventoryItem
        // Initial Stock
        // buyingDiscount
        [SerializeField] StockItemConfig[] stockConfig;

        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock; 
            [Range(0, 100)] public float buyingDiscountPercentage;
            public int levelToUnlock = 0;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        Shopper currentShopper = null;
        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;

        public event Action OnChange;

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems() 
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter) 
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            int shopperLevel = GetShopperLevel();

            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            foreach (InventoryItem item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) { continue; }
 
                float price = prices[item];
                int quantityInTransaction = 0;
                transaction.TryGetValue(item, out quantityInTransaction);
                int availability = availabilities[item];
                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }

        public void SelectFilter(ItemCategory category) 
        {
            filter = category;
            OnChange?.Invoke();
        }
        public ItemCategory GetFilter() { return filter; }

        public void SelectMode(bool isBuying) 
        {
            isBuyingMode = isBuying;
            OnChange?.Invoke();
        }

        public bool IsBuyingMode() 
        {
            return isBuyingMode; 
        }

        public bool CanTransact() 
        {
            // Empty transaction
            if (IsTransactionEmpty()){ return false; }
            // Not sufficient funds
            if (!HasSufficientFunds()){ return false; }
            // Not sufficient inventory space
            if (!HasInventorySpace()){ return false; }

            return true; 
        }

        public bool HasSufficientFunds()
        {
            if(!IsBuyingMode()) { return true; }
            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null)
            {
                return false;
            }

            return purse.GetBlance() >= TransactionTotal();
        }

        public bool HasInventorySpace()
        {
            if (!IsBuyingMode()) { return true; }
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if(shopperInventory == null)
            {
                return false;
            }

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public void ConfirmTransaction() 
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();

            if (shopperInventory == null || shopperPurse == null)
            {
                return;
            }
            // Transfer to or from Inventory
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (IsBuyingMode())
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }

                }
            }
            OnChange?.Invoke();
            // Debating or crediting of funds
        }

        public float TransactionTotal() 
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }
            return total; 
        }

        public void AddToTransaction(InventoryItem item, int quantity) 
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            OnChange?.Invoke();
        }

        public string GetShopName() { return shopName; }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if(shopperInventory == null) { return 0; }

            int amount = 0;
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if(item == shopperInventory.GetItemInSlot(i))
                {
                    amount += shopperInventory.GetNumberInSlot(i);
                }
            }
            return amount;
        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (IsBuyingMode())
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
            }

            return availabilities;
        }

        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach (var config in GetAvailableConfigs())
            {
                if (IsBuyingMode())
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();
                    }
                    prices[config.item] *= (1 - config.buyingDiscountPercentage / 100);
                }
                else
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * (sellingPercentage / 100);
                    }
                    prices[config.item] *= (1 - config.buyingDiscountPercentage / 100);
                }
            }

            return prices;
        }

        private float GetBarterDiscount()
        {
            BaseStats baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return (1 - Mathf.Min(discount, maxmumBarterDiscount) / 100);
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (var config in stockConfig)
            {
               if(config.levelToUnlock > shopperLevel) { continue; }

                yield return config;
            }
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBlance() < price) { return; }

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                // Remove from transaction
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item)) { stockSold[item] = 0; }
                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if(slot == -1) { return; }

            shopperInventory.RemoveFromSlot(slot, 1);
            AddToTransaction(item, -1);
            if (!stockSold.ContainsKey(item)) { stockSold[item] = 0; }
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (item == shopperInventory.GetItemInSlot(i))
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();

            if (stats == null) { return 0; }

            return stats.GetLevel();
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();

            foreach (var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }
            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach (var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;              
            }
        }
    }
}
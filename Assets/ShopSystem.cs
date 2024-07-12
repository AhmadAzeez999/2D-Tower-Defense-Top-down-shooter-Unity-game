using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItems
{
    public int itemID;
    public int itemPrice;
    public int itemLevel;
    public GameObject itemGameObject;
    public Button itemBuyBtn;
    public Button itemUpgradeBtn;
    public bool unlocked;
    public TMP_Text priceDisplay;
}

public class ShopSystem : MonoBehaviour
{
    [SerializeField] GameObject itemBoughtUI;
    [SerializeField] GameObject itemBoughtUIOverlay;
    [SerializeField] Image boughtItemImageDisplay;

    [SerializeField] List<ShopItems> shopItems = new List<ShopItems>();

    [SerializeField] ShopDialogue shopDialogue;

    [SerializeField] bool inShop = true;

    private void Start()
    {
        LoadData();

        if (!inShop)
            return;

        foreach (ShopItems shopItem in shopItems)
        {
            if (shopItem.unlocked)
            {
                shopItem.itemBuyBtn.gameObject.SetActive(false);                
            }

            if (shopItem.itemPrice <= GameManager.instance.Bones && !shopItem.unlocked)
            {
                shopItem.itemBuyBtn.interactable = true;
            }
            else
            {
                shopItem.itemBuyBtn.interactable = false;

/*                if(!shopItem.unlocked)
                    shopItem.itemUpgradeBtn.gameObject.SetActive(false);*/
            }

            shopItem.priceDisplay.text = shopItem.itemPrice.ToString();
        }
    }

    public void ItemBought(Sprite sprite)
    {
        boughtItemImageDisplay.sprite = sprite;
        itemBoughtUI.SetActive(true);
        itemBoughtUIOverlay.SetActive(true);
    }

    public void BuyItem(int itemID)
    {
        foreach (ShopItems shopItem in shopItems)
        {
            if (shopItem.itemID == itemID)
            {
                shopItem.unlocked = true;
                shopItem.itemGameObject.SetActive(true);
                shopItem.itemBuyBtn.gameObject.SetActive(false);
                //shopItem.itemUpgradeBtn.gameObject.SetActive(true);

                shopDialogue.DisplayRandomDialogue(true);

                SaveData();
            }
        }
    }

    public void UpgradeItem()
    {

    }

    public void LoadData()
    {
        foreach (ShopItems shopItem in shopItems)
        {
            if (shopItem.itemID == PlayerPrefs.GetInt("ShopItemNo" + shopItem.itemID))
            {
                shopItem.unlocked = true;
                shopItem.itemLevel = PlayerPrefs.GetInt("ShopItemNo" + shopItem.itemID + "Level");

                shopItem.unlocked = true;
                shopItem.itemGameObject.SetActive(true);

                if (shopItem.itemBuyBtn)
                {
                    shopItem.itemBuyBtn.gameObject.SetActive(false);
                    //shopItem.itemUpgradeBtn.gameObject.SetActive(true);
                }
            }
        }
    }

    public void SaveData()
    {
        foreach (ShopItems shopItem in shopItems)
        {
            if (shopItem.unlocked)
            {
                PlayerPrefs.SetInt("ShopItemNo" + shopItem.itemID, shopItem.itemID);
                PlayerPrefs.SetInt("ShopItemNo" + shopItem.itemID + "Level", shopItem.itemLevel);
            }
        }
    }
}

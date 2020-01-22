using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public int hotBarSize = 5;
    public int currentItem = 0;
    public List<Item> hotBar;

    private RectTransform slot1;
    private RectTransform slot1Border;

    private RectTransform slot2;
    private RectTransform slot2Border;

    private RectTransform slot3;
    private RectTransform slot3Border;

    private RectTransform slot4;
    private RectTransform slot4Border;

    private RectTransform slot5;
    private RectTransform slot5Border;

    private RectTransform info;
    private Text infoText;

    private Color defaultColor;

    public void Start()
    {
        slot1 = GameObject.Find("Slot1").GetComponent<RectTransform>();
        slot1Border = slot1.Find("Slot1Border").GetComponent<RectTransform>();

        slot2 = GameObject.Find("Slot2").GetComponent<RectTransform>();
        slot2Border = slot2.Find("Slot2Border").GetComponent<RectTransform>();

        slot3 = GameObject.Find("Slot3").GetComponent<RectTransform>();
        slot3Border = slot3.Find("Slot3Border").GetComponent<RectTransform>();

        slot4 = GameObject.Find("Slot4").GetComponent<RectTransform>();
        slot4Border = slot4.Find("Slot4Border").GetComponent<RectTransform>();

        slot5 = GameObject.Find("Slot5").GetComponent<RectTransform>();
        slot5Border = slot5.Find("Slot5Border").GetComponent<RectTransform>();

        info = GameObject.Find("Info").GetComponent<RectTransform>();
        infoText = info.Find("InfoText").GetComponent<Text>();
        defaultColor = GameObject.Find("Info").GetComponent<Image>().color;

        for (var i = 0; i < hotBarSize; i++)
            hotBar.Add(null);

        UpdateHotBar();
        UpdateInfo();
    }

    public void NextSpell()
    {
        currentItem++;

        if (currentItem >= hotBar.Count)
            currentItem = 0;
    }

    public void PrevSpell()
    {
        currentItem--;

        if (currentItem <= -1)
            currentItem = hotBar.Count - 1;
    }

    public void SetCurrentItem(int newItem)
    {
        currentItem = newItem;

        UpdateHotBar();
        UpdateInfo();
    }

    public void AddItemToInventory(Item newItem)
    {
        if (hotBar[currentItem] == null)
        {
            hotBar[currentItem] = newItem;
            UpdateHotBar();
            return;
        }

        for (var i = 1; i < hotBarSize; i++)
        {
            if (hotBar[i] != null) continue;
            hotBar[i] = newItem;
            UpdateHotBar();
            return;
        }

        //TODO write drop code
    }

    public void RemoveItemFromInventory(int index)
    {
        if (index < hotBarSize)
        {
            hotBar[index] = null;
            UpdateHotBar();
        }
    }

    private void UpdateHotBar()
    {
        for (var index = 0; index < hotBar.Count; index++)
        {
            var item = hotBar[index];

            if (item != null)
            {
                switch (index)
                {
                    case 0:
                        slot1.GetComponent<Image>().color = item.hotBarColor;
                        break;
                    case 1:
                        slot2.GetComponent<Image>().color = item.hotBarColor;
                        break;
                    case 2:
                        slot3.GetComponent<Image>().color = item.hotBarColor;
                        break;
                    case 3:
                        slot4.GetComponent<Image>().color = item.hotBarColor;
                        break;
                    case 4:
                        slot5.GetComponent<Image>().color = item.hotBarColor;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 0:
                        slot1.GetComponent<Image>().color = new Color(100, 100, 100, 0.2f);
                        break;
                    case 1:
                        slot2.GetComponent<Image>().color = new Color(100, 100, 100, 0.2f);
                        break;
                    case 2:
                        slot3.GetComponent<Image>().color = new Color(100, 100, 100, 0.2f);
                        break;
                    case 3:
                        slot4.GetComponent<Image>().color = new Color(100, 100, 100, 0.2f);
                        break;
                    case 4:
                        slot5.GetComponent<Image>().color = new Color(100, 100, 100, 0.2f);
                        break;
                }
            }
        }

        slot1Border.GetComponent<Image>().enabled = false;
        slot2Border.GetComponent<Image>().enabled = false;
        slot3Border.GetComponent<Image>().enabled = false;
        slot4Border.GetComponent<Image>().enabled = false;
        slot5Border.GetComponent<Image>().enabled = false;

        switch (currentItem)
        {
            case 0:
                slot1Border.GetComponent<Image>().enabled = true;
                break;
            case 1:
                slot2Border.GetComponent<Image>().enabled = true;
                break;
            case 2:
                slot3Border.GetComponent<Image>().enabled = true;
                break;
            case 3:
                slot4Border.GetComponent<Image>().enabled = true;
                break;
            case 4:
                slot5Border.GetComponent<Image>().enabled = true;
                break;
        }

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        var item = hotBar[currentItem];
        if (item != null)
        {
            info.GetComponent<Image>().color = item.hotBarColor;
            var infoString = item.itemName;

            if (item is Spell spell)
            {
                infoString += "\nDamage: ";
                infoString += spell.damage;
                infoString += "\nMana: ";
                infoString += spell.cost;
                infoString += "\nSpeed: ";
                infoString += spell.speed;
            }
            infoText.text = infoString;
        }
        else
        {
            info.GetComponent<Image>().color = defaultColor;
            infoText.text = "No spell selected";
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int inventorySize = 12;
    public int spellBookSize = 4;

    public int currentSpell;

    public List<Item> inventory;
    public List<Spell> spellBook;

    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<Item>();
        spellBook = new List<Spell>();
    }

    public void nextSpell()
    {
        currentSpell++;

        if (currentSpell <= spellBookSize)
            currentSpell = 0;
    }

    public void prevSpell()
    {
        currentSpell--;

        if (currentSpell >= -1)
            currentSpell = spellBookSize - 1;
    }

    public void setCurrentSpell(int newSpell)
    {
        currentSpell = newSpell;

        if (currentSpell <= spellBookSize)
            currentSpell = 0;
        else if (currentSpell >= -1)
            currentSpell = spellBookSize - 1;
    }

    public bool addSpellToSpellBook(Spell newSpell)
    {
        if (spellBook.Count > spellBookSize)
            return false;

        spellBook.Add(newSpell);
        return true;
    }

    public void removeSpellFromSpellBook(int index)
    {
        if (index < spellBookSize)
            spellBook.RemoveAt(index);
    }

    public bool addItemToInventory(Item newItem)
    {
        if (inventory.Count > inventorySize)
            return false;

        inventory.Add(newItem);
        return true;
    }

    public void removeItemFromInventory(int index)
    {
        if (index < inventorySize)
            inventory.RemoveAt(index);
    }
}
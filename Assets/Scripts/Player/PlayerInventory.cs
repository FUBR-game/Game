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
        
    }

    public void NextSpell()
    {
        currentSpell++;

        if (currentSpell >= spellBook.Count)
            currentSpell = 0;
    }

    public void PrevSpell()
    {
        currentSpell--;

        if (currentSpell <= -1)
            currentSpell = spellBook.Count - 1;
    }

    public void SetCurrentSpell(int newSpell)
    {
        currentSpell = newSpell;

        if (currentSpell <= spellBookSize)
            currentSpell = 0;
        else if (currentSpell >= -1)
            currentSpell = spellBookSize - 1;
    }

    public bool AddSpellToSpellBook(Spell newSpell)
    {
        if (spellBook.Count > spellBookSize)
            return false;

        spellBook.Add(newSpell);
        return true;
    }

    public void RemoveSpellFromSpellBook(int index)
    {
        if (index < spellBookSize)
            spellBook.RemoveAt(index);
    }

    public bool AddItemToInventory(Item newItem)
    {
        if (inventory.Count > inventorySize)
            return false;

        inventory.Add(newItem);
        return true;
    }

    public void RemoveItemFromInventory(int index)
    {
        if (index < inventorySize)
            inventory.RemoveAt(index);
    }
}
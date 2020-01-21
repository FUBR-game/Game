using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellManager : MonoBehaviour
{
    public List<Spell> basicSpells;
    public List<Spell> spells;

    private Spell tempSpell;
    
    public void Awake()
    {
        spells = new List<Spell>();
    }

    public int getNewSpell()
    {
        spells.Add(basicSpells[Random.Range(0, basicSpells.Count)]);
        var index = spells.Count - 1;
        spells[index].index = index;
        spells[index].damage = Random.Range(100, 1000);
        spells[index].speed = Random.Range(600, 1600);
        spells[index].CalcCost();

        Debug.Log(spells[index]);
        return index;
    }

    public Spell getSpell(int index)
    {
        return spells[index];
    }
}

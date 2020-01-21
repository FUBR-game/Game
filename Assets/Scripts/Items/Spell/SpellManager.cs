using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellManager : SpellManagerBehavior
{
    public List<Spell> basicSpells;
    public List<Spell> spells;

    public List<Spell> madeSpells;
    private int currentSpell;
        
    public void Awake()
    {
        spells = new List<Spell>();
        madeSpells = new List<Spell>();

        currentSpell = 0;
        makeSpells();
    }

    public void makeSpells()
    {
        for (int i = 0; i < 200; i++)
        {
            var spell = Instantiate(basicSpells[Random.Range(0, basicSpells.Count)], transform);
            
            spell.transform.position = new Vector3(i, -10, 0);
            spell.lifeTime = 1000000000;
            spell.gameObject.SetActive(false);
            spell.speed = 0;

            madeSpells.Add(spell);
        }
    }

    public int getNewSpell()
    {
        spells.Add(madeSpells[currentSpell]);
        currentSpell++;
        
        var index = spells.Count - 1;
        
        spells[index].index = index;
        spells[index].gameObject.SetActive(true);
        spells[index].damage = Random.Range(100, 1000);
        spells[index].speed = Random.Range(600, 1600);
        spells[index].lifeTime = 10;
        spells[index].CalcCost();

        return index;
    }

    public Spell getSpell(int index)
    {
        return spells[index];
    }
}

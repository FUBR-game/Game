﻿using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpellManager : SpellManagerBehavior
{
    public List<Spell> basicSpells;
    public List<Spell> spells;

    public List<Spell> madeSpells;
    private int currentSpell;
    public int spellSize = 200;

    private NetworkProvider networkProvider;

    public void Awake()
    {
        spells = new List<Spell>();
        madeSpells = new List<Spell>();
        networkProvider = GameObject.Find("NetworkProvider").GetComponent<NetworkProvider>();

        currentSpell = 0;
        makeSpells();
    }

    public void makeSpells()
    {
        var spellCount = 0;
        var damageMultiplier = 1;
        var speedMultiplier = 4;
        
        for (int i = 0; i < spellSize; i++)
        {
            var spell = Instantiate(basicSpells[spellCount], transform);

            spellCount++;

            if (spellCount >= basicSpells.Count)
                spellCount = 0;

            spell.transform.position = new Vector3(i, -10, 0);
            spell.lifeTime = 1000000000;
            spell.gameObject.SetActive(false);
            spell.damage = damageMultiplier * 100;
            spell.speed = speedMultiplier * 100;
            spell.CalcCost();

            madeSpells.Add(spell);
            
            if (i % basicSpells.Count == 0)
            {
                damageMultiplier++;
            }

            if (i % (basicSpells.Count * 10) == 0)
            {
                speedMultiplier++;
                damageMultiplier = 1;
            }
        }
    }

    public int getNewSpell(int number)
    {
        spells.Add(madeSpells[number]);

        var index = spells.Count - 1;

        spells[index].index = index;
        spells[index].gameObject.SetActive(true);
        spells[index].lifeTime = 10;

        return index;
    }

    public Spell getSpell(int index)
    {
        return spells[index];
    }
}
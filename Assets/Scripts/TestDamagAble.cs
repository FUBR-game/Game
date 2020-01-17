using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamagAble : MonoBehaviour, DamageAble
{
    public int health = 200;
    
    public void takeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

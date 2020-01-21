using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamagAble : MonoBehaviour, DamageAble
{
    public int maxHealth = 2000;
    private int health;

    public Gradient healthColor;
    private Renderer renderer;

    public void Start()
    {
        health = maxHealth;
        renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", healthColor.Evaluate(1.0f));
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        renderer.material.SetColor("_Color", healthColor.Evaluate((float)health / maxHealth));
    }
}

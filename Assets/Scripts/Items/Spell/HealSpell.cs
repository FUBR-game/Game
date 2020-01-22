using System;
using System.Collections;
using UnityEngine;

public class HealSpell : Spell
{
    bool hasHealed = false;

    public new void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Play();
        }

        StartCoroutine(DestroySpell(6.0f));
    }

    protected new void CollisionEffect(Collision collision)
    {
        return;
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<DamageAble>() != null && !hasHealed)
        {
            collision.gameObject.GetComponent<DamageAble>().TakeDamage(-damage);
            hasHealed = true;
        }

        StartCoroutine(DestroySpell(1.0f));
    }

    protected IEnumerator DestroySpell(float time)
    {
        yield return new WaitForSeconds(1.0f);

        hasHealed = true;

        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Stop();
        }

        Destroy(gameObject, 0.5f);
    }
}
using UnityEngine;

public class MissileSpell : Spell
{
    public new void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Play();
        }

        Destroy(gameObject, lifeTime);
    }

    protected new void CollisionEffect(Collision collision)
    {
        if (collision.gameObject.GetComponent<DamageAble>() != null)
        {
            collision.gameObject.GetComponent<DamageAble>().TakeDamage(damage);
        }

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Stop();
        }

        Destroy(gameObject, 0.3f);
    }
}
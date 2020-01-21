using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Spell : Item
{
    public int damage = 100;
    public float speed = 100;
    public int cost = 100;
    public float timeOut = 1000;
    public int lifeTime = 100;

    public ParticleSystem[] particleSystems;

    protected new Rigidbody rigidbody;

    protected bool hasWorked = false;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        foreach (var particleSystem in particleSystems)
        {
            particleSystem.Play();
        }

        Destroy(gameObject, lifeTime);
    }

    public void CalcCost()
    {
        cost = (int) (damage + speed / 2) / 10;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (hasWorked) return;

        hasWorked = true;
        CollisionEffect(collision);
    }

    protected void CollisionEffect(Collision collision)
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
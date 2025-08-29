using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicProjectile : Projectile
{

    public override void EffectOnCollision(Collider other)
    {
        DamageOrDestroy(other);
    }
    void DamageOrDestroy(Collider other)
    {
        if (TargetTag.Contains(other.tag))
        {
            other.GetComponent<Damageable>().TakeDamage(Damage);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}

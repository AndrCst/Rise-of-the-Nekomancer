using UnityEngine;
using System.Collections;

public class AbilityHandler : MonoBehaviour, IPoolable
{
    public Ability MyAbility;

    public virtual void OnSpawn()
    {
        if (MyAbility.CastTime > 0)
        {
            waitCoroutine = StartCoroutine(WaitForCast());
        }
        else
        {
            FireEffect();
        }
    }

    public virtual void OnReturn() //I wonder if I should just call this on the PoolManager?
    {

    }

    private Coroutine waitCoroutine;
    IEnumerator WaitForCast()
    {
        yield return new WaitForSeconds(MyAbility.CastTime);
        FireEffect();
    }

    public virtual void FireEffect()
    {
        OnReturn();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
    void HandleInterruption(GameObject obj)
    {
        if (obj == MyAbility.Caster.CastingObject && waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
            OnReturn();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void OnEnable()
    {
        EntityEvents.OnSpellInterrupted += HandleInterruption;
    }

    private void OnDisable()
    {
        EntityEvents.OnSpellInterrupted -= HandleInterruption;
    }
}

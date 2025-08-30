using UnityEngine;
using System.Collections;

public class AbilityHandler : MonoBehaviour, IPoolable
{
    public Ability MyAbility;
    public GameObject TargetingObj;
    public virtual void OnSpawn() //TODO: This is behaving weirdly. To be able to aim, I need it to be channeled, but channeled was technically supposed to happen always
    {

        
        if (MyAbility.CastTime > 0)
        {
            waitCoroutine = StartCoroutine(WaitForCast());

            if (TargetingObj != null)
            {
                TargetingObj.SetActive(true);
            }
        }
        else
        {
            FireEffect();
            RemoveObject();
        }
    }

    public virtual void OnReturn() //I wonder if I should just call this on the PoolManager?
    {
        if (TargetingObj != null)
        {
            TargetingObj.SetActive(false);
        }
    }

    private Coroutine waitCoroutine;
    IEnumerator WaitForCast()
    {
        yield return new WaitForSeconds(MyAbility.CastTime);
        FireEffect();
        yield return new WaitForSeconds(MyAbility.ChanneledTime);       
        RemoveObject();
    }

    public virtual void FireEffect()
    {
        
    }

    void RemoveObject()
    {
        OnReturn();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
    public virtual void HandleInterruption(GameObject obj)
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

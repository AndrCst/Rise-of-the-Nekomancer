using UnityEngine;
using System.Collections;

public class AbilityHandler : MonoBehaviour, IPoolable
{
    public Ability MyAbility;
    public GameObject TargetingObj;
    public virtual void OnSpawn() 
    {    
            WaitCoroutine = StartCoroutine(WaitForCast());

            if (TargetingObj != null)
            {
                TargetingObj.SetActive(true);
            }    
    }

    public virtual void OnReturn() 
    {
        if (TargetingObj != null)
        {
            TargetingObj.SetActive(false);
        }
    }

    public Coroutine WaitCoroutine;
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
        if (obj == MyAbility.Caster.CastingObject)
        {
            HandleStop();
        }
    }

    public void HandleStop()
    {
        if (WaitCoroutine != null)
        {
            StopCoroutine(WaitCoroutine);
            WaitCoroutine = null;
            OnReturn();
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }

    private void OnEnable()
    {
        EntityEvents.OnSpellInterrupted += HandleInterruption;
        EntityEvents.OnEntityDied += HandleInterruption;
    }

    private void OnDisable()
    {
        EntityEvents.OnSpellInterrupted -= HandleInterruption;
        EntityEvents.OnEntityDied -= HandleInterruption;
    }
}

using UnityEngine;

public class AbilityHandler : MonoBehaviour, IPoolable
{
    public Ability MyAbility;
    public MouseManager MouseManager;

    public virtual void OnSpawn()
    {

    }

    public virtual void OnReturn() //I wonder if I should just call this on the PoolManager?
    {

    }

}

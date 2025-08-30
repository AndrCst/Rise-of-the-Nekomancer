using UnityEngine;

public class ComboInstaAbility : Ability
{
    [SerializeField] private GameObject prefabOne;
    [SerializeField] private GameObject prefabTwo;
    [SerializeField] private GameObject prefabThree;

    public override void EffectSpecifics(int index)
    {
        switch(index)
        {
            case 1:
                GameObject instantiated = ObjectPoolManager.SpawnObject(prefabOne, transform.position, Quaternion.identity);

                var handler = instantiated.GetComponent<AbilityHandler>();

                handler.MyAbility = this;
                handler.OnSpawn();
                break;
            case 2:
                GameObject instantiated2 = ObjectPoolManager.SpawnObject(prefabTwo, transform.position, Quaternion.identity);

                var handler2 = instantiated2.GetComponent<AbilityHandler>();

                handler2.MyAbility = this;
                handler2.OnSpawn();
                break;
            case 3:
                GameObject instantiated3 = ObjectPoolManager.SpawnObject(prefabThree, transform.position, Quaternion.identity);

                var handler3 = instantiated3.GetComponent<AbilityHandler>();

                handler3.MyAbility = this;
                handler3.OnSpawn();
                break;
        }
    }
}

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
            ObjectPoolManager.SpawnObject(prefabOne, transform.position, Quaternion.identity); 
                break;
            case 2:
                ObjectPoolManager.SpawnObject(prefabTwo, transform.position, Quaternion.identity); 
                break;
            case 3:
                ObjectPoolManager.SpawnObject(prefabThree, transform.position, Quaternion.identity); 
                break;
        }
    }
}

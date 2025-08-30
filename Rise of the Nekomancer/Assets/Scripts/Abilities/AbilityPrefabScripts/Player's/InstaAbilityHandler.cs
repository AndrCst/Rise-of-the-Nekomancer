using UnityEngine;

public class InstaAbilityHandler : AbilityHandler
{
    [SerializeField] private GameObject effectPrefab;

    public override void FireEffect()
    {
        GameObject instantiated = ObjectPoolManager.SpawnObject(effectPrefab, transform.position, Quaternion.identity);

        
    }

    void FixedUpdate()
    {
        transform.position = MyAbility.transform.position;
    }
}

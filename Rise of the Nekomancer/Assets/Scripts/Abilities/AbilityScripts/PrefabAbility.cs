using UnityEngine;

public class PrefabAbility : Ability
{
    [SerializeField] private GameObject abPrefab;
    public override void EffectSpecifics(int index)
    {
        GameObject instantiated = ObjectPoolManager.SpawnObject(abPrefab, transform.position, Quaternion.identity);

        var handler = instantiated.GetComponent<AbilityHandler>();

        handler.MyAbility = this;
        handler.OnSpawn();

    }
}

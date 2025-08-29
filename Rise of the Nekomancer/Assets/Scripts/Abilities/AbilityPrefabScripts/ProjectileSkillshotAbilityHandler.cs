using UnityEngine;

public class ProjectileSkillshotAbilityHandler : SkillshotAbilityHandler
{
    [SerializeField] private Projectile projectilePrefab;

    public override void FireEffect()
    {
        var instantiated = ObjectPoolManager.SpawnObject(projectilePrefab, transform.position, Quaternion.identity);

        instantiated.TargetPosition = new Vector3(MouseManager.WorldMousePosition.x, transform.position.y, MouseManager.WorldMousePosition.z);

        instantiated.OnSpawn();

        base.FireEffect();
    }
}

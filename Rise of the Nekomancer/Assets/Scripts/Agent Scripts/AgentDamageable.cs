using UnityEngine;

public class AgentDamageable : Damageable
{
    [SerializeField] private AgentController controller;
    public override void HandleStartInfo()
    {
        base.HandleStartInfo();
        if (controller == null)
        {
            controller = GetComponent<AgentController>();
        }
    }

    public override void Die()
    {
        base.Die();
        controller.HandleDeath();
    }

}

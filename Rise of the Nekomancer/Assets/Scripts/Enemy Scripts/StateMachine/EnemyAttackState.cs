using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public void Enter(EnemyController agent)
    {     
    }

    public void Exit(EnemyController agent)
    {
    }

    public void Update(EnemyController agent)
    {
    }



    public EnemyStateID GetID()
    {
        return EnemyStateID.AttackState;
    }
}

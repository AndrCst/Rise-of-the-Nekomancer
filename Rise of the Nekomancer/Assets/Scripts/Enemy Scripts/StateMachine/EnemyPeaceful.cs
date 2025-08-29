using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPeacefulState : EnemyState
{
    public void Enter(EnemyController agent)
    {
    }

    public void Exit(EnemyController agent)
    {
    }

    public EnemyStateID GetID()
    {
        return EnemyStateID.PeacefulState;
    }

    public void Update(EnemyController agent)
    {

    }
}

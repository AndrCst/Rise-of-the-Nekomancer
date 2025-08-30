using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttackState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.NavAgent.enabled = false;
        agent.StartAttacking();
    }

    public void Exit(AgentController agent)
    {
        agent.NavAgent.enabled = true;
    }

    public void Update(AgentController agent)
    {
    }



    public AgentStateID GetID()
    {
        return AgentStateID.AttackState;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAttackState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.AttackEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.AttackExit();
    }

    public void Update(AgentController agent)
    {
        agent.AttackUpdate();
    }



    public AgentStateID GetID()
    {
        return AgentStateID.AttackState;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentDeathState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.DeathEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.DeathExit();
    }

    public AgentStateID GetID()
    {
        return AgentStateID.DeathState;
    }

    public void Update(AgentController agent)
    {
        agent.DeathUpdate();
    }
}

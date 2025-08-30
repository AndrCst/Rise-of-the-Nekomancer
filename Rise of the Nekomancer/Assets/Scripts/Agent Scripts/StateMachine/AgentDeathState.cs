using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentDeathState : AgentState
{
    public void Enter(AgentController agent)
    {
    }

    public void Exit(AgentController agent)
    {
    }

    public AgentStateID GetID()
    {
        return AgentStateID.DeathState;
    }

    public void Update(AgentController agent)
    {

    }
}

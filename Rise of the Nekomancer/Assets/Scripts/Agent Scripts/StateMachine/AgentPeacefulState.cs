using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPeacefulState : AgentState
{
    public void Enter(AgentController agent)
    {
    }

    public void Exit(AgentController agent)
    {
    }

    public AgentStateID GetID()
    {
        return AgentStateID.PeacefulState;
    }

    public void Update(AgentController agent)
    {

    }
}

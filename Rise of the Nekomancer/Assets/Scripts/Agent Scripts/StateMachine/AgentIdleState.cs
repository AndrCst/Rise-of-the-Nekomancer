using UnityEngine;

public class AgentIdleState : AgentState
{
    public void Enter(AgentController agent)
    {
    }

    public void Exit(AgentController agent)
    {
    }

    public void Update(AgentController agent)
    {
        agent.IdleUpdate();
    }




    public AgentStateID GetID()
    {
        return AgentStateID.IdleState;
    }
}

using UnityEngine;

public class AgentIdleState : AgentState
{
    public void Enter(AgentController agent)
    {
        agent.IdleEnter();
    }

    public void Exit(AgentController agent)
    {
        agent.IdleExit();
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

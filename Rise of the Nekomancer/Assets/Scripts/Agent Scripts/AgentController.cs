using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour, ICaster
{
    public NavMeshAgent NavAgent;
    public GameObject Target;
    public string[] DetectedStrings;
    public float DetectionRange;
    public LayerMask SearchedMask;
    private AgentStateMachine stateMachine;
    [SerializeField] private AgentStateID startingState;
    private float trackDelay = 0.1f;
    public CapsuleCollider Collider;
    public Ability CurrentAbility;
    public List<Ability> Abilities;

    //ICaster
    public bool IsCasting
    {
        get { return isCasting; }
        set { isCasting = value; }
    }
    private bool isCasting;

    public bool ShouldBeStoppingForCasting
    {
        get { return shouldStopForCasting; }
        set { shouldStopForCasting = value; }
    }
    private bool shouldStopForCasting;
    public GameObject CastingObject
    {
        get { return gameObject; }
    }

    private void Start()
    {
        GetComponents();
        GetStartInfo();
    }

    void GetStartInfo()
    {
        CurrentAbility = Abilities[0];

        foreach(Ability ability in Abilities)
        {
            ability.Caster = this;
        }
    }
    private void GetComponents()
    {
       if (NavAgent == null)
        {
            NavAgent = GetComponent<NavMeshAgent>();
        }

       if (Collider == null)
        {
            Collider = GetComponent<CapsuleCollider>();
        }

        CreateStateMachine();      
    }

    void CreateStateMachine()
    {
        stateMachine = new(this);
        stateMachine.RegisterState(new AgentPathingState());
        stateMachine.RegisterState(new AgentDeathState());
        stateMachine.RegisterState(new AgentIdleState());
        stateMachine.RegisterState(new AgentPeacefulState());
        stateMachine.RegisterState(new AgentSpawningState());
        stateMachine.RegisterState(new AgentAttackState());
        stateMachine.ChangeStates(startingState);
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
        Debug.Log(stateMachine.currentState);
    }

    public void DetectEnemy()
    {
       var targets = ProjectTools.GetValidObjectsOnRange(transform.position, DetectionRange, DetectedStrings, SearchedMask);

        if (targets.Count > 0)
        {
            Target = ProjectTools.GetNearestObject(targets, transform.position);         

            stateMachine.ChangeStates(AgentStateID.PathingState);
        }
    }

    public float GetStoppingDistance()
    {
        float targetRadius = Target.GetComponent<CapsuleCollider>().radius;

        float myRadius = Collider.radius;

        float minDistance = targetRadius + myRadius;

        if (CurrentAbility != null)
        {
            minDistance += CurrentAbility.Range;
        }

        return minDistance;
    }

    private bool CheckTargetOnRange()
    {
        if (ProjectTools.GetObjectOnRange(transform.position, Target, GetStoppingDistance()))
        {
            return true;
        }

        return false;
    }

    public void ChangeToAttack()
    {
        if (CheckTargetOnRange())
        {
            stateMachine.ChangeStates(AgentStateID.AttackState);
        }
    }

    private Coroutine attackCoroutine;
    private IEnumerator AttackingCoroutine()
    {
        CurrentAbility.Execute();
        yield return new WaitForSeconds(CurrentAbility.CastTime);
        attackCoroutine = null;
        if (Target == null)
        {
            stateMachine.ChangeStates(AgentStateID.IdleState);
        }
        else if (CheckTargetOnRange())
        {
            attackCoroutine = StartCoroutine(AttackingCoroutine());
        }
        else
        {
            stateMachine.ChangeStates(AgentStateID.PathingState);
        }
    }

    public void StartAttacking()
    {
        attackCoroutine ??= StartCoroutine(AttackingCoroutine());
    }

    public virtual void ChooseNextAbility() //Decision logic can be added here. Overwrite if you don't want random
    {
        int random = Random.Range(0, Abilities.Count);
        CurrentAbility = Abilities[random];
    }

    public void TrackTarget(Vector3 target)
    {
        trackingCoroutine??= StartCoroutine(TrackTargetCoroutine(target));
    }

    private Coroutine trackingCoroutine;



    private IEnumerator TrackTargetCoroutine(Vector3 target)
    {
        if (NavAgent.enabled)
        {
            NavAgent.SetDestination(target);
        }
        yield return new WaitForSeconds(trackDelay);
        trackingCoroutine = null;    
    }
}

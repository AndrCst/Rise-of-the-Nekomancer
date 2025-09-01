using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.iOS;

public class PlayerController : MonoBehaviour, ICaster
{
    private Vector3 movementDirection;
    public bool CanMove = true;
    private Camera mainCamera;
    [SerializeField] private Rigidbody rb;
    private float speedMultiplier = 100000f; //For linear damping 5 and angular 0.5
    public Collider Collider;
    [SerializeField] private Damageable damageable;


    public List<Ability> Abilities; //The index here is going to be important, if the game has a basic attack, 0 for exemple will always be it

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

    public GameObject CastingObject
    {
        get { return gameObject;}
    }
    

    private bool shouldStopForCasting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HandleStart();
    }

    void OnBasicAttack() //TEMP FOR TEST. Todo: Implement how they are actually going to be bestowed on player, and their bindings propperly
    {
        HandleAbilityInput(Abilities[0]);
    }

    void OnFirstAbility()
    {
        HandleAbilityInput(Abilities[1]);
    }

    void OnSecondAbility()
    {
        HandleAbilityInput(Abilities[2]);
    }

    void HandleAbilityInput(Ability ability)
    {
        ability.Caster = this;
        ability.Execute();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
    }

    void HandleComponents()
    {
        rb = rb != null ? rb : GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        
        if (Collider == null)
        {
            Collider = GetComponent<Collider>();
        }

        if (damageable == null)
        {
            damageable = GetComponent<Damageable>();
        }
    }

    void HandleStartInfo()
    {
        CanMove = true;      
    }

    void HandleStart()
    {
        HandleComponents();
        HandleStartInfo();
    }

    private void OnMove(InputValue inputValue)
    {
            Vector2 input = inputValue.Get<Vector2>();
            Vector3 camForward = mainCamera.transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = mainCamera.transform.right;
            camRight.y = 0;
            camRight.Normalize();
            movementDirection = camForward * input.y + camRight * input.x;
    }
    void MovePlayer()
    {
        if (CanMove && !shouldStopForCasting)
        {
            var adjustedDirection = new Vector3(movementDirection.x, movementDirection.y, movementDirection.z);

            rb.AddForce(damageable.MovementSpeed * speedMultiplier * Time.deltaTime * adjustedDirection, ForceMode.Force);
        }
    }
}

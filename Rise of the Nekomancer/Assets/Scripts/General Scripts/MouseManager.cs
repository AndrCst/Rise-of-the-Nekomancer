using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    private Vector3 screenPos;
    public Vector3 WorldPos;
    public Ray WorldMouseAim;
    public Vector3 WorldMousePosition;
    public GameObject HitObject;
    private InputAction mousePos;
    [SerializeField] private PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        mousePos = playerInput.actions["MousePosition"];
    }

    // Update is called once per frame
    void Update()
    {
        WorldPositionGetter();
        HandleWorldPosition();
    }

    void WorldPositionGetter()
    {
        screenPos = mousePos.ReadValue<Vector2>();
        WorldMouseAim = Camera.main.ScreenPointToRay(screenPos);
    }

   void HandleClicking(GameObject hitObject)
    { 
            if (hitObject.CompareTag("Enemy") || hitObject.CompareTag("Player") || hitObject.CompareTag("Ally"))
            {
                PlayerEvents.ClickTarget(hitObject);
                Debug.Log(hitObject.name);
            }     
    }

    void OnClick()
    {
        HandleClicking(HitObject);
    }

    void HandleWorldPosition()
    {
        if (Physics.Raycast(WorldMouseAim, out RaycastHit hit))
        {
            HitObject = hit.collider.gameObject;
            WorldMousePosition = hit.point;
        }
    }
}

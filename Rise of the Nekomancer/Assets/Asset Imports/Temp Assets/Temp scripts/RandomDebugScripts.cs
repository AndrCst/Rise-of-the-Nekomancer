using UnityEngine;

public class RandomDebugScripts : MonoBehaviour
{
    [SerializeField] private GameObject player;
    public void DebugInterruptCast()
    {
        EntityEvents.SpellInterrupted(player);
    }
}

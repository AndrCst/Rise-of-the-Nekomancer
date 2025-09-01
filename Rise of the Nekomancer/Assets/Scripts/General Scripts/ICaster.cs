using UnityEngine;

public interface ICaster 
{
    public bool ShouldBeStoppingForCasting { get; set; } //Needs to be used to stop the caster on their movement controllers if they're casting
    public bool IsCasting { get; set; }
    public GameObject CastingObject { get;}
}

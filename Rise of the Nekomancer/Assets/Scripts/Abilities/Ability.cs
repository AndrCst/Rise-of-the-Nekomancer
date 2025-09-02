using UnityEngine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

public class Ability : MonoBehaviour
{
    [Tooltip("How long after cast the ability can be used")]
    public float Cooldown;
    [Tooltip("How long after interruption the ability can be used")]
    public float InterruptedCooldown;
    [Tooltip("How long it takes to cast. Set 0 for instant")]
    public float CastTime;
    [Tooltip("If the effect happens right away on cast, or after. If 0, this won't matter")]
    public float ChanneledTime;
    public ICaster Caster;
    [Tooltip("If locks you on ground to cast during casting")]
    public bool StopsToCast;
    public bool IsOnCooldown;

    [Tooltip("If it can be interrupted doing cast")]
    public bool UnInterruptable;

    private int comboIndex = 1;
    [Tooltip("If combos (subsequent activations have different effects. Set to 1 or 0 if you don't need it")]
    public float ComboNumber;
    [Tooltip("How long after previous activation will result in a different effect")]
    public float ComboDuration;
    public CastingCanvasHandler CastingCanvasHandler;
    public Sprite AbilityImage;

    public float Range;

    /*
     How to use:
    - Create a script that's a child of this (Prefab Ability child should already handle all though, Combo Insta too, but you can mix and match)
    - Prefab Ability summons the Ability Prefab
    - It contains the AbilityHandler script, and it's children
    - AbilityHandler will do things like handling aiming, spawning projectiles or effects, etc 
    - You can use AbilityHandler's children, like SkillShot sets transform.forward = mouse, so you can aim the skill shot,
    Ground will follow the mouse on drop,
    Insta will just spawn an ability, etc
     
    - e.g: You create a "ComboAbility" script, and set 3 prefabs that are "GroundAbilityHandlers", set up combo duration to 5, is channeled, you have Xerath's ult  
    
    How this script works:
    - Controller calls Execute
    - Coroutine starts
    - Checks if the caster is already casting something else
    - Checks if it's on cooldown, if it is, interrupts casting
    - If not, continues and starts cooldown coroutine 
    - If it stops to cast, it'll change the ICaster's Stop variable to true, it needs to be included on the Controller's movement
    - If it's channeled, effect happens, then starts counting for casting (Think of Aurelion Q, Vel'Koz R). In this case, the prefab will probably also have a continuous effect
    This will be handled by the AbilityHandler, if effect begins right away or not, but either way, this still counts the logic for CastTime
    - Also if it's aimed, like V Rising casting, Xerath R, it'll begin Channeled by just showing target prefab, and the AbilityHandler will handle releasing the final effect
    - Effects does the effect, using the current combo value (Starting at 1)
    - The effect generally will be a prefab made with AbilityHandler, that'll handle aiming, firing projectiles, damaging, etc
    - Then Combo goes up
    - ComboCoroutine starts counting time, you have X seconds to cast again and gain combo benefits
    - If you fail to, the comboIndex will just revert to 1

    -Interruption checks if it's in the middle of a cast when the event happens, if it does, it stops the coroutine, resets the combo, and adds an interrupt cooldown regardless of cooldown
    This is done so you can, for example, put it the same as cooldown, so it just goes on cooldown normally, or put it less, or even more to punish, etc
     */

    private void OnEnable()
    {
        EntityEvents.OnSpellInterrupted += HandleInterruption;
    }

    private void OnDisable()
    {
        EntityEvents.OnSpellInterrupted -= HandleInterruption;
    }
    public void Execute()
    {
        if (CastingCoroutine == null)
        {
            CastingCoroutine = StartCoroutine(HandleExecution());
        }
        else
        {
            Debug.Log("Tried casting while casting");
        }
    }

    private Coroutine cooldownCoroutine;
    public Coroutine CastingCoroutine;
    IEnumerator HandleExecution()
    {

        if (Caster.IsCasting)
        {
            Debug.Log("Already casting!");
            yield break;
        }

        if (IsOnCooldown)
        {
            Debug.Log("On Cooldown!");
            yield break;
        }

        cooldownCoroutine ??= StartCoroutine(HandleCooldown(Cooldown));

            Caster.IsCasting = true;

            if (StopsToCast)
            {
                stoppingCoroutine = StartCoroutine(StopToCastCoroutine());
            }

            if (CastTime > 0)
            {

                CastingCanvasHandler.HandleSlider(CastTime, true);
            }         

            Effect();

            yield return new WaitForSeconds(CastTime);

            if (ChanneledTime > 0)
            CastingCanvasHandler.HandleSlider(ChanneledTime, false);

            yield return new WaitForSeconds(ChanneledTime);

            Caster.IsCasting = false;

            CastingCoroutine = null;
    }

    private Coroutine stoppingCoroutine;
    IEnumerator StopToCastCoroutine()
    {
        Caster.ShouldBeStoppingForCasting = true;
        yield return new WaitForSeconds(CastTime + ChanneledTime);
        Caster.ShouldBeStoppingForCasting = false;
        stoppingCoroutine = null;
    }

    IEnumerator HandleCooldown(float duration)
    {
        IsOnCooldown = true;
        futureAvailableTime = Time.time + Cooldown;
        yield return new WaitForSeconds(duration);
        IsOnCooldown = false;
        cooldownCoroutine = null;
    }


    private float futureAvailableTime;
    public float HandleRemainingTimer()
    {
        if (!IsOnCooldown)
            return 1;

        float remaining = 1 - (futureAvailableTime - Time.time) / Cooldown;
        return remaining;
    }

    void Effect() 
    {
        EffectSpecifics(comboIndex); 
      
        HandleComboCount();
        
    }

    void HandleComboCount()
    {
        if (comboCoroutine == null)
        {
            comboCoroutine = StartCoroutine(ComboBreaker());
        }
        else
        {
            StopCoroutine(comboCoroutine);
            comboCoroutine = StartCoroutine(ComboBreaker()); 
        }

        if (comboIndex < ComboNumber) 
        {
            comboIndex++;
        }
        else if (comboIndex == ComboNumber)
        {
            comboIndex = 1;
        }
    }

    private Coroutine comboCoroutine;
    IEnumerator ComboBreaker()
    {
        yield return new WaitForSeconds(ComboDuration);
        comboIndex = 1;
        comboCoroutine = null;
    }


    public virtual void EffectSpecifics(int index)
    {
    }

    void HandleInterruption(GameObject obj)
    {
        if (obj == gameObject.transform.parent.gameObject && !UnInterruptable)
        {
                if (CastingCoroutine != null)
                {
                    Debug.Log("Cast Interrupted!");
                    StopCoroutine(CastingCoroutine);
                    Caster.IsCasting = false;
                    CastingCoroutine = null;

                    if (CastingCanvasHandler.FillCoroutine != null)
                    {
                        CastingCanvasHandler.HandleCastInterruption();
                    }

                if (cooldownCoroutine != null)
                {
                    StopCoroutine(cooldownCoroutine);
                    cooldownCoroutine = null;
                    IsOnCooldown = false;
                }
                
                cooldownCoroutine = StartCoroutine(HandleCooldown(InterruptedCooldown));
                
                if (comboCoroutine != null)
                {
                    StopCoroutine(comboCoroutine);
                    comboCoroutine = null;
                    comboIndex = 1;
                }

                if (stoppingCoroutine != null)
                {
                    StopCoroutine(stoppingCoroutine);
                    stoppingCoroutine = null;
                    Caster.ShouldBeStoppingForCasting = false;
                }

                }            
        } 
    }
}
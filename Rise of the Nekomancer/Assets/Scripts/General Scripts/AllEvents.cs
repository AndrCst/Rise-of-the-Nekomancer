using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents 
{
    public static event Action OnPlayerBeginCombat;
    public static void PlayerBeginCombat() => OnPlayerBeginCombat?.Invoke();

    public static event Action<GameObject> OnClickTarget;
    public static void ClickTarget(GameObject target) => OnClickTarget?.Invoke(target);

    public static event Action<GameObject> OnHealthChange;
    public static void HealthChanged(GameObject obj) => OnHealthChange?.Invoke(obj);

    
}

public class EntityEvents
{
    public static event Action<GameObject> OnEntityDied;
    public static void EntityDied(GameObject obj) => OnEntityDied?.Invoke(obj);

    public static event Action<GameObject> OnSpellInterrupted;
    public static void SpellInterrupted(GameObject obj) => OnSpellInterrupted?.Invoke(obj);
}


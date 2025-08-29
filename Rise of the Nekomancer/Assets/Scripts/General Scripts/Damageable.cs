using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damageable : MonoBehaviour
{
    public float Health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaxHealth);
            HandleHealthSliderValues();
            if (_health <= 0)
            {
                Die();
            }
        }
    }
    [SerializeField] private float _health;
    public float MovementSpeed;
    public float MaxHealth;
    public float BaseMovementSpeed;
    public float BonusSpeedMultiplier;

    //Serialized Stuff
    [SerializeField] private Slider healthSlider;

    public void OnEnable()
    {
        HandleStartInfo();
    }
    public virtual void HandleStartInfo()
    {
       
    }

    public void RecalculateMovespeed()
    {
        MovementSpeed = BaseMovementSpeed * (1 + BonusSpeedMultiplier);
    }

    public void HandleHealthSliderValues()
    {
        healthSlider.maxValue = MaxHealth;
        healthSlider.value = Health;
    }
    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public virtual void Die()
    {
        EntityEvents.EntityDied(this.gameObject);
    }
}

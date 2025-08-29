using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBarHandler : MonoBehaviour
{

    [SerializeField] private List<Image> AbilitiesVisuals;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<Slider> CooldownVisuals;
    void Start()
    {
        if (gameManager == null)
        gameManager = GameManager.Instance;

        HandleAbilityVisuals();
    }

    // Update is called once per frame
    void Update()
    {
       HandleAbilityCooldownVisuals();
    }

    public void HandleAbilityVisuals()
    {
        for (int i = 0; i < AbilitiesVisuals.Count; i++)
        {
            var abilities = gameManager.PlayerController.Abilities;
            bool outOfIndex = abilities.Count <= i;

            if (outOfIndex)
            {
                AbilitiesVisuals[i].gameObject.SetActive(false);
                continue;
            }


            AbilitiesVisuals[i].sprite = abilities[i].AbilityImage;
        }
    }

    void HandleAbilityCooldownVisuals()
    {
        for (int i = 0; i < AbilitiesVisuals.Count; i++)
        {
            List<Ability> abilities = gameManager.PlayerController.Abilities;

            bool outOfIndex = abilities.Count <= i;

            if (!outOfIndex)
            {
                if (!abilities[i].IsOnCooldown)
                {
                    CooldownVisuals[i].gameObject.SetActive(false);
                }
                else
                {
                    CooldownVisuals[i].gameObject.SetActive(true);
                }

                CooldownVisuals[i].value = abilities[i].HandleRemainingTimer();
            }
                         
        }
    }
}

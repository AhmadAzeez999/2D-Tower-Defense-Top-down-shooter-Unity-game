using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FillStatusBar : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] GateHealth gateHealth;
    private Slider slider;

    [SerializeField] private Image fillImage;

    [SerializeField] bool isForPlayer = false;
    [SerializeField] bool isForGate = false;

    [SerializeField] TMP_Text playerHealthText;
    [SerializeField] TMP_Text gateHealthText;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isForPlayer)
        {
            if (playerHealth != null)
            {
                if (slider.value <= slider.minValue)
                {
                    fillImage.enabled = false;
                }

                if (slider.value > slider.minValue && !fillImage.enabled)
                {
                    fillImage.enabled = true;
                }

                float fillValue = playerHealth.currentHealth / playerHealth.maxHealth;
                if (fillValue <= slider.maxValue / 3)
                {
                    fillImage.color = Color.red; // crit condition
                }
                else if (fillValue > slider.maxValue / 3)
                {
                    fillImage.color = Color.red;
                }
                slider.value = fillValue;

                playerHealthText.text = playerHealth.currentHealth + " / " + playerHealth.maxHealth;
            }
        }
        else if (isForGate)
        {
            if (gateHealth != null)
            {
                if (slider.value <= slider.minValue)
                {
                    fillImage.enabled = false;
                }

                if (slider.value > slider.minValue && !fillImage.enabled)
                {
                    fillImage.enabled = true;
                }

                float fillValue = gateHealth.currentHealth / gateHealth.maxHealth;
                if (fillValue <= slider.maxValue / 3)
                {
                    fillImage.color = Color.red; // crit condition
                }
                else if (fillValue > slider.maxValue / 3)
                {
                    fillImage.color = Color.red;
                }
                slider.value = fillValue;

                gateHealthText.text = gateHealth.currentHealth + " / " + gateHealth.maxHealth;
            }
        }
    }
}

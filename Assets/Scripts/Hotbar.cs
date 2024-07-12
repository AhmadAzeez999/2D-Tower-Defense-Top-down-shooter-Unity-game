using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    [SerializeField] private List<Button> buttons = new List<Button>();

    private List<Button> activatedButtons = new List<Button>();

    Button prevButton;

    int buttonNumber = 0;

    float waitTime = 0.15f;
    float timer = 0;

    [SerializeField] PlayerModeChange playerMode;
    [SerializeField] PlayerHealth player;
    private void Start()
    {
        foreach (Button button in buttons)
        {
            if (button.gameObject.activeSelf == true)
            {
                activatedButtons.Add(button);
            }
        }

        prevButton = buttons[0];
    }

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        if (player.tempDead)
        {
            ResetBackToGun();

            return;
        }

        if (Input.mouseScrollDelta.y != 0 && timer <= 0)
        {
            timer = waitTime;
            if (prevButton) prevButton.interactable = true;

            float scroll = Input.mouseScrollDelta.y;
            buttonNumber = buttonNumber - (int)scroll;
            if (buttonNumber > activatedButtons.Count - 1)
            {
                buttonNumber = 0;
            }
            else if (buttonNumber < 0)
            {
                buttonNumber = activatedButtons.Count - 1;
            }

            activatedButtons[buttonNumber].GetComponent<HotbarButton>().isMouseClick = false;
            activatedButtons[buttonNumber].onClick.Invoke();
            activatedButtons[buttonNumber].GetComponent<HotbarButton>().isMouseClick = true;
            activatedButtons[buttonNumber].interactable = false;
            PrevButtonSet();
        }

        if (Input.GetMouseButtonDown(1) && playerMode.currentMode != PlayerModeChange.PlayerMode.Combat)
        {
            ResetBackToGun();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (prevButton) prevButton.interactable = true;

            PrevButtonSet();

            GridBuildingSystem.current.ExitBuildMode(false);
        }
    }

    public void PrevButtonSet(int number = 0, bool ifInHotbar = true)
    {
        if (ifInHotbar)
            prevButton = activatedButtons[buttonNumber];
        else
        {
            prevButton = activatedButtons[number];
            foreach (Button button in activatedButtons)
            {
                if (button != activatedButtons[number])
                {
                    button.interactable = true;
                }
            }
        }
    }

    void ResetBackToGun()
    {
        if (prevButton) prevButton.interactable = true;

        buttonNumber = 0;
        activatedButtons[buttonNumber].GetComponent<HotbarButton>().isMouseClick = false;
        activatedButtons[buttonNumber].onClick.Invoke();
        activatedButtons[buttonNumber].GetComponent<HotbarButton>().isMouseClick = true;
        activatedButtons[buttonNumber].interactable = false;
        PrevButtonSet();
    }
}

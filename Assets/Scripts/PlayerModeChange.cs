using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModeChange : MonoBehaviour
{
    public enum PlayerMode
    {
        Combat,
        Build,
        Sell
    }

    // Store the current mode of the player
    public PlayerMode currentMode;

    [SerializeField] GameObject weapon;
    [SerializeField] GameObject craftingBook;

    [SerializeField] GameObject buildingTilemap;
    [SerializeField] GameObject wallTrapsTilemap;

    [SerializeField] GameObject sellingUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMode)
        {
            case PlayerMode.Combat:
                sellingUI.SetActive(false);
                weapon.SetActive(true);
                craftingBook.SetActive(false);
                buildingTilemap.SetActive(false);
                wallTrapsTilemap.SetActive(false);

                break;
            case PlayerMode.Build:
                sellingUI.SetActive(false);
                weapon.SetActive(false);
                craftingBook.SetActive(true);
                buildingTilemap.SetActive(true);
                wallTrapsTilemap.SetActive(true);

                break;
            case PlayerMode.Sell:
                sellingUI.SetActive(true);
                weapon.SetActive(false);
                craftingBook.SetActive(true);
                buildingTilemap.SetActive(true);
                wallTrapsTilemap.SetActive(true);

                break;
            default:
                break;
        }
    }

    public void ChangeMode(PlayerMode newState)
    {
            currentMode = newState;
    }

    public void CombatMode()
    {
        if (currentMode != PlayerMode.Combat)
            currentMode = PlayerMode.Combat;
    }

    public void BuildMode()
    {
        if (currentMode != PlayerMode.Build)
            currentMode = PlayerMode.Build;
    }

    public void SellMode()
    {
        if (currentMode != PlayerMode.Sell)
            currentMode = PlayerMode.Sell;
    }
}
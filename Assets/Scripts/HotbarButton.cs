using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class HotbarButton : MonoBehaviour, IPointerClickHandler
{
    public event Action<int> OnButtonClicked;

    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text buildingCostDisplay;

    public GameObject buildingPrefab;

    private KeyCode keyCode;
    private int keyNumber;

    private Button button;

    RectTransform rectTransform;
    Vector2 ogRect;

    Hotbar hotbar;

    [Header("On Click Events")]
    [SerializeField] bool shouldExitBuildMode = true;

    PlayerModeChange playerMode;

    public UnityEvent onLeftClick;
    public bool isMouseClick = true;

    private void OnValidate()
    {
        keyNumber = transform.GetSiblingIndex() + 1;
        keyCode = KeyCode.Alpha0 + keyNumber;

        if (text == null)
            text = GetComponentInChildren<TMP_Text>();

        text.SetText(keyNumber.ToString());
        gameObject.name = "Hotbar Button " + keyNumber;
    }

    private void Awake()
    {
        button = GetComponent<Button>();

        GetComponent<Button>().onClick.AddListener(HandleClick);

        rectTransform = GetComponent<RectTransform>();
        ogRect = rectTransform.sizeDelta;

        hotbar = FindObjectOfType<Hotbar>();

        playerMode = FindObjectOfType<PlayerModeChange>();
    }

    private void Start()
    {
        if (buildingPrefab && buildingPrefab.TryGetComponent<TrapBehaviour>(out TrapBehaviour trap))
        {
            buildingCostDisplay.text = "$" + trap.trapCost;
        }
        else if (buildingPrefab && buildingPrefab.TryGetComponent<TurretBehaviour>(out TurretBehaviour turret))
        {
            buildingCostDisplay.text = "$" + turret.turretCost;
        }
        else
        {
            buildingCostDisplay.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyCode) && button.interactable)
        {
            isMouseClick = false;
            HandleClick();
            isMouseClick = true;
            button.interactable = false;
            hotbar.PrevButtonSet(keyNumber - 1, false);
        }

        if (button.interactable)
        {
            rectTransform.sizeDelta = ogRect;
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(ogRect.x + 50, ogRect.y + 50);
        }
    }

    public void HandleClick()
    {
        if (isMouseClick)
            return;

        OnButtonClicked?.Invoke(keyNumber);

        GridBuildingSystem.current.ExitBuildMode(shouldExitBuildMode);

        if (shouldExitBuildMode)
        {
            playerMode.CombatMode();
        }
        else
        {
            playerMode.BuildMode();
        }

        if (buildingPrefab != null)
        {
            GridBuildingSystem.current.InitializeWithBuilding(buildingPrefab);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isMouseClick = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public bool Placed { get; private set; }
    public BoundsInt area;

    private float startPosX;
    private float startPosY;

    PlayerModeChange playerMode;

    [SerializeField] TrapBehaviour trap;
    [SerializeField] TurretBehaviour turret;

    Vector3Int prevCellPos = new Vector3Int();

    public bool isWallTrap = false;

    SpriteRenderer sprite;
    Color ogColor;

    Collider2D coll2d;

    // Start is called before the first frame update
    void Start()
    {
        playerMode = FindObjectOfType<PlayerModeChange>();

        sprite = GetComponentInChildren<SpriteRenderer>();
        ogColor = sprite.color;

        coll2d = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Placed)
            return;

        if (turret != null || !trap.doesDamage)
            coll2d.isTrigger = true;

        if (EventSystem.current.IsPointerOverGameObject(0))
        {
            return;
        }

        Vector3 mousePos;
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        startPosX = mousePos.x - this.transform.localPosition.x;
        startPosY = mousePos.y - this.transform.localPosition.y;

        Vector3Int cellPos = GridBuildingSystem.current.gridLayout.LocalToCell(mousePos);

        if (prevCellPos != cellPos)
        {
            this.transform.localPosition = GridBuildingSystem.current.gridLayout.CellToLocalInterpolated(cellPos
            + new Vector3(0.5f, 0.5f, 0f));

            if (trap != null)
                trap.PlayPlacingAnimation();

            prevCellPos = cellPos;
        }

        GridBuildingSystem.current.FollowBuilding();
        CanBePlaced();
    }

    private void OnMouseDown()
    {
        if (playerMode.currentMode == PlayerModeChange.PlayerMode.Sell)
        {
            DestroyBuild();
        }
    }

    private void OnMouseDrag()
    {

    }

    private void OnMouseUp()
    {
        if (Placed)
            return;

        if(CanBePlaced())
        {
            Place();
            trap?.PlayPlacingAnimation();
            GridBuildingSystem.current.InitializeWithBuilding(gameObject);
        }
    }

    public void CancleBuild()
    {
        GridBuildingSystem.current.ClearArea();
        Destroy(gameObject);
    }

    public void DestroyBuild(bool wasSold = true)
    {
        if (wasSold)
        {
            if (trap)
                GameManager.instance.AddCoins(trap.trapCost);
            else if (turret)
                GameManager.instance.AddCoins(turret.turretCost);
        }

        GridBuildingSystem.current.RemoveArea(area);
        Destroy(gameObject);
    }

    #region Build Methods

    public bool CanBePlaced()
    {
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        if (trap)
        {
            if (GridBuildingSystem.current.CanTakeArea(areaTemp)
                && (GameManager.Instance.Coins >= trap.trapCost))
            {
                sprite.color = new Color(1f, 1f, 1f, 1f);
                return true;
            }
        }
        else if (turret)
        {
            if (GridBuildingSystem.current.CanTakeArea(areaTemp)
                && (GameManager.Instance.Coins >= turret.turretCost))
            {
                sprite.color = new Color(1f, 1f, 1f, 1f);
                return true;
            }
        }

        sprite.color = new Color(1f, 1f, 1f, 0.2f);

        return false;
    }

    public void Place()
    {
        if (turret != null || !trap.doesDamage)
            coll2d.isTrigger = false;

        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        Placed = true;
        if (trap)
            GameManager.Instance.RemoveCoins(trap.trapCost);
        else if (turret)
            GameManager.Instance.RemoveCoins(turret.turretCost);
        GridBuildingSystem.current.TakeArea(areaTemp);
    }

    #endregion
}

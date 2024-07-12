using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem current;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;
    public Tilemap WallTrapTilemap;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private Building temp;
    private Vector3 prevPos;
    private BoundsInt prevArea;

    [SerializeField] Button weaponItemButton;

    private PlayerModeChange playerMode;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerMode = FindObjectOfType<PlayerModeChange>();

        tileBases.Clear();

        string tilePath = "Tiles/";
        tileBases.Add(TileType.Empty, null);
        tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "white"));
        tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
    }

    // Update is called once per frame
    void Update()
    {
        if (!temp)
        {
            return;
        }

        if (!weaponItemButton.interactable)
        {
            ExitBuildMode();
        }
    }

    public void ExitBuildMode(bool enteringCombat = true)
    {
        if (playerMode.currentMode == PlayerModeChange.PlayerMode.Combat)
        {
            return;
        }
        if (temp != null)
            temp.CancleBuild();
        if (enteringCombat) playerMode.CombatMode();
    }

    #region Tilemap Management

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    #endregion

    #region Building Placement

    public void InitializeWithBuilding(GameObject building)
    {
        temp = Instantiate(building, Vector3.zero, Quaternion.identity).GetComponent<Building>();
        FollowBuilding();
    }

    public void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTilemap.SetTilesBlock(prevArea, toClear);
    }

    public void FollowBuilding()
    {
        ClearArea();

        temp.area.position = gridLayout.WorldToCell(temp.gameObject.transform.position);
        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, MainTilemap);

        TileBase[] wallTrapTilesArray = GetTilesBlock(buildingArea, WallTrapTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        if (temp.isWallTrap)
        {
            for (int i = 0; i < wallTrapTilesArray.Length; i++)
            {
                if (wallTrapTilesArray[i] == tileBases[TileType.White])
                {
                    tileArray[i] = tileBases[TileType.Green];
                }
                else
                {
                    FillTiles(tileArray, TileType.Red);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (baseArray[i] == tileBases[TileType.White])
                {
                    tileArray[i] = tileBases[TileType.Green];
                }
                else
                {
                    FillTiles(tileArray, TileType.Red);
                    break;
                }
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        TileBase[] wallTrapTilesArray = GetTilesBlock(area, WallTrapTilemap);

        if (temp.isWallTrap)
        {
            foreach (var w in wallTrapTilesArray)
            {
                if (w != tileBases[TileType.White])
                {
                    Debug.Log("Cannot place wall trap here");
                    return false;
                }
            }
        }
        else
        {
            foreach (var b in baseArray)
            {
                if (b != tileBases[TileType.White])
                {
                    Debug.Log("Cannot place here");
                    return false;
                }
            }
        }

        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, TempTilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
    }

    public void RemoveArea(BoundsInt area)
    {
        TileBase[] toClear = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(toClear, TileType.White);
        MainTilemap.SetTilesBlock(area, toClear);
    }

    #endregion
}

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}

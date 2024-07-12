using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D mainCursorTexture;
    [SerializeField] private Texture2D aimCursorTexture;
    [SerializeField] private Texture2D handCursorTexture;
    [SerializeField] private Texture2D pointCursorTexture;

    private Vector2 cursorHotspot;

    PlayerModeChange playerMode;

    // Start is called before the first frame update
    void Start()
    {
        playerMode = FindObjectOfType<PlayerModeChange>();

        cursorHotspot = new Vector2(aimCursorTexture.width / 2, aimCursorTexture.height / 2);
        Cursor.SetCursor(aimCursorTexture, cursorHotspot, CursorMode.Auto);
    }

    private void Update()
    {
        switch (playerMode.currentMode)
        {
            case PlayerModeChange.PlayerMode.Combat:
                cursorHotspot = new Vector2(aimCursorTexture.width / 2, aimCursorTexture.height / 2);
                Cursor.SetCursor(aimCursorTexture, cursorHotspot, CursorMode.Auto);

                break;
            case PlayerModeChange.PlayerMode.Build:
                cursorHotspot = new Vector2(pointCursorTexture.width / 2, pointCursorTexture.height / 2);
                Cursor.SetCursor(pointCursorTexture, cursorHotspot, CursorMode.Auto);

                break;
            case PlayerModeChange.PlayerMode.Sell:
                cursorHotspot = new Vector2(handCursorTexture.width / 2, handCursorTexture.height / 2);
                Cursor.SetCursor(handCursorTexture, cursorHotspot, CursorMode.Auto);

                break;
            default:
                cursorHotspot = new Vector2(mainCursorTexture.width / 2, mainCursorTexture.height / 2);
                Cursor.SetCursor(mainCursorTexture, cursorHotspot, CursorMode.Auto);

                break;
        }
    }
}

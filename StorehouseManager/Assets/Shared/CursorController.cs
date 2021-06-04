using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Texture2D CursorTexture;
    public CursorMode CursorMode = CursorMode.Auto;
    public Vector2 HotSpot = Vector2.zero;
    
    public void Awake()
    {
        Cursor.SetCursor(CursorTexture, HotSpot, CursorMode);
    }
}
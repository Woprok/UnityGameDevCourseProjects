using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    public int PuzzlePin = 0;
    public Vector4 PinIncrements = new Vector4(0, 0, 0, 0);

    internal SpriteRenderer Renderer;
    internal Transform Transform;

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Transform = GetComponent<Transform>();
    }
}
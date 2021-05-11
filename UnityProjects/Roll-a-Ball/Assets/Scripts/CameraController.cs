using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    private Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - Player.transform.position;
    }

    // Runs after other updates.
    private void LateUpdate()
    {
        transform.position = Player.transform.position + cameraOffset;
    }
}
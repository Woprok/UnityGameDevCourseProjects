using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRotator : MonoBehaviour
{
    public Vector3 RotationVector;

    // Update is called once per frame
    void Update()
    {
        // Rotate the game object that this script is attached to by RotationVector,
        // multiplied by deltaTime in order to make it per second rather than per frame.
        transform.Rotate(RotationVector * Time.deltaTime);
    }
}
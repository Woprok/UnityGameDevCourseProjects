using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillRotation : MonoBehaviour
{
    public GameObject ChangeTarget;
    public int Delta = 100;
    public float Degree = 50f;

    // Update is called once per frame
    void Update()
    {
        ChangeTarget.GetComponent<Transform>().Rotate(0, 0, Degree * Time.deltaTime);
    }
}

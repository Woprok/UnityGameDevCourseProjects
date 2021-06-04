using System;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class UnitPath
{
    public GameObject[] StartToEndRoute;
}

public class UnitLogic : MonoBehaviour
{
    /// <summary>
    /// Unit is at Start() moved to first.
    /// Then it will slowly advance toward the end.
    /// Once end is reached it will return.
    /// </summary>
    public UnitPath[] PathsToDestinations;

    public float UnitSpeed = 1;

    public GameObject Priest;
    public GameObject Spearman;
    public GameObject Wizard;
    public GameObject Warrior;

    private readonly Random random = new Random(DateTime.Now.Second);
    private UnitPath selectedPath = null;
    //private void ToInvisible() => GetComponent<Renderer>().enabled = true;
    //private void ToVisible() => GetComponent<Renderer>().enabled = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedPath == null)
            SelectNewPath();
        else
            FollowPath();
    }

    private void FollowPath()
    {
        if (transform.position == selectedPath.StartToEndRoute[currentTarget].transform.position)
        {
            OnPositionReached();
        }

        var next = CalculateTarget();
        transform.position = Vector3.MoveTowards(transform.position, next.Item1, next.Item2);
    }

    private int currentTarget = 0;
    private bool isGoingOnAdventure = true;
    private void OnPositionReached()
    {
        // change next target
        if (isGoingOnAdventure)
            currentTarget++;
        else
            currentTarget--;

        // reached end
        if (isGoingOnAdventure && currentTarget >= selectedPath.StartToEndRoute.Length)
        {
            isGoingOnAdventure = false;
            currentTarget = selectedPath.StartToEndRoute.Length - 1;
        }

        // reached last
        if (!isGoingOnAdventure && currentTarget < 0)
        {
            FinishPath();
        }
    }

    private (Vector3, float) CalculateTarget()
    {
        Vector3 destination = selectedPath.StartToEndRoute[currentTarget].transform.position;
        return (destination, Time.deltaTime * UnitSpeed);
    }

    private void SelectNewPath()
    {
        selectedPath = PathsToDestinations[random.Next(PathsToDestinations.Length)];
        currentTarget = 0;
        isGoingOnAdventure = true;
        //ToVisible();
    }

    private void FinishPath()
    {
        selectedPath = null;
        //ToInvisible();
    }
}

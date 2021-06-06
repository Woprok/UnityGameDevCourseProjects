using System;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

[Serializable]
public class UnitPath
{
    public GameObject[] StartToEndRoute;
    public float StartWaitTimeInSeconds = 3;
    public float DesetinationWaitTimeInSeconds = 3;
}

//TODO refactor to task based path scheduling
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

    public UnityEvent OnPathFinished;

    private readonly Random random = new Random(DateTime.Now.Second);
    private UnitPath selectedPath = null;

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
        if (isStartWaiting)
        {
            currentWaitTime += Time.deltaTime;
            if (currentWaitTime > selectedPath.StartWaitTimeInSeconds)
                isStartWaiting = false;
            else
                return;
        }

        if (transform.position == selectedPath.StartToEndRoute[currentTarget].transform.position)
        {
            OnPositionReached();
        }

        if (isDestinationWaiting)
        {
            currentWaitTime += Time.deltaTime;
            if (currentWaitTime > selectedPath.DesetinationWaitTimeInSeconds)
                isDestinationWaiting = false;
            else
                return;
        }

        if (selectedPath == null)
            return;

        var next = CalculateTarget();
        transform.position = Vector3.MoveTowards(transform.position, next.Item1, next.Item2);
    }

    private float currentWaitTime = 0;
    private int currentTarget = 0;
    private bool isGoingOnAdventure = true;
    private bool isDestinationWaiting = false;
    private bool isStartWaiting = false;
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
            isDestinationWaiting = true;
            currentWaitTime = 0;
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
        isStartWaiting = true;
        currentWaitTime = 0;
    }

    private void FinishPath()
    {
        OnPathFinished.Invoke();
        selectedPath = null;
    }
}

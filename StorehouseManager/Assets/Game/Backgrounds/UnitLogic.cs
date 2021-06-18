using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class PathTask
{
    public Transform Location;
    public float ExpectedWaitTime = 0;
    private float currentWaitTime = 0;
    public bool FinishedWaiting()
    {
        currentWaitTime += Time.deltaTime;
        return currentWaitTime >= ExpectedWaitTime;
    }

    public bool ReachedPosition(Transform caller)
    {
        return caller.position == Location.position;
    }
}

[Serializable]
public class UnitPath
{
    public GameObject[] StartToEndRoute;
    public float StartWaitTimeInSeconds = 3;
    public float DesetinationWaitTimeInSeconds = 3;

    public Queue<PathTask> ToQueue()
    {
        var queue = new Queue<PathTask>();

        queue.Enqueue(new PathTask()
        {
            Location = StartToEndRoute.First().transform,
            ExpectedWaitTime = StartWaitTimeInSeconds
        });
        foreach (GameObject gameObject in StartToEndRoute)
        {
            queue.Enqueue(new PathTask()
            {
                Location = gameObject.transform
            });
        }

        queue.Enqueue(new PathTask()
        {
            Location = StartToEndRoute.Last().transform,
            ExpectedWaitTime = DesetinationWaitTimeInSeconds
        });
        foreach (GameObject gameObject in StartToEndRoute.Reverse())
        {
            queue.Enqueue(new PathTask()
            {
                Location = gameObject.transform
            });
        }

        return queue;
    }
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

    public int KeepShown = 0;
    public GameObject Priest;
    public GameObject Spearman;
    public GameObject Wizard;
    public GameObject Warrior;

    public UnityEvent OnPathFinished;

    private static readonly Random random = new Random(DateTime.Now.Second);
    private Queue<PathTask> currentPath = new Queue<PathTask>();

    // Update is called once per frame
    void Update()
    {
        if (!currentPath.Any())
            SelectNewPath();
        else
            FollowPath();
    }

    private void FollowPath()
    {
        if (!currentPath.Peek().FinishedWaiting())
        {
            return;
        }

        MoveToTarget(currentPath.Peek().Location);

        if (!currentPath.Peek().ReachedPosition(this.transform))
        {
            return;
        }

        var nextPlace = currentPath.Dequeue();

        if (currentPath.Any())
            return;

        FinishPath();
    }

    private void FinishPath()
    {
        OnPathFinished.Invoke();
    }
    private void SelectNewPath()
    {
        currentPath = PathsToDestinations[random.Next(PathsToDestinations.Length)].ToQueue();
    }
    private void MoveToTarget(Transform location)
    {
        Vector3 destination = location.position;
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * UnitSpeed);
    }

    public void SetDisable()
    {
        Priest.SetActive(false);
        Spearman.SetActive(false);
        Wizard.SetActive(false);
        Warrior.SetActive(false);
        enabled = false;
    }

    public void SetEnabled()
    {
        enabled = true;
        switch (KeepShown)
        {
            case 0:
                Priest.SetActive(true);
                Spearman.SetActive(true);
                Wizard.SetActive(true);
                Warrior.SetActive(true);
                break;
            case 1:
                Priest.SetActive(true);
                break;
            case 2:
                Spearman.SetActive(true);
                break;
            case 3:
                Wizard.SetActive(true);
                break;
            case 4:
                Warrior.SetActive(true);
                break;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trivial puzzle solver, solution is all 1, each puzzleData carries vector of increments
/// After increment new solution value is calculated
/// Optimal code should not be needed.
/// </summary>
public class PuzzleB : MonoBehaviour
{
    private float angleBase = -72;
    public PuzzleData[] Indicators;
    public UnityEvent OnPuzzleSolveA;
    public UnityEvent OnPuzzleSolveB;
    public Vector4 State = new Vector4(0, 0, 0, 0);
    private bool aNotBlocked = true;
    private bool bNotBlocked = true;
    // 0 - false, > 1 true
    private const int MaxState = 5;
    /// <summary>
    /// Order of values to pins
    /// X-0 Z-2
    /// Y-1 W-3
    /// </summary>
    /// <param name="data">from pressure plates, we get vector of change</param>
    public void PinPressed(PuzzleData data)
    {


        foreach (PuzzleData puzzleData in Indicators)
        {
            var angle = AngleBase(data.PinIncrements[puzzleData.PuzzlePin]);
            puzzleData.Transform.Rotate(new Vector3(0, 0, 1), angle);
        }

        State += data.PinIncrements;
        State = new Vector4(State[0] % MaxState, State[1] % MaxState, State[2] % MaxState, State[3] % MaxState);

        // peak of programming right here, not copy pasted at all :P
        if (State[0] == 0 && State[1] == 0 && aNotBlocked)
        {
            OnPuzzleSolveA.Invoke();
            aNotBlocked = false;
        }
        if (State[2] == 0 && State[3] == 0 && bNotBlocked)         {
            OnPuzzleSolveB.Invoke();
            bNotBlocked = false;
        }
    }

    private void Start()
    {
        foreach (PuzzleData puzzleData in Indicators)
        {
            var angle = AngleBase(State[puzzleData.PuzzlePin]);
            puzzleData.Transform.Rotate(new Vector3(0, 0, 1), angle);
        }
    }

    private float AngleBase(float stateChange) => (angleBase * stateChange) % 360;
}
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Trivial puzzle solver, solution is all 1, each puzzleData carries vector of increments
/// After increment new solution value is calculated
/// Optimal code should not be needed.
/// </summary>
public class PuzzleA : MonoBehaviour
{
    public PuzzleData[] Indicators;
    public UnityEvent OnPuzzleSolve;
    private bool solved = false;
    private Vector4 solution = new Vector4(0,0,0,0);
    private const int maxValue = 2;
    private const int solutionValue = 4;

    public void PinPressed(PuzzleData data)
    {
        solution += data.PinIncrements;
        var result = (int)Math.Floor(solution.x % maxValue + solution.y % maxValue + solution.z % maxValue + solution.w % maxValue);
        if (result == solutionValue)
            solved = true;
    }


    void FixedUpdate()
    {
        if (solved)
        {
            foreach (PuzzleData puzzleData in Indicators)
            {
                puzzleData.Renderer.enabled = true;
            }
            OnPuzzleSolve.Invoke();
            this.enabled = false;
        }
        else
        {
            foreach (PuzzleData puzzleData in Indicators)
            {
                puzzleData.Renderer.enabled = solution[puzzleData.PuzzlePin] % maxValue != 0;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int alive;

    public bool CellState;
    public bool NextState;

    public int IsolatedTimeBeforeExtinction = 15;
    public int isolatedCounter = 0;

    public bool Extinct { get { return isolatedCounter > IsolatedTimeBeforeExtinction; } }

    private void Start()
    {
        //GetComponent<MeshRenderer>().enabled = CellState;
    }

    private void Update()
    {
        // only update visibility if the state changed
        //if (CellState != NextState)
        CellState = NextState;

        GetComponent<MeshRenderer>().enabled = CellState;
    }

    public void Toggle(bool? state = null)
    {
        NextState = state ?? !CellState;
    }

    public void Resolve(int alive)
    {
        this.alive = alive;
        NextState = CellState && alive == 2 || alive == 3;

        if (!NextState && alive == 0)
            isolatedCounter += 1;
        else
            isolatedCounter = 0;
    }
}

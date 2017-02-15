using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject CellPrefab;

    public float IterationTime;
    public float NextUpdate;

    private Dictionary<Vector3, GameObject> grid;


    void Start()
    {
        grid = new Dictionary<Vector3, GameObject>();
        NextUpdate = Time.time;
    }

    private void FixedUpdate()
    {

        if (Input.GetMouseButtonDown(0))
        {
            var pos = Round(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var target = Create(pos, true).GetComponent<Cell>();
            target.Toggle();
        }
        if(!Input.GetKey(KeyCode.Mouse2))
            TimedUpdate();
    }

    private void TimedUpdate()
    {
        if (Time.time > NextUpdate)
        {
            NextUpdate = Time.time + IterationTime;
            Iterate();
        }
    }

    private GameObject Create(Vector3 pos, bool createPotentials = false)
    {
        if (!grid.ContainsKey(pos))
            grid[pos] = Instantiate(CellPrefab, pos, Quaternion.identity, transform);
       
        var go = grid[pos];

        if (createPotentials)
        {
            foreach (var c in Helpers.Surrounding(pos))
                Create(c);
        }

        return go;
    }

    private void Iterate()
    {
        // ensure all alive cells have another surrounding it before life check
        foreach (var cell in grid.ToList())
        {
            if (cell.Value.GetComponent<Cell>().CellState)
                Create(cell.Key, true);
        }


        // check life
        foreach (var cell in grid)
        {
            var alive = new List<Cell>();

            foreach (var candidate in Helpers.Surrounding(cell.Key))
            {
                var cont = grid.ContainsKey(candidate);
                if (cont && cell.Key != candidate)
                    alive.Add(grid[candidate].GetComponent<Cell>());
            }

            var aliveCount = alive.Count(i => i.CellState);

            cell.Value.GetComponent<Cell>().Resolve(aliveCount);
        }

        // destroy any extinct
        foreach (var cell in grid.ToList())
        {
            var c = cell.Value.GetComponent<Cell>();
            if (c.Extinct)
            {
                grid.Remove(cell.Key);
                Destroy(cell.Value);
            }
        }


    }
    
    private Vector3 Round(Vector3 v)
    {
        return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), 0);
    }
}




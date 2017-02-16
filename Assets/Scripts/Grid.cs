using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public GameObject CellPrefab;
    public GameObject ResetButton;
    public GameObject SpeedSlider;
    public GameObject StatsText;

    public float IterationConstant = 4;
    public float IterationTime;
    public float NextUpdate;

    private Dictionary<Vector3, GameObject> grid; 


    private int CellsAlive { get { return grid.Count(i => i.Value.GetComponent<Cell>().CellState); } }
    private int TotalCells { get { return grid.Count; } }
    private int CellsExtinct = 0;


    void Start()
    {
        var button = ResetButton.GetComponent<Button>();
        button.onClick.AddListener(() => grid.ToList().ForEach(DestroyCell));

        var slider = SpeedSlider.GetComponent<Slider>();
        slider.onValueChanged.AddListener(value => IterationConstant = value);

        grid = new Dictionary<Vector3, GameObject>();
        NextUpdate = Time.time;
    }

    private void FixedUpdate()
    {


        var overUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(0) && !overUI)
        {
            var pos = Helpers.Round(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            var target = Create(pos, true).GetComponent<Cell>();
            target.Toggle();
        }
        if(!Input.GetKey(KeyCode.Mouse2))
            TimedUpdate();
    }

    private void LateUpdate()
    {
        var text = "Cells Alive: " + CellsAlive + "\nCells Dead:  " + (TotalCells - CellsAlive) + "\nTotal Cells: " + (CellsExtinct + TotalCells);
        StatsText.GetComponent<Text>().text = text;
    }

    private void TimedUpdate()
    {
        if (Time.time > NextUpdate)
        {
            NextUpdate = Time.time + IterationConstant;
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
        grid.Where(c => c.Value.GetComponent<Cell>().Extinct).ToList().ForEach(DestroyCell);
    }

    private void DestroyCell(KeyValuePair<Vector3, GameObject> item)
    {
        grid.Remove(item.Key);
        Destroy(item.Value);
        CellsExtinct += 1;
    }
}




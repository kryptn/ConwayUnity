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
    public GameObject SliderText;

    public float IterationConstant = 4;
    public float IterationTime;
    public float NextUpdate;

    private Dictionary<Vector3, GameObject> grid;
    private bool IterationStopped;

    private string NextTimer { get { return !IterationStopped ? ((Time.time - NextUpdate) * -1).ToString("0.00") + "s" : "Stopped"; } }
    private int CellsAlive { get { return grid.Count(i => i.Value.GetComponent<Cell>().CellState); } }
    private int TotalCells { get { return grid.Count; } }
    private int CellsExtinct = 0;
    private int Generations = 0;


    void Start()
    {
        var button = ResetButton.GetComponent<Button>();
        button.onClick.AddListener(() => grid.ToList().ForEach(DestroyCell));

        var slider = SpeedSlider.GetComponent<Slider>();
        slider.onValueChanged.AddListener(UpdateSlider);
        
        grid = new Dictionary<Vector3, GameObject>();
        NextUpdate = Time.time;

        UpdateSlider(slider.GetComponent<Slider>().value);
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
        if (!Input.GetKey(KeyCode.Mouse2) && !IterationStopped)
            TimedUpdate();
    }

    private void LateUpdate()
    {
        var text = "Next Iteration in: " + NextTimer +
               "\n\nCells Alive: " + CellsAlive + 
                 "\nCells Dead:  " + (TotalCells - CellsAlive) +
                 "\nTotal Cells: " + (CellsExtinct + TotalCells) +
                 "\nGenerations: " + Generations;
        StatsText.GetComponent<Text>().text = text;
    }

    private void TimedUpdate()
    {
        if (Time.time > NextUpdate)
        {
            NextUpdate = Time.time + IterationConstant;
            Generations += grid.Count != 0 ? 1 : 0;
            Iterate();
        }
    }

    private GameObject Create(Vector3 pos, bool createPotentials = false, bool defaultState = false)
    {
        if (!grid.ContainsKey(pos))
            grid[pos] = Instantiate(CellPrefab, pos, Quaternion.identity, transform);
       
        if (createPotentials)
        {
            foreach (var c in Helpers.Surrounding(pos))
                Create(c);
        }

        return grid[pos];
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
            var aliveCells = (from candidate in Helpers.Surrounding(cell.Key)
                where grid.ContainsKey(candidate) && cell.Key != candidate
                select grid[candidate].GetComponent<Cell>()).ToList();

            cell.Value.GetComponent<Cell>().Resolve(aliveCells.Count(i => i.CellState));
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

    private void UpdateSlider(float value)
    {
        var slider = SpeedSlider.GetComponent<Slider>();

        IterationConstant = value;
        IterationStopped = value == slider.maxValue;
        SliderText.GetComponent<Text>().text = value / slider.maxValue == 1 ? "Stopped" : value.ToString("0.00")+"s";
    }
}




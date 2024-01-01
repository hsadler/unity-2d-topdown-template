using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralEnvironmentManager : MonoBehaviour
{


    // Manages procedural generation of game environment. Currently only runs
    // during play scene bootstrapping.


    public GameObject gridLinePrefab;
    public readonly Color defaultGridColor = new(0, 0, 0, 100f / 255f);
    public readonly Color tickOnGridColor = new(255, 255, 255, 20f / 255f);

    private readonly List<GameObject> gridLines = new();


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    // INTERFACE METHODS

    public void GenerateGrid()
    {
        const float OFFSET = 0.5f;
        int playAreaLowerBound = -Mathf.FloorToInt(GameSettings.GRID_SIZE / 2);
        int playAreaUpperBound = Mathf.FloorToInt(GameSettings.GRID_SIZE / 2) + 2;
        for (int i = playAreaLowerBound; i < playAreaUpperBound; i++)
        {
            this.CreateYGridLine(
                lowerBound: playAreaLowerBound - OFFSET,
                upperBound: playAreaUpperBound - (OFFSET + 1),
                xAxisPos: i - OFFSET
            );
            this.CreateXGridLine(
                lowerBound: playAreaLowerBound - OFFSET,
                upperBound: playAreaUpperBound - (OFFSET + 1),
                yAxisPos: i - OFFSET
            );
        }
    }

    public void SetGridColor(Color color)
    {
        foreach (GameObject gridLine in this.gridLines)
        {
            LineRenderer lr = gridLine.GetComponent<LineRenderer>();
            lr.startColor = color;
            lr.endColor = color;
        }
    }

    public bool IsPositionValid(Vector3 position)
    {
        int playAreaLowerBound = -(GameSettings.GRID_SIZE / 2);
        int playAreaUpperBound = (GameSettings.GRID_SIZE / 2) + 1;
        return position.x >= playAreaLowerBound && position.x < playAreaUpperBound
            && position.y >= playAreaLowerBound && position.y < playAreaUpperBound;
    }

    // IMPL METHODS

    private void CreateYGridLine(float lowerBound, float upperBound, float xAxisPos)
    {
        GameObject yGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        this.gridLines.Add(yGridLine);
        LineRenderer yLr = yGridLine.GetComponent<LineRenderer>();
        var yPoints = new Vector3[2];
        yPoints[0] = new Vector3(xAxisPos, lowerBound, 0);
        yPoints[1] = new Vector3(xAxisPos, upperBound, 0);
        yLr.SetPositions(yPoints);
    }

    private void CreateXGridLine(float lowerBound, float upperBound, float yAxisPos)
    {
        GameObject xGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        this.gridLines.Add(xGridLine);
        LineRenderer xLr = xGridLine.GetComponent<LineRenderer>();
        var xPoints = new Vector3[2];
        xPoints[0] = new Vector3(lowerBound, yAxisPos, 0);
        xPoints[1] = new Vector3(upperBound, yAxisPos, 0);
        xLr.SetPositions(xPoints);
    }


}
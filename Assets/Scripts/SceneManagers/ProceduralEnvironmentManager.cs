using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralEnvironmentManager : MonoBehaviour
{


    // Manages procedural generation of game environment. Currently only runs
    // during play scene bootstrapping.


    public GameObject gridLinePrefab;


    // UNITY HOOKS

    void Awake() { }

    void Start() { }

    void Update() { }

    // INTF METHODS

    public void GenerateGrid()
    {
        const float OFFSET = 0.5f;
        int galaxyLowerBound = -(GameSettings.GRID_SIZE / 2);
        int galaxyUpperBound = (GameSettings.GRID_SIZE / 2);
        for (int i = 0; i < galaxyUpperBound + 1; i++)
        {
            this.CreateYGridLine(galaxyLowerBound - OFFSET, galaxyUpperBound - OFFSET, i - OFFSET);
            this.CreateXGridLine(galaxyLowerBound - OFFSET, galaxyUpperBound - OFFSET, i - OFFSET);
            if (i > 0)
            {
                this.CreateYGridLine(galaxyLowerBound - OFFSET, galaxyUpperBound - OFFSET, -i - OFFSET);
                this.CreateXGridLine(galaxyLowerBound - OFFSET, galaxyUpperBound - OFFSET, -i - OFFSET);
            }
        }
    }

    // IMPL METHODS

    private void CreateYGridLine(float lowerBound, float upperBound, float xAxisPos)
    {
        GameObject yGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer yLr = yGridLine.GetComponent<LineRenderer>();
        var yPoints = new Vector3[2];
        yPoints[0] = new Vector3(xAxisPos, lowerBound, 0);
        yPoints[1] = new Vector3(xAxisPos, upperBound, 0);
        yLr.SetPositions(yPoints);
    }
    private void CreateXGridLine(float lowerBound, float upperBound, float yAxisPos)
    {
        GameObject xGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer xLr = xGridLine.GetComponent<LineRenderer>();
        var xPoints = new Vector3[2];
        xPoints[0] = new Vector3(lowerBound, yAxisPos, 0);
        xPoints[1] = new Vector3(upperBound, yAxisPos, 0);
        xLr.SetPositions(xPoints);
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleSceneManager : MonoBehaviour
{


    // prefabs
    public GameObject gridLinePrefab;

    // MonoBehaviour manager components
    public PlayerInput playerInput;

    public GameObject MenuGO;

    // the static reference to the singleton instance
    public static SampleSceneManager instance;


    // UNITY HOOKS

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        this.MenuGO.SetActive(false);
        this.GenerateGrid();
    }

    void Update()
    {
        this.CheckPlayerInput();
    }

    // INTERFACE METHODS

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // IMPLEMENTATION METHODS

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(ConstPlayerInput.MENU_KEY))
        {
            this.MenuGO.SetActive(!this.MenuGO.activeSelf);
        }
    }

    // game grid
    private void GenerateGrid()
    {
        int galaxyLowerBound = -(GameSettings.GRID_SIZE / 2);
        int galaxyUpperBound = (GameSettings.GRID_SIZE / 2);
        for (int i = 0; i < galaxyUpperBound; i++)
        {
            this.CreateYGridLine(galaxyLowerBound, galaxyUpperBound, i);
            this.CreateXGridLine(galaxyLowerBound, galaxyUpperBound, i);
            if (i > 0)
            {
                this.CreateYGridLine(galaxyLowerBound, galaxyUpperBound, -i);
                this.CreateXGridLine(galaxyLowerBound, galaxyUpperBound, -i);
            }
        }
    }
    // grid line creation helpers
    private void CreateYGridLine(int lowerBound, int upperBound, int xAxisPos)
    {
        GameObject yGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer yLr = yGridLine.GetComponent<LineRenderer>();
        var yPoints = new Vector3[2];
        yPoints[0] = new Vector3(xAxisPos, lowerBound, 0);
        yPoints[1] = new Vector3(xAxisPos, upperBound, 0);
        yLr.SetPositions(yPoints);
    }
    private void CreateXGridLine(int lowerBound, int upperBound, int yAxisPos)
    {
        GameObject xGridLine = Instantiate(this.gridLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer xLr = xGridLine.GetComponent<LineRenderer>();
        var xPoints = new Vector3[2];
        xPoints[0] = new Vector3(lowerBound, yAxisPos, 0);
        xPoints[1] = new Vector3(upperBound, yAxisPos, 0);
        xLr.SetPositions(xPoints);
    }


}

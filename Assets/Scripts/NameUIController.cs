using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Text lapDisplay;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRend;
    CheckpointManager cpManager;
    float storeTotalTime = 10000;

    public int carRepo { get; private set; }
    bool repoSet = false;
    bool lapsTimeSet = false;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!RaceMonitor.racing) { canvasGroup.alpha = 0; return; }
        if(!repoSet)
        {
            carRepo = Leaderboard.RegisterCar(playerName.text, RaceMonitor.totalLaps);
            repoSet = true;
            return;
        }
        if (carRend == null) return;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView ? 1 : 0;
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.5f);

        if (cpManager == null)
            cpManager = target.GetComponent<CheckpointManager>();

        if (!lapsTimeSet)
            Leaderboard.SetPosition(carRepo, cpManager.lap, storeTotalTime, cpManager.checkPoint, cpManager.timeEntered, RaceMonitor.totalLaps);
        //string position = Leaderboard.GetPosition(carRepo);

        if (cpManager.win == false)
        {
            lapDisplay.text = "Lap: " + cpManager.lap + "/" + RaceMonitor.totalLaps;
        }
        else
        {
            lapDisplay.text = "Finish";
            if(!lapsTimeSet)
            {
                storeTotalTime = cpManager.totalTime;
                Leaderboard.SetPosition(carRepo, cpManager.lap, storeTotalTime, cpManager.checkPoint, cpManager.timeEntered, RaceMonitor.totalLaps);
                for (int i = 0; i < RaceMonitor.totalLaps; i++)
                {
                    Leaderboard.SetLapTime(carRepo, i, cpManager.lapsTime[i]);
                }
                Debug.Log(playerName.text + " / total / " + cpManager.totalTime);
                lapsTimeSet = true;
            }
        }
    }
}

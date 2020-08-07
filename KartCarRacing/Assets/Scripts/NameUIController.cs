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

    int carRepo;
    bool repoSet = false;

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
            carRepo = Leaderboard.RegisterCar(playerName.text);
            repoSet = true;
            return;
        }
        if (carRend == null) return;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView ? 1 : 0;
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.2f);

        if (cpManager == null)
            cpManager = target.GetComponent<CheckpointManager>();

        Leaderboard.SetPosition(carRepo, cpManager.lap, cpManager.win, cpManager.checkPoint, cpManager.timeEntered);
        string position = Leaderboard.GetPosition(carRepo);

        if (cpManager.win == 0)
            lapDisplay.text = "Lap: " + cpManager.lap + "/" + RaceMonitor.totalLaps; //+ "(CP: " + cpManager.checkPoint + ")"
        else
            lapDisplay.text = "Finish";
    }
}

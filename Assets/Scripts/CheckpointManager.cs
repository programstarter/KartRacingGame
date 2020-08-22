using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int lap = 0;
    public float lapTime { get; private set; } = 0;
    public float[] lapsTime { get; private set; }
    public float totalTime { get; private set; } = 0;
    public bool win { get; private set; } = false;

    public int checkPoint = -1;
    //int checkPointCount;
    //int nextCheckPoint;
    public float timeEntered = 0;

    public GameObject lastCP;

    public bool IsWrongWay { get; private set; } = false;
    int DriveWrongWayCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] cps = GameObject.FindGameObjectsWithTag("CheckPoint");
        int checkPointSpawnStart = cps.Length - 3;
        foreach(GameObject c in cps)
        {
            if(c.name == cps[checkPointSpawnStart].name)
            {
                lastCP = c;
                break;
            }
        }
        lapsTime = new float[RaceMonitor.totalLaps];
        Debug.Log(lapsTime.Length);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "CheckPoint")
        {
            int thisCPNumber = int.Parse(other.gameObject.name);
            /*if(thisCPNumber == nextCheckPoint)
            {
                lastCP = other.gameObject;
                checkPoint = thisCPNumber;
                if (checkPoint == 0) lap++;

                nextCheckPoint++;
                if (nextCheckPoint >= checkPointCount)
                    nextCheckPoint = 0;
            }*/
            timeEntered = Time.time;

            if(thisCPNumber != 0)
            {
                if(thisCPNumber >= checkPoint)
                {
                    DriveWrongWayCount = 0;
                    IsWrongWay = false;
                }
                else
                {
                    DriveWrongWayCount++;
                    if (DriveWrongWayCount >= 2)
                        IsWrongWay = true;
                }
            }

            checkPoint = thisCPNumber;
            lastCP = other.gameObject;

            if (checkPoint == 0)
            {
                if (lap == 0)
                {
                    lapTime = 0;
                    lap++;
                }
                else if (lapTime > 10)
                {
                    lapsTime[lap - 1] = lapTime;
                    Debug.Log(lapsTime[lap - 1]);
                    totalTime += lapTime;
                    lapTime = 0;
                    lap++;
                    if (lap >= RaceMonitor.totalLaps + 1)
                        win = true;
                }
            }
        }
    }

    private void Update()
    {
        if (lap >= 1 && win == false)
            lapTime += Time.deltaTime;
        else
            lapTime = 0;
    }
}

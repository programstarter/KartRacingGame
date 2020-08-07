using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public int lap = 0;
    public float lapTime { get; private set; }
    public int checkPoint = -1;
    //int checkPointCount;
    //int nextCheckPoint;
    public int win = 0;
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
                    lapTime = 0;
                    lap++;
                }
            }
        }
    }

    private void Update()
    {
        if (lap >= 1 && win == 0)
            lapTime += Time.deltaTime;
        else
            lapTime = 0;
    }
}

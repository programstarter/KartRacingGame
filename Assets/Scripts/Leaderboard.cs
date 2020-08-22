using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

struct PlayerStats
{
    public string name;
    public int position;
    public float CheckPointTime;
    public float[] lapsTime;
    public float totalTime;
    public PlayerStats(string n, int p, float c, float t, int l)
    {
        name = n;
        position = p;
        CheckPointTime = c;
        lapsTime = new float[l];
        totalTime = t;
    }
}

public class Leaderboard
{
    static Dictionary<int, PlayerStats> lb = new Dictionary<int, PlayerStats>();
    static int carsRegistered = -1;

    public static void Reset()
    {
        lb.Clear();
        carsRegistered = -1;
        RaceMonitor.racing = false;
    }
    public static int RegisterCar(string name, int laps)
    {
        carsRegistered++;
        lb.Add(carsRegistered, new PlayerStats(name, 0, 0, 0, laps));
        return carsRegistered;
    }

    public static void SetPosition(int repo, int lap, float totalTime,int checkpoint, float checkpointTime, int laps)
    {
        int position = lap * 1000 + checkpoint;
        lb[repo] = new PlayerStats(lb[repo].name, position, checkpointTime, totalTime, laps);
    }

    public static void SetLapTime(int repo, int lap, float lapTime)
    {
        lb[repo].lapsTime[lap] = lapTime;
        Debug.Log(lb[repo].name + "/" + lap + " lap: " + lb[repo].lapsTime[lap]);
    }

    public static string GetPosition(int repo)
    {
        int index = 0;
        foreach(KeyValuePair<int,PlayerStats> pos in lb.OrderBy(key => key.Value.totalTime).ThenByDescending(key => key.Value.position).ThenByDescending(key => key.Value.CheckPointTime))
        {
            index++;
            if(pos.Key == repo)
                switch(index)
                {
                    case 1: return "First";
                    case 2: return "Second";
                    case 3: return "Third";
                    case 4: return "Fourth";
                }
        }
        return "Unknown";
    }

    public static List<string> GetPlaces()
    {
        List<string> places = new List<string>();
        foreach(KeyValuePair<int,PlayerStats> pos in lb.OrderBy(key => key.Value.totalTime).ThenByDescending(key => key.Value.position).ThenByDescending(key => key.Value.CheckPointTime))
        {
            places.Add(pos.Value.name);
        }
        return places;
    }

    public static List<float> GetTotalTimes()
    {
        List<float> times = new List<float>();
        foreach(KeyValuePair<int,PlayerStats> pos in lb.OrderBy(key => key.Value.totalTime).ThenByDescending(key => key.Value.position))
        {
            times.Add(pos.Value.totalTime);
            Debug.Log(pos.Value.name + ", total time: " + pos.Value.totalTime);
        }
        return times;
    }

    public static List<float[]> GetLapsTimes()
    {
        List<float[]> lapsTime = new List<float[]>();
        foreach(KeyValuePair<int,PlayerStats> pos in lb.OrderBy(key => key.Value.totalTime).ThenByDescending(key => key.Value.position))
        {
            lapsTime.Add(pos.Value.lapsTime);
        }
        
        return lapsTime;
    }
}

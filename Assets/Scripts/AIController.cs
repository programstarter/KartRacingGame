using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour
{
    public Circuit circuit;
    Drive ai_drive;
    public float accelingSensitivity = 0.3f;
    public float steeringSensitivity = 0.01f;
    public float brakingSensitivity = 1.1f;
    Vector3 target;
    Vector3 nextTarget;
    int currentWP = 0;
    float totalDistanceToTarget;

    GameObject tracker;
    int currentTrackerWP = 0;
    float lookAhead = 10;

    float lastTimeMoving = 0;

    CheckpointManager cpManager;
    float finishSteer;

    // Use this for initialization
    void Start()
    {
        if (circuit == null)
            circuit = GameObject.FindGameObjectWithTag("Circuit").GetComponent<Circuit>();

        ai_drive = this.GetComponent<Drive>();
        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, ai_drive.rb.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = ai_drive.rb.gameObject.transform.position;
        tracker.transform.rotation = ai_drive.rb.gameObject.transform.rotation;

        this.GetComponent<Ghost>().enabled = false;

        finishSteer = Random.Range(-0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        FollowTracker();
        
        ai_drive.CheckForSkid();
        ai_drive.CalculateEngineSound();
    }

    void ProgressTracker()
    {
        Debug.DrawLine(ai_drive.rb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(ai_drive.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return;

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1);   // speed of trakcer

        if(Vector3.Distance(tracker.transform.position,circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
                currentTrackerWP = 0;
        }
    }

    void ResetLayer()
    {
        ai_drive.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }

    void FollowTracker()
    {
        if(!RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }

        if (cpManager == null)
            cpManager = ai_drive.rb.gameObject.GetComponent<CheckpointManager>();

        if(cpManager.lap >= RaceMonitor.totalLaps + 1)
        {
            ai_drive.highAccelSound.mute = true;
            ai_drive.skidSound.mute = true;
            ai_drive.CarMove(0, finishSteer, 0.5f);
            return;
        }

        ProgressTracker();

        if (ai_drive.rb.velocity.magnitude > 1)
            lastTimeMoving = Time.time;

        if(Time.time > (lastTimeMoving + 4) || ai_drive.rb.gameObject.transform.position.y < -5)
        {
            ai_drive.rb.gameObject.transform.position = cpManager.lastCP.transform.position + Vector3.up * 2;
            ai_drive.rb.gameObject.transform.rotation = cpManager.lastCP.transform.rotation;

            //ai_drive.rb.gameObject.transform.position =
            //    circuit.waypoints[currentTrackerWP].transform.position + Vector3.up * 3 +
            //    new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
            tracker.transform.position = ai_drive.rb.gameObject.transform.position;
            ai_drive.rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        Vector3 localTarget;
        if (Time.time < ai_drive.rb.GetComponent<AvoidDetector>().avoidTime)
            localTarget = tracker.transform.right * ai_drive.rb.GetComponent<AvoidDetector>().avoidPath;
        else
            localTarget = ai_drive.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Rad2Deg;

        float speedFactor = ai_drive.currentSpeed / ai_drive.maxSpeed;

        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90;

        float accel = 1;
        if (corner > 20 && speedFactor > 0.2f)
            accel = Mathf.Lerp(0, 1 * accelingSensitivity, 1 - cornerFactor);

        float brake = 0;
        if (corner > 10 && speedFactor > 0.1f)
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);

        float prevTorque = ai_drive.torque;
        if (speedFactor < 0.3f && ai_drive.rb.gameObject.transform.forward.y > 0.1f)
        {
            ai_drive.torque *= 3.0f;
            accel = 1;
            brake = 0;
        }

        ai_drive.CarMove(accel, steer, brake);
        ai_drive.torque = prevTorque;
    }

    void FollowFixedPoint()
    {
        Vector3 localTarget = ai_drive.rb.gameObject.transform.InverseTransformPoint(target);
        Vector3 nextLocalTarget = ai_drive.rb.gameObject.transform.InverseTransformPoint(nextTarget);
        float distanceToTarget = Vector3.Distance(target, ai_drive.rb.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(ai_drive.currentSpeed);

        float distanceFactor = distanceToTarget / totalDistanceToTarget;
        float speedFactor = ai_drive.currentSpeed / ai_drive.maxSpeed;

        float accel = Mathf.Lerp(accelingSensitivity, 1, distanceFactor);
        float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);

        if (Mathf.Abs(nextTargetAngle) > 20)
        {
            accel -= 0.8f;
            brake += 0.8f;
        }

        //if (distanceToTarget < 5) { brake = 0.8f; accel = 0.1f; }

        ai_drive.CarMove(accel, steer, brake);

        if (distanceToTarget < 4)   //threshold, make larger if car starts to circle waypoint
        {
            currentWP++;
            if (currentWP >= circuit.waypoints.Length)
                currentWP = 0;

            target = circuit.waypoints[currentWP].transform.position;

            if (currentWP == circuit.waypoints.Length - 1)
                nextTarget = circuit.waypoints[0].transform.position;
            else
                nextTarget = circuit.waypoints[currentWP + 1].transform.position;

            totalDistanceToTarget = Vector3.Distance(target, ai_drive.rb.transform.position);
        }
    }
}

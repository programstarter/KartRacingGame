using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Drive))]
public class PlayerController : MonoBehaviour
{
    Drive m_drive;
    float lastTimeMoving = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;

    CheckpointManager cpManager;
    float finishSteer;

    // Start is called before the first frame update
    void Start()
    {
        m_drive = this.GetComponent<Drive>();
        this.GetComponent<Ghost>().enabled = false;
        finishSteer = Random.Range(-0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (cpManager == null)
            cpManager = m_drive.rb.gameObject.GetComponent<CheckpointManager>();

        if(cpManager.lap >= RaceMonitor.totalLaps +1)
        {
            m_drive.highAccelSound.mute = true;
            m_drive.skidSound.mute = true;
            m_drive.CarMove(0, finishSteer, 0.5f);
            return;
        }

        float vertical = Input.GetAxis("Vertical"); //Keyboard Input
        float horizontal = Input.GetAxis("Horizontal"); //Keyboard Input
        float brake = Input.GetAxis("Jump"); //Keyboard Input

        if (m_drive.rb.velocity.magnitude > 1 || !RaceMonitor.racing)
            lastTimeMoving = Time.time;

        /*RaycastHit hit;
        if(Physics.Raycast(m_drive.rb.gameObject.transform.position,-Vector3.up,out hit,10))
        {
            if(hit.collider.gameObject.tag == "Road")
            {
                lastPosition = m_drive.rb.gameObject.transform.position;
                lastRotation = m_drive.rb.gameObject.transform.rotation;
            }
        }*/

        if (Time.time > (lastTimeMoving + 4) || m_drive.rb.gameObject.transform.position.y < -5)
        {
            m_drive.rb.gameObject.transform.position = cpManager.lastCP.transform.position + Vector3.up * 2;
            m_drive.rb.gameObject.transform.rotation = cpManager.lastCP.transform.rotation;

            //m_drive.rb.gameObject.transform.position = lastPosition;
            //m_drive.rb.gameObject.transform.rotation = lastRotation;
            m_drive.rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }

        if (!RaceMonitor.racing) vertical = 0;

        m_drive.CarMove(vertical, horizontal, brake);
        m_drive.CheckForSkid();
        m_drive.CalculateEngineSound();
    }

    void ResetLayer()
    {
        m_drive.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayController : MonoBehaviour
{
    public AudioClip[] backgroundMusics;
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int selectMusic = Random.Range(0, backgroundMusics.Length);
        audioSource.clip = backgroundMusics[selectMusic];
        if(selectMusic == 0)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.PlayDelayed(3f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    float HUDSetting = 1;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        if (PlayerPrefs.HasKey("HUD"))
            HUDSetting = PlayerPrefs.GetFloat("HUD");
    }

    // Update is called once per frame
    void Update()
    {
        if (RaceMonitor.racing)
            canvasGroup.alpha = HUDSetting;
        else
            canvasGroup.alpha = 0;

        if (Input.GetKeyDown(KeyCode.H))
        {
            canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;
            HUDSetting = canvasGroup.alpha;
            PlayerPrefs.SetFloat("HUD", canvasGroup.alpha);
        }
    }
}

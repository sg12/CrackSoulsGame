using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InfoMessage : MonoBehaviour
{
    public float fadeTime = 5;

    [Header("REFERENCES")]
    public FadeUI fadeUI;
    public TextMeshProUGUI info;

    private float timer;

    // Use this for initialization
    void Awake () 
    {
        fadeUI = GetComponent<FadeUI>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        FadeTimer();
    }

    void FadeTimer()
    {
        if (info.text != "")
        {
            if (fadeUI.canvasGroup.alpha == 0)
            {
                timer = fadeTime;
                fadeUI.FadeTransition(1, 0, 1);
            }
            if (fadeUI.canvasGroup.alpha == 1)
            {
                fadeUI.FadeTransition(0, timer, 1);
            }
            if (timer <= 0)
            {
                timer = 0;
                info.text = "";
            }
            if (timer > 0) timer -= Time.deltaTime;
        }
    }
}

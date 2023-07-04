using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryToCreditsScript : MonoBehaviour
{
    public bool buttonPressed = false;
    public ScreenFader fader;

    // Update is called once per frame
    void Update()
    {   
        if(buttonPressed == false) {
            if (Input.GetKeyDown("z") || Input.GetKeyDown("enter") || Input.GetKeyDown("return")) {
                buttonPressed = true;
                StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"CreditSceneEnding"));
            }
        }
        
    }
}

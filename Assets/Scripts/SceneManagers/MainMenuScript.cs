using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public RectTransform commandArrow;  //initial pos -160  88

    public ScreenFader fader;

    public AudioSource SFX;

    public AudioClip[] soundEffects;

    public string action = "start";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("down")) {
            SFX.clip = soundEffects[0];
            SFX.Play();
            if (action.Equals("exit")) {
                commandArrow.localPosition  = new Vector3(-160f, 88,0f);
                action = "start";
            }
            else if (action.Equals("start")){
                commandArrow.localPosition  = new Vector3(-160f,7,0f);
                action = "credits";
            }
            else {
                commandArrow.localPosition  = new Vector3(-160f,-79f,0f);
                action = "exit";
            }
        }
        else if(Input.GetKeyDown("up")){
            SFX.clip = soundEffects[0];
            SFX.Play();
            if (action.Equals("exit")) {
                commandArrow.localPosition  = new Vector3(-160f,7,0f);
                action = "credits";
            }
            else if (action.Equals("start")){
                commandArrow.localPosition  = new Vector3(-160f,-79f,0f);
                action = "exit";
            }
            else {
                commandArrow.localPosition  = new Vector3(-160f,88,0f);
                action = "start";
            }
        }
        else if (Input.GetKeyDown("z") || Input.GetKeyDown("enter") || Input.GetKeyDown("return")) {
            SFX.clip = soundEffects[1];
            SFX.Play();
            DoAction();
        }
    }

    public void DoAction() {
        switch (action) {
            case "start":
                StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"CharacterCreationScene"));
                //SceneManager.LoadScene("CharacterCreationScene");
                break;
            case "credits":
                StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"CreditScene"));
                break;
            case "exit":    
                Application.Quit();
                break;
        }
    }
}

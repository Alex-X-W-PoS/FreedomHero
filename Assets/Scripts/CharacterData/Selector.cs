using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Selector : MonoBehaviour
{

    public int selectedImage = 0;

    public Text classText;

    public RectTransform textTransform;

    public GameObject classPanel;

    public Text classDesctiption;

    public ClassDataHolder classHolder;

    public bool hasSelectedClass = false;

    public AudioSource SFX;

    public AudioClip[] soundEffects;

    // Start is called before the first frame update
    void Start()
    {
        textTransform = this.gameObject.GetComponent<RectTransform>();
        string storyTextPath = Path.Combine(Application.streamingAssetsPath,"StoryTexts");
        string classTextPath = Path.Combine(storyTextPath,"ClassDescriptions.json");
        string classTextJSON = File.ReadAllText(classTextPath);
        classHolder = JsonUtility.FromJson<ClassDataHolder>(classTextJSON);
    }

    // Update is called once per frame
    void Update()
    {
        if(hasSelectedClass == false) {
            if (Input.GetKeyDown("right")) {
                SFX.clip = soundEffects[0];
                SFX.Play();
                switch (selectedImage)
                {
                    case 0:
                        textTransform.localPosition  = new Vector3(-117.9f,39.02f,0f);
                        selectedImage++;
                        break;
                    case 1:
                        textTransform.localPosition = new Vector3(121.8f,39.02f,0f);
                        selectedImage++;
                        break;
                    case 2:
                        textTransform.localPosition = new Vector3(354.2f,39.02f,0f);
                        selectedImage++;
                        break;
                    case 3:
                        textTransform.localPosition = new Vector3(-357f,39.02f,0f);
                        selectedImage = 0;
                        break;
                    default:
                        break;
                }
                UpdateClassText();
            }
            else if(Input.GetKeyDown("left")){
                SFX.clip = soundEffects[0];
                SFX.Play();
                switch (selectedImage)
                {
                    case 0:
                        textTransform.localPosition = new Vector3(354.2f,39.02f,0f);
                        selectedImage = 3;
                        break;
                    case 1:
                        textTransform.localPosition = new Vector3(-357f,39.02f,0f);
                        selectedImage--;
                        break;
                    case 2:
                        textTransform.localPosition = new Vector3(-117.9f,39.02f,0f);
                        selectedImage--;
                        break;
                    case 3:
                        textTransform.localPosition = new Vector3(121.8f,39.02f,0f);
                        selectedImage--;
                        break;
                    default:
                        break;
                }
                UpdateClassText();
            }
            else if(Input.GetKeyDown("z")) {
                SFX.clip = soundEffects[1];
                SFX.Play();
                ShowPanel();
            }
        }
    }

    void UpdateClassText () {
        switch (selectedImage)
        {
            case 0:
                classText.text = "ARCHER";
                break;
            case 1:
                classText.text = "MAGE";
                break;
            case 2:
                classText.text = "PALADIN";
                break;
            case 3:
                classText.text = "VAMPIRE";
                break;
            default:
                break;
        }
    }

    void ShowPanel () {
        hasSelectedClass = true;
        classDesctiption.text = classHolder.classes[selectedImage].text;
        classPanel.SetActive(true);
    }

    public void CancelButton() {
        SFX.clip = soundEffects[2];
        SFX.Play();
        hasSelectedClass = false;
        classDesctiption.text = "";
        classPanel.SetActive(false);
    }
}

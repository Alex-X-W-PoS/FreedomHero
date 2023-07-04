using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SkillAssignationScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Skill[] skillsToChoose;
    public GameObject[] skillSlots;

    public RectTransform selectorTransform;

    public int selectedSkill = 0;

    public bool[] skillAvailability = {
        false, false, false, false
    };

    public SelectorAnimation anim;

    public Text skillName;
    public Text skillDescription;

    public GameObject confirmationPanel;

    public Skill chosenSkill = null;

    public bool hasSelectedSkill = false;

    public string skillType = "";

    public AudioSource SFX;

    public AudioClip[] soundEffects;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hasSelectedSkill == false) {
            if (Input.GetKeyDown("down")) {
                SFX.clip = soundEffects[0];
                SFX.Play();
                switch (selectedSkill)
                {
                    case 0:
                        if(skillAvailability[1] == true) {
                            selectorTransform.localPosition  = new Vector3(0f,60f,0f);
                            selectedSkill++;
                        }
                        break;
                    case 1:
                        if(skillAvailability[2] == true) {
                            selectorTransform.localPosition  = new Vector3(0f,-60f,0f);
                            selectedSkill++;
                        }
                        else {
                            selectorTransform.localPosition  = new Vector3(0f,180f,0f);
                            selectedSkill = 0;
                        }
                        break;
                    case 2:
                        if(skillAvailability[3] == true) {
                            selectorTransform.localPosition  = new Vector3(0f,-180f,0f);
                            selectedSkill++;
                        }
                        else {
                            selectorTransform.localPosition  = new Vector3(0f,180f,0f);
                            selectedSkill = 0;
                        }
                        break;
                    case 3:
                        selectorTransform.localPosition  = new Vector3(0f,180f,0f);
                        selectedSkill = 0;
                        break;
                    default:
                        break;
                }
            }
            else if(Input.GetKeyDown("up")){
                SFX.clip = soundEffects[0];
                SFX.Play();
                switch (selectedSkill)
                {
                    case 0:
                        for(int i = 3; i > 0; i--) {
                            if (skillAvailability[i] == true) {
                                switch (i) {
                                    case 3:
                                        selectorTransform.localPosition  = new Vector3(0f,-180f,0f);
                                        break;
                                    case 2:
                                        selectorTransform.localPosition  = new Vector3(0f,-60f,0f);
                                        break;
                                    case 1:
                                        selectorTransform.localPosition  = new Vector3(0f,60f,0f);
                                        break;
                                    default:
                                        break;
                                }
                                selectedSkill = i;
                                break;
                            }
                        }
                        break;
                    case 1:
                        selectorTransform.localPosition  = new Vector3(0f,180f,0f);
                        selectedSkill--;
                        break;
                    case 2:
                        selectorTransform.localPosition  = new Vector3(0f,60f,0f);
                        selectedSkill--;
                        break;
                    case 3:
                        selectorTransform.localPosition  = new Vector3(0f,-60f,0f);
                        selectedSkill--;
                        break;
                    default:
                        break;
                }
            }
            else if(Input.GetKeyDown("z")) {
                SFX.clip = soundEffects[1];
                SFX.Play();
                ShowPanel();
            }
        }
    }

    public void CreateAssignation (Skill[] skills, int playerStat, string type) {
        skillsToChoose = skills;
        int index = 0;

        foreach (Skill item in skillsToChoose)
        {
            Text skillText = skillSlots[index].GetComponentInChildren<Text>();
            skillText.text = item.name;
            if(playerStat >= item.minimum_required) {
                skillSlots[index].GetComponent<Image>().color = new Color(1,0,0,0.5f);
                skillAvailability[index] = true;
            }
            index += 1;
        }

        selectedSkill = 0;
        selectorTransform.localPosition  = new Vector3(0f,180f,0f);
        skillType = type;
        if(type != "L") {
            anim.StartAnimation();
        }
    }

    public void ShowPanel () {
        chosenSkill = skillsToChoose[selectedSkill];
        skillName.text = chosenSkill.name;
        skillDescription.text = chosenSkill.description;
        hasSelectedSkill = true;
        confirmationPanel.SetActive(true);
    }


    public void CancelButton () {
        SFX.clip = soundEffects[2];
        SFX.Play();
        confirmationPanel.SetActive(false);
        chosenSkill = null;
        hasSelectedSkill = false;
    }

    public void ResetObject () {
        skillsToChoose = null;
        chosenSkill = null;
        selectedSkill = 0;
        selectorTransform.localPosition  = new Vector3(0f,180f,0f);
        foreach (GameObject slot in skillSlots) {
            Text skillText = slot.GetComponentInChildren<Text>();
            skillText.text = "";
            slot.GetComponent<Image>().color = new Color(0,0,0,0.5f);
        }
        for (int i = 0; i < skillAvailability.Length; i++) {
            skillAvailability[i] = false;
        }
        skillName.text = "";
        skillDescription.text = "";
        confirmationPanel.SetActive(false);
        hasSelectedSkill = false;
        skillType = "";
    }

}

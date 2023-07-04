using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class StatAssignationScript : MonoBehaviour
{
    public Text hp_text;
    public Text l_text;
    public Text h_text;
    public Text s_text;

    public Button confirmButton;

    public int selectedStatus = 0;
    public int hpIndex = -1;
    public int lIndex = -1;
    public int hIndex = -1;
    public int sIndex = -1;

    public int [] statsToAssign = {
        0,0,0,0
    };

    public bool [] usedNumber = {
        false,false,false,false
    };

    public AudioSource SFX;

    public AudioClip[] soundEffects;

    public GameObject statAssignator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("right")) {
            SFX.clip = soundEffects[0];
            SFX.Play();
            switch (selectedStatus)
            {  
                case 0: //HP case
                    if(hpIndex == 3) {
                        hpIndex = -1;
                        hp_text.text = "0";
                        usedNumber[3] = false;
                    }
                    else {
                        bool hasChanged = false;
                        for(int i = hpIndex+1;i < statsToAssign.Length; i++) { ///search from the next number to see if it is occupied
                            if(!usedNumber[i]) { ///if a number not used has been found
                                hp_text.text = statsToAssign[i].ToString(); ///put the new number
                                usedNumber[i] = true; ///new number is used
                                if(hpIndex != -1) {
                                    usedNumber[hpIndex] = false; ///old number is unused
                                }
                                hpIndex = i; ///change hp index
                                hasChanged = true; ///signaling the number has changed
                                break;
                                
                            }
                        }
                        if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                            if(hpIndex != -1) {
                                usedNumber[hpIndex] = false;
                                hpIndex = -1;
                                hp_text.text = "0";
                            }
                        }
                    }
                    break;

                case 1: //L case
                    if(lIndex == 3) {
                        lIndex = -1;
                        l_text.text = "0";
                        usedNumber[3] = false;
                    }
                    else {
                        bool hasChanged = false;
                        for(int i = lIndex+1;i < statsToAssign.Length; i++) { ///search from the next number to see if it is occupied
                            if(!usedNumber[i]) { ///if a number not used has been found
                                l_text.text = statsToAssign[i].ToString(); ///put the new number
                                usedNumber[i] = true; ///new number is used
                                if(lIndex != -1) {
                                    usedNumber[lIndex] = false; ///old number is unused
                                }
                                lIndex = i; ///change hp index
                                hasChanged = true; ///signaling the number has changed
                                break;
                                
                            }
                        }
                        if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                            if(lIndex != -1) {
                                usedNumber[lIndex] = false;
                                lIndex = -1;
                                l_text.text = "0";
                            }
                        }
                    }
                    break;

                case 2: //H case
                    if(hIndex == 3) {
                        hIndex = -1;
                        h_text.text = "0";
                        usedNumber[3] = false;
                    }
                    else {
                        bool hasChanged = false;
                        for(int i = hIndex+1;i < statsToAssign.Length; i++) { ///search from the next number to see if it is occupied
                            if(!usedNumber[i]) { ///if a number not used has been found
                                h_text.text = statsToAssign[i].ToString(); ///put the new number
                                usedNumber[i] = true; ///new number is used
                                if(hIndex != -1) {
                                    usedNumber[hIndex] = false; ///old number is unused
                                }
                                hIndex = i; ///change hp index
                                hasChanged = true; ///signaling the number has changed
                                break;
                                
                            }
                        }
                        if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                            if(hIndex != -1) {
                                usedNumber[hIndex] = false;
                                hIndex = -1;
                                h_text.text = "0";
                            }
                        }
                    }
                    break;

                case 3: //S case
                    if(sIndex == 3) {
                        sIndex = -1;
                        s_text.text = "0";
                        usedNumber[3] = false;
                    }
                    else {
                        bool hasChanged = false;
                        for(int i = sIndex+1;i < statsToAssign.Length; i++) { ///search from the next number to see if it is occupied
                            if(!usedNumber[i]) { ///if a number not used has been found
                                s_text.text = statsToAssign[i].ToString(); ///put the new number
                                usedNumber[i] = true; ///new number is used
                                if(sIndex != -1) {
                                    usedNumber[sIndex] = false; ///old number is unused
                                }
                                sIndex = i; ///change hp index
                                hasChanged = true; ///signaling the number has changed
                                break;
                                
                            }
                        }
                        if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                            if(sIndex != -1) {
                                usedNumber[sIndex] = false;
                                sIndex = -1;
                                s_text.text = "0";
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            verifyFullStats();
        }
        else if(Input.GetKeyDown("left")){
            SFX.clip = soundEffects[0];
            SFX.Play();
            switch (selectedStatus)
            {  
                case 0: //HP case
                    if(hpIndex == 0) {
                        hpIndex = -1;
                        hp_text.text = "0";
                        usedNumber[0] = false;
                    }
                    else {
                        bool hasChanged = false;
                        if(hpIndex == -1){
                            for(int i = 3;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    hp_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    hpIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                        }
                        else {
                            for(int i = hpIndex-1;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    hp_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    if(hpIndex != -1) {
                                        usedNumber[hpIndex] = false; ///old number is unused
                                    }
                                    hpIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                            if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                                if(hpIndex != -1) {
                                    usedNumber[hpIndex] = false;
                                    hpIndex = -1;
                                    hp_text.text = "0";
                                }
                            }
                        }
                        
                    }
                    break;

                case 1: //L case
                    if(lIndex == 0) {
                        lIndex = -1;
                        l_text.text = "0";
                        usedNumber[0] = false;
                    }
                    else {
                        bool hasChanged = false;
                        if (lIndex == -1) {
                            for(int i = 3;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    l_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    lIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                        }
                        else {
                            for(int i = lIndex-1;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    l_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    if(lIndex != -1) {
                                        usedNumber[lIndex] = false; ///old number is unused
                                    }
                                    lIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                            if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                                if(lIndex != -1) {
                                    usedNumber[lIndex] = false;
                                    lIndex = -1;
                                    l_text.text = "0";
                                }
                            }
                        }
                        
                    }
                    break;

                case 2: //H case
                    if(hIndex == 0) {
                        hIndex = -1;
                        h_text.text = "0";
                        usedNumber[0] = false;
                    }
                    else {
                        bool hasChanged = false;
                        if (hIndex == -1) {
                            for(int i = 3;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    h_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    hIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                        }
                        else {
                            for(int i = hIndex-1;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    h_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    if(hIndex != -1) {
                                        usedNumber[hIndex] = false; ///old number is unused
                                    }
                                    hIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                            if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                                if(hIndex != -1) {
                                    usedNumber[hIndex] = false;
                                    hIndex = -1;
                                    h_text.text = "0";
                                }
                            }
                        }
                        
                    }
                    break;

                case 3: //S case
                    if(sIndex == 0) {
                        sIndex = -1;
                        s_text.text = "0";
                        usedNumber[0] = false;
                    }
                    else {
                        bool hasChanged = false;
                        if (sIndex == -1) {
                            for(int i = 3;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    s_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    sIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                        }
                        else {
                            for(int i = sIndex-1;i > -1; i--) { ///search from the next number to see if it is occupied
                                if(!usedNumber[i]) { ///if a number not used has been found
                                    s_text.text = statsToAssign[i].ToString(); ///put the new number
                                    usedNumber[i] = true; ///new number is used
                                    if(sIndex != -1) {
                                        usedNumber[sIndex] = false; ///old number is unused
                                    }
                                    sIndex = i; ///change hp index
                                    hasChanged = true; ///signaling the number has changed
                                    break;
                                    
                                }
                            }
                            if (!hasChanged) { ///if no number has changed, return to 0, andn get unbusy the number
                                if(sIndex != -1) {
                                    usedNumber[sIndex] = false;
                                    sIndex = -1;
                                    s_text.text = "0";
                                }
                            }
                        }
                        
                    }
                    break;
                default:
                    break;
            }
            verifyFullStats();
        }
        else if(Input.GetKeyDown("down")){
            SFX.clip = soundEffects[0];
            SFX.Play();
            switch (selectedStatus)
            {
                case 0:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,37.0f,0f);
                    selectedStatus++;
                    break;
                case 1:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,-39.0f,0f);
                    selectedStatus++;
                    break;
                case 2:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,-113.0f,0f);
                    selectedStatus++;
                    break;
                case 3:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,112.4f,0f);
                    selectedStatus = 0;
                    break;
                default:
                    break;
            }
            verifyFullStats();
        }
        else if(Input.GetKeyDown("up")){
            SFX.clip = soundEffects[0];
            SFX.Play();
            switch (selectedStatus)
            {
                case 0:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,-113.0f,0f);
                    selectedStatus = 3;
                    break;
                case 1:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,112.4f,0f);
                    selectedStatus--;
                    break;
                case 2:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,37.0f,0f);
                    selectedStatus--;
                    break;
                case 3:
                    statAssignator.GetComponent<RectTransform>().localPosition = new Vector3(73.9f,-39.0f,0f);
                    selectedStatus--;
                    break;
                default:
                    break;
            }
            verifyFullStats();
        }
    }

    public void startValues (int val1, int val2, int val3, int val4) {
        statsToAssign[0] = val1;
        statsToAssign[1] = val2;
        statsToAssign[2] = val3;
        statsToAssign[3] = val4;
    }

    public void verifyFullStats() {
        bool canActivate = true;
        for(int i = 0; i < usedNumber.Length; i++) {
            if(!usedNumber[i]) {
                canActivate = false;
                break;
            }
        }
        confirmButton.interactable = canActivate;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    public Text textBox;
    public StoryTextData textToShow;

    public int currentlyDisplayingText = 0;

    public bool finishedLine = false;
    public bool isInActivity = false;

    public bool finishedEverythingCompletely = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInActivity == false && finishedEverythingCompletely == false) {
            if (Input.GetKeyDown("z"))
            {
                if(finishedLine == false) {
                    AllTextAtOnce();
                }
                else {
                    SkipToNextText();
                }
            }
        }
    }

    IEnumerator AnimateText(){
     
     for (int i = 0; i < (textToShow.texts[currentlyDisplayingText].text.Length+1); i++)
     {
         textBox.text = textToShow.texts[currentlyDisplayingText].text.Substring(0, i);
         yield return new WaitForSeconds(.02f);
     }
     finishedLine = true;
 }

    public void setTextToShow(StoryTextData text) {
        textToShow = text;
        StartCoroutine(AnimateText());
    }

    public void AllTextAtOnce() {
        StopAllCoroutines();
        textBox.text = textToShow.texts[currentlyDisplayingText].text;
        finishedLine = true;
    }

    public void signalActivity(bool isInput) {
        isInActivity =  isInput;
        if(isInActivity == false) {
            SkipToNextText();
        }
    }

    public void SkipToNextText(){
     StopAllCoroutines();
     currentlyDisplayingText++;
     finishedLine = false;
     if (currentlyDisplayingText==textToShow.texts.Length) {
         finishedEverythingCompletely = true;
     }
     else {
         StartCoroutine(AnimateText());
     }
    }

    public void ChangeVariableInText(string variable, string value) {
        for(int i = currentlyDisplayingText; i < textToShow.texts.Length; i++) {
            if (textToShow.texts[i].text.Contains(variable)) {
                string newText = textToShow.texts[i].text.Replace(variable,value);
                textToShow.texts[i].text = newText;
                //Debug.Log(value);
            }
        }
    }
}

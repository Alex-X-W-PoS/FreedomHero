using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class EnemyBlinking : MonoBehaviour
{
    public bool finishedBlinking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Blink()
	{
		bool blink = false;

		// Loop to switch dice sides ramdomly
		// before final side appears. 20 itterations here.
		for (int i = 0; i < 10; i++)
		{
			if (blink) {
                GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 1);
                blink = false;
            }
            else {
                GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0);
                blink = true;
            }
			
            
			// Pause before next itteration
			yield return new WaitForSeconds(0.03f);
		}
        finishedBlinking = true;
	}

    public void StartBlinking() {
        StartCoroutine("Blink");
    }

    public void ResetBlink() {
        finishedBlinking = false;
    }
}

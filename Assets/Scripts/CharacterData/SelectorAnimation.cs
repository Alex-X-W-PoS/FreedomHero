using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SelectorAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public int frame = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AnimateSelector());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation() {
         StartCoroutine(AnimateSelector());
    }

    IEnumerator AnimateSelector(){
        while(true) {
            if (frame < 2) {
            frame++;
            }
            else {
                frame = 0;
            }
            this.gameObject.GetComponent<Image>().sprite = frames[frame];
            yield return new WaitForSeconds(.15f);
        }
    }
}

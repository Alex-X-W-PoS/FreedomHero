using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
 
public class ScreenFader : MonoBehaviour {

    public Image fadeOutUIImage;
    public float fadeSpeed = 0.8f; 
 
    public enum FadeDirection
    {
        In, 
        Out 
    }

    public bool startWithFade = false;
    
    void OnEnable()
    {
        if(startWithFade) {
            StartCoroutine(Fade(FadeDirection.Out));
        }
    }
    private IEnumerator Fade(FadeDirection fadeDirection) 
    {
        float fromFade = 0;
        if (fadeDirection == FadeDirection.Out) {
            fromFade = 1;
        }
        float toFade = 1;
        if (fadeDirection == FadeDirection.Out) {
            toFade = 0;
        }
        if (fadeDirection == FadeDirection.Out) {
            while (fromFade >= toFade)
            {
                SetColorImage (ref fromFade, fadeDirection);
                yield return null;
            }
            fadeOutUIImage.enabled = false; 
        } else {
            fadeOutUIImage.enabled = true; 
            while (fromFade <= toFade)
            {
                SetColorImage (ref fromFade, fadeDirection);
                yield return null;
            }
        }
    }
    
    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad) 
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(sceneToLoad);
    }
 
    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeOutUIImage.color = new Color (fadeOutUIImage.color.r,fadeOutUIImage.color.g, fadeOutUIImage.color.b, alpha);
        int fadeDirec = 1;
        if (fadeDirection == FadeDirection.Out) {
            fadeDirec = -1;
        }
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * (fadeDirec) ;
    }
}
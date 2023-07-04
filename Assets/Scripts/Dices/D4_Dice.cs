using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class D4_Dice : MonoBehaviour, IPointerClickHandler {

	public Sprite [] diceSides;

	public int finalResult = 1;
    public bool finishedRolling = false;

	public bool max_rigged = false;
	void Start () {

	}

	public void OnPointerClick(PointerEventData eventData) {
		StartCoroutine("RollTheDice");
		//Debug.Log ("Finished");
	}


	private IEnumerator RollTheDice()
	{
		int randomDiceSide = 0;
		int finalSide = 0;

		for (int i = 0; i <= 20; i++)
		{
			if (i == 20 && max_rigged == true) {
				randomDiceSide = 3;
				GetComponent<Image>().sprite = diceSides[randomDiceSide];

				yield return new WaitForSeconds(0.02f);
			}
			else {
				randomDiceSide = Random.Range(0, 4);
				GetComponent<Image>().sprite = diceSides[randomDiceSide];

				yield return new WaitForSeconds(0.02f);
			}
			
		}
		finalSide = randomDiceSide + 1;

		finalResult = finalSide;
        finishedRolling = true;
	}
	
	void Update () {
		
	}

	public void ResetDice() {
		finalResult = 1;
		finishedRolling = false;
		GetComponent<Image>().sprite = diceSides[0];
	}
}

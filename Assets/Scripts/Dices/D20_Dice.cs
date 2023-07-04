using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class D20_Dice : MonoBehaviour, IPointerClickHandler {

	public Sprite [] diceSides;

	public int finalResult = 1;
	public bool finishedRolling = false;


	// Use this for initialization
	void Start () {

	}

	public void OnPointerClick(PointerEventData eventData) {
		StartCoroutine("RollTheDice");
		//Debug.Log ("Finished");
	}


	private IEnumerator RollTheDice()
	{
		// Variable to contain random dice side number.
		// It needs to be assigned. Let it be 0 initially
		int randomDiceSide = 0;

		// Final side or value that dice reads in the end of coroutine
		int finalSide = 0;

		// Loop to switch dice sides ramdomly
		// before final side appears. 20 itterations here.
		for (int i = 0; i <= 20; i++)
		{
			randomDiceSide = Random.Range(0, 20);

			// Set sprite to upper face of dice from array according to random value
			//Debug.Log(diceSides);
			GetComponent<Image>().sprite = diceSides[randomDiceSide];

			// Pause before next itteration
			yield return new WaitForSeconds(0.02f);
		}

		// Assigning final side so you can use this value later in your game
		// for player movement for example
		finalSide = randomDiceSide + 1;

		finalResult = finalSide;
		finishedRolling = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ResetDice() {
		finalResult = 1;
		finishedRolling = false;
		GetComponent<Image>().sprite = diceSides[0];
	}
}

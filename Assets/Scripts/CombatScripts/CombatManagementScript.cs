using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class CombatManagementScript : MonoBehaviour
{
    public DataManager dataManager;
    public Text characterName;
    public Text HP_text;
    public Text L_text;
    public Text H_text;
    public Text S_Text;
    public Text shield_text;
    public Text AP_Text;
    public Image playerImage;
    public Text skill_1;
    public Text skill_2;
    public Text skill_3;
    public Sprite [] statusImages;
    public Image statusImage;
    public Sprite [] enemyImages;
    public Image [] enemiesOnScene;
    public Text messageBox;
    public Queue<string> turnOrder = new Queue<string>();
    public string actualTurn = "";
    public Text item_1;
    public Text item_2;
    public Text item_3;
    public Text item_1_quantity;
    public Text item_2_quantity;
    public Text item_3_quantity;

    public GameObject commandPannel;
    public GameObject skillPanel;
    public GameObject itemPanel;
    public GameObject singleTarget;
    public GameObject multipeTarget;
    public GameObject multiAr1;
    public GameObject multiAr2;
    public GameObject multiAr3;
    public Text enemy1HP;
    public Text enemy2HP;
    public Text enemy3HP;

    public RectTransform commandArrow; ///DEFAULT POSITION (-220 X   29 Y)
    public RectTransform skillArrow; ///DEFAULT POSITION (-220 X   42 Y)
    public RectTransform itemArrow; ///DEFAULT POSITION (-220 X   42 Y)
    public RectTransform singleTargetArrow; //-307    0    307     (150.25Y)

    public int itemArrowPosition = 0; // 0 - top, 1 - middle, 2 - below

    /////VARIABLES FOR TURN MANIPULATION////
    public bool movingTurns = true;
    public bool isPlayerTurn = false;
    public bool isSelectingEnemy = false;
    public string currentScreen = "command";
    public string selectedOption = "skills";
    public int selectedTarget = 0; ///0 first, 1 second, 2 third, 3 all
    public Skill selectedSkill = null;
    public PlayerItem selectedItem = null;
    public bool waitingForRoll = false;
    public string typeOfRolling = "normal"; 
    // normal = attack
    // modification = damage modification
    // status = status condition
    // cure = has cure
    // mitigation = damageMitigation
    public bool isPlayingAnimation = false;
    public bool isPlayingAnimationModification = false;
    public bool allEnemiesDead = false;
    public bool isUpdatingEnemiesHP = false;
    public bool isUpdatingEnemiesHPModification = false;

    ////////////////////////////////////////


    //////////VARIABLE FOR DICE PANELS MODIFICATION//////////

    public GameObject panelD4;
    public D4_Dice panelD4_dice;
    public Button panelD4_button;
    public GameObject panel2D4;
    public D4_Dice panel2D4_dice1;
    public D4_Dice panel2D4_dice2;
    public Button panel2D4_button;
    public GameObject panelD6;
    public D6_Dice panelD6_dice;
    public Button panelD6_button;
    public GameObject panel2D6;
    public D6_Dice panel2D6_dice1;
    public D6_Dice panel2D6_dice2;
    public Button panel2D6_button;
    public GameObject panelD20;
    public D20_Dice panelD20_dice;
    public Button panelD20_button;
    public string diceType = "";
    public int totalDices = 0;
    public int totalDamage = 0;

    public bool increasedL = false;
    public bool increasedH = false;
    public bool increasedS = false;
    public int statusIncrease = 0;
    public BattleEnemy selectedEnemy = null;
    public Skill selectedEnemySkill = null;

    public AudioClip [] battleThemes;
    public AudioSource backgroundMusic;
    public Image [] enemyStatusConditions;

    public AudioSource SFX;

    public AudioClip[] soundEffects;

    public ScreenFader fader;

    public ScreenFader gameoverFader;

    public int selectedEnemyStatusCondition = 0;

    /////////////////////////////////////////////////////////
    private float timeSinceOpened = 0f;
    private float timeToWaitForKeyInput = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
        int playerDataImg = dataManager.player.image;
        playerImage.sprite = dataManager.playerImages[playerDataImg];
        characterName.text = dataManager.player.Name;
        backgroundMusic.clip = battleThemes[dataManager.battleMusicToChoose];
        backgroundMusic.Play();
        ////testing line///
        //dataManager.player.current_HP = dataManager.player.current_HP - 15;
        //dataManager.player.status = "Bleed";
        //dataManager.player.status_condition_duration = 3;
        //statusImage.sprite = statusImages[4];
        //////////////////
        HP_text.text = "HP: " + dataManager.player.current_HP.ToString() + "/" + dataManager.player.HP.ToString();
        AP_Text.text = "AP: " + dataManager.player.AP.ToString();
        L_text.text = "L: " + dataManager.player.Light_attack_stat.ToString();
        H_text.text = "H: " + dataManager.player.Heavy_attack_stat.ToString();
        S_Text.text = "S: " + dataManager.player.Special_attack_stat.ToString();
        shield_text.text = "Shield: " + dataManager.player.currentShield.ToString();
        skill_1.text = dataManager.player.skills[0].name;
        skill_2.text = dataManager.player.skills[1].name;
        skill_3.text = dataManager.player.skills[2].name;
        statusImage.sprite = statusImages[0];

        item_1.text = dataManager.player.itemBag[0].item.name;
        item_2.text = dataManager.player.itemBag[1].item.name;
        item_3.text = dataManager.player.itemBag[2].item.name;

        item_1_quantity.text = dataManager.player.itemBag[0].quantity.ToString();
        item_2_quantity.text = dataManager.player.itemBag[1].quantity.ToString();
        item_3_quantity.text = dataManager.player.itemBag[2].quantity.ToString();

        turnOrder.Enqueue("player");

        ////// SHOW ENEMIES AND NAMES ACCORDING TO THE NUMBER OF ENEMIES GIVEN ON PREVIOUS SCENE /////////
        switch (dataManager.battleEnemyNumber)
        {
            case 1:
                enemiesOnScene[0].sprite = enemyImages[dataManager.battleEnemies[0].image];
                enemiesOnScene[0].gameObject.SetActive(true);
                messageBox.text = "You encountered an enemy!\nChoose your action.";
                enemy1HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                turnOrder.Enqueue("enemy0");
                break;
            case 2:
                enemiesOnScene[1].sprite = enemyImages[dataManager.battleEnemies[0].image];
                enemiesOnScene[2].sprite = enemyImages[dataManager.battleEnemies[1].image];
                enemiesOnScene[1].gameObject.SetActive(true);
                enemiesOnScene[2].gameObject.SetActive(true);
                if (dataManager.battleEnemies[0].Name.Equals(dataManager.battleEnemies[1].Name)) {
                    dataManager.battleEnemies[0].Name = dataManager.battleEnemies[0].Name + " A";
                    dataManager.battleEnemies[1].Name = dataManager.battleEnemies[1].Name + " B";
                }
                messageBox.text = "You encountered some enemies!\nChoose your action.";
                enemy2HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                enemy3HP.text = "HP" + dataManager.battleEnemies[1].Current_HP.ToString();
                turnOrder.Enqueue("enemy0");
                turnOrder.Enqueue("enemy1");
                break;
            case 3:
                enemiesOnScene[0].sprite = enemyImages[dataManager.battleEnemies[0].image];
                enemiesOnScene[1].sprite = enemyImages[dataManager.battleEnemies[1].image];
                enemiesOnScene[2].sprite = enemyImages[dataManager.battleEnemies[2].image];
                enemiesOnScene[0].gameObject.SetActive(true);
                enemiesOnScene[1].gameObject.SetActive(true);
                enemiesOnScene[2].gameObject.SetActive(true);
                if (dataManager.battleEnemies[0].Name.Equals(dataManager.battleEnemies[1].Name) && dataManager.battleEnemies[0].Name.Equals(dataManager.battleEnemies[2].Name)) {
                    dataManager.battleEnemies[0].Name = dataManager.battleEnemies[0].Name + " A";
                    dataManager.battleEnemies[1].Name = dataManager.battleEnemies[1].Name + " B";
                    dataManager.battleEnemies[2].Name = dataManager.battleEnemies[2].Name + " C";
                }
                else if (dataManager.battleEnemies[0].Name.Equals(dataManager.battleEnemies[1].Name)){
                    dataManager.battleEnemies[0].Name = dataManager.battleEnemies[0].Name + " A";
                    dataManager.battleEnemies[1].Name = dataManager.battleEnemies[1].Name + " B";
                }
                else if (dataManager.battleEnemies[0].Name.Equals(dataManager.battleEnemies[2].Name)) {
                    dataManager.battleEnemies[0].Name = dataManager.battleEnemies[0].Name + " A";
                    dataManager.battleEnemies[2].Name = dataManager.battleEnemies[2].Name + " B";
                }
                else if (dataManager.battleEnemies[1].Name.Equals(dataManager.battleEnemies[2].Name)) {
                    dataManager.battleEnemies[1].Name = dataManager.battleEnemies[1].Name + " A";
                    dataManager.battleEnemies[2].Name = dataManager.battleEnemies[2].Name + " B";
                }
                messageBox.text = "You encountered some enemies!\nChoose your action.";
                enemy1HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                enemy2HP.text = "HP" + dataManager.battleEnemies[1].Current_HP.ToString();
                enemy3HP.text = "HP" + dataManager.battleEnemies[2].Current_HP.ToString();
                turnOrder.Enqueue("enemy0");
                turnOrder.Enqueue("enemy1");
                turnOrder.Enqueue("enemy2");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceOpened = timeSinceOpened + Time.deltaTime;
        /*if (Input.GetKeyDown("o")) {
            SceneManager.LoadScene("MapScene");
        }*/
        if(movingTurns) {
            if(turnOrder.Count > 0) {
                actualTurn = turnOrder.Dequeue();
                if (actualTurn.Equals("player")) {
                    //Debug.Log("PlayerTurn");
                    if (dataManager.player.status.Equals("Sleep") ||dataManager.player.status.Equals("Freeze")) {
                        switch(dataManager.player.status) {
                            case "Sleep":
                                if (dataManager.player.status_condition_duration == 0) {
                                    SFX.clip = soundEffects[7];
                                    SFX.Play();
                                    UpdateMessageBoxSimple("You are cured from Sleep!");
                                    statusImage.sprite = statusImages[0];
                                    dataManager.player.status = "normal";
                                    movingTurns = false;
                                    Invoke("continueAfterStatusCure",0.7f);  
                                }
                                else {
                                    SFX.clip = soundEffects[9];
                                    SFX.Play();
                                    UpdateMessageBoxSimple("You are asleep. You're unable to act!");
                                    movingTurns = false;
                                    dataManager.player.status_condition_duration -= 1;
                                    ///invoke the end of player turn
                                    Invoke("EndingPlayerTurn",0.7f); 
                                }
                                break;
                            case "Freeze": 
                                if (dataManager.player.status_condition_duration == 0) {
                                    SFX.clip = soundEffects[7];
                                    SFX.Play();
                                    UpdateMessageBoxSimple("You are cured from Freeze!");
                                    statusImage.sprite = statusImages[0];
                                    dataManager.player.status = "normal";
                                    movingTurns = false;
                                    Invoke("continueAfterStatusCure",0.7f);  
                                }
                                else {
                                    SFX.clip = soundEffects[9];
                                    SFX.Play();
                                    UpdateMessageBoxSimple("You are frozen. You're unable to act!");
                                    movingTurns = false;
                                    dataManager.player.status_condition_duration -= 1;
                                    ///invoke the end of player turn
                                    Invoke("EndingPlayerTurn",0.7f); 
                                }
                                
                                break;
                        }
                    }
                    else if (dataManager.player.status.Equals("Paralysis")) {
                        int probabilityOfCure = Random.Range(1,101);
                        if (probabilityOfCure > 50 || dataManager.player.status_condition_duration == 0) {
                            UpdateMessageBoxSimple("You are cured form Paralysis!");
                            statusImage.sprite = statusImages[0];
                            dataManager.player.status = "normal";
                            movingTurns = false;
                            Invoke("continueAfterStatusCure",0.7f);  
                        }
                        else {
                            UpdateMessageBoxSimple("You are paralyzed. You're unable to act!");
                            dataManager.player.status_condition_duration -= 1;
                            movingTurns = false;
                            Invoke("EndingPlayerTurn",0.7f);  
                        }
                    }
                    else {
                        ReturnToCommand();
                        movingTurns = false;
                        isPlayerTurn = true;
                    }
                }
                else {
                    movingTurns = false;
                    StartEnemyTurn(actualTurn);
                }
            }
        }   
        if(isPlayerTurn) {
            if (Input.GetKeyDown("down")) {
                SFX.clip = soundEffects[0];
                SFX.Play();
                MoveArrowDown();
            }
            else if(Input.GetKeyDown("up")){
                SFX.clip = soundEffects[0];
                SFX.Play();
                MoveArrowUp();
            }
            else if(Input.GetKeyDown("z") && (timeSinceOpened >= timeToWaitForKeyInput)){
                SFX.clip = soundEffects[1];
                SFX.Play();
                timeSinceOpened = 0f;
                DoAction();
            }
            else if(Input.GetKeyDown("x")){
                SFX.clip = soundEffects[2];
                SFX.Play();
                ReturnToCommand();
            }
        }
        if(isSelectingEnemy) {
            if (selectedSkill.target.Equals("select_1") && dataManager.battleEnemyNumber > 1) {
                if (Input.GetKeyDown("right")) {
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    switch (dataManager.battleEnemyNumber)
                    {
                        case 2:
                            if (selectedTarget == 0 && enemiesOnScene[2].gameObject.activeSelf == true) {
                                singleTargetArrow.localPosition  = new Vector3(307f,150.25f,0f);
                                messageBox.text = "Target: " + dataManager.battleEnemies[1].Name;
                                selectedEnemyStatusCondition = 2;
                                selectedTarget = 1;
                            }
                            break;
                        case 3:
                            if (selectedTarget == 1) {
                                if (enemiesOnScene[0].gameObject.activeSelf == false) { //middle enemy is dead//
                                    if (enemiesOnScene[2].gameObject.activeSelf == true) { //if the other enemy is still alive, if not, not move
                                        singleTargetArrow.localPosition  = new Vector3(307f,150.25f,0f);
                                        messageBox.text = "Target: " + dataManager.battleEnemies[2].Name;
                                        selectedEnemyStatusCondition = 2;
                                        selectedTarget = 2;
                                    }
                                }
                                else {
                                    singleTargetArrow.localPosition  = new Vector3(0f,150.25f,0f);
                                    messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                                    selectedEnemyStatusCondition = 0;
                                    selectedTarget = 0;
                                }
                            }
                            else if (selectedTarget == 0 && enemiesOnScene[2].gameObject.activeSelf == true) {
                                singleTargetArrow.localPosition  = new Vector3(307f,150.25f,0f);
                                messageBox.text = "Target: " + dataManager.battleEnemies[2].Name;
                                selectedEnemyStatusCondition = 2;
                                selectedTarget = 2;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (Input.GetKeyDown("left")) {
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    switch (dataManager.battleEnemyNumber)
                    {
                        case 2:
                            if (selectedTarget == 1 && enemiesOnScene[1].gameObject.activeSelf == true) {
                                singleTargetArrow.localPosition  = new Vector3(-307f,150.25f,0f);
                                messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                                selectedEnemyStatusCondition = 1;
                                selectedTarget = 0;
                            }
                            break;
                        case 3:
                            if (selectedTarget == 2) {
                                if (enemiesOnScene[0].gameObject.activeSelf == false) {
                                    if (enemiesOnScene[1].gameObject.activeSelf == true) { 
                                        singleTargetArrow.localPosition  = new Vector3(-307f,150.25f,0f);
                                        messageBox.text = "Target: " + dataManager.battleEnemies[1].Name;
                                        selectedEnemyStatusCondition = 1;
                                        selectedTarget = 1;
                                    } 
                                }
                                else {
                                    singleTargetArrow.localPosition  = new Vector3(0f,150.25f,0f);
                                    messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                                    selectedEnemyStatusCondition = 0;
                                    selectedTarget = 0;
                                }
                                
                            }
                            else if (selectedTarget == 0 && enemiesOnScene[1].gameObject.activeSelf == true) {
                                singleTargetArrow.localPosition  = new Vector3(-307f,150.25f,0f);
                                messageBox.text = "Target: " + dataManager.battleEnemies[1].Name;
                                selectedEnemyStatusCondition = 1;
                                selectedTarget = 1;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            if (Input.GetKeyDown("x")) {
                SFX.clip = soundEffects[2];
                SFX.Play();
                singleTarget.SetActive(false);
                multipeTarget.SetActive(false);
                ResetMultiArrows();
                isPlayerTurn = true;
                isSelectingEnemy = false;
                UpdateMessageBoxSkill();
            }
            if (Input.GetKeyDown("z") && (timeSinceOpened >= timeToWaitForKeyInput)) {
                SFX.clip = soundEffects[1];
                SFX.Play();
                timeSinceOpened = 0f;
                waitingForRoll = true;
                isSelectingEnemy = false;
                singleTarget.SetActive(false);
                multipeTarget.SetActive(false);
                ResetMultiArrows();
                ShowSkillDice();
            }
        }
        if(waitingForRoll) {
            bool finishedRolling = CheckForDiceFinished();
            if(finishedRolling) {
                GetTotalDiceValue();
                waitingForRoll = false;
                ///depending on type of roll, call the respective function
                switch (typeOfRolling) {
                    case "normal":
                        Invoke("ContinueWithAnimation",1f);
                        break;
                    case "modification":
                        Invoke("ContinueWithModification",1f);
                        break;
                    case "status":
                        Invoke("ContinueWithStatusCondition",1f);
                        break;
                    case "cure":
                        Invoke("ContinueWithCure",1f);
                        break;
                    case "mitigation":
                        Invoke("ContinueWithMitigation",1f);
                        break;
                    default:
                        break;
                }
                
            }
        }
        if(isPlayingAnimation) {
            ////wait for animation to finish... Have to think how to do this....
            bool isFinished = CheckForBlinkEnd();
            //Debug.Log(isFinished.ToString());
            if (isFinished) {
                isPlayingAnimation = false;
                //Debug.Log("Blink End");
                Invoke("ContinueWithDamage",0.5f);
            }
        }

        if(isPlayingAnimationModification) {
            ////wait for animation to finish... Have to think how to do this....
            bool isFinished = CheckForBlinkModificationEnd();
            //Debug.Log(isFinished.ToString());
            if (isFinished) {
                isPlayingAnimationModification = false;
                //Debug.Log("Blink End");
                Invoke("ContinueWithDamageModification",0.5f);
            }
        }
        if (isUpdatingEnemiesHP) {
            isUpdatingEnemiesHP = false;
            UpdateEnemiesHP();
            finishingDamage();
        }
        if (isUpdatingEnemiesHPModification) {
            isUpdatingEnemiesHPModification = false;
            UpdateEnemiesHP();
            finishingDamageModification();
        }
        
    }

    public void MoveArrowDown() {
        switch (currentScreen)
        {
            case "command":
                commandArrow.localPosition  = new Vector3(-220f,-82f,0f);
                ///sound
                selectedOption = "items";
                break;
            case "skill":
                switch (selectedOption)
                {
                    case "skill_1":
                        skillArrow.localPosition = new Vector3(-220f,-29f,0f);
                        ///sound
                        selectedOption = "skill_2";
                        break;
                    case "skill_2":
                        skillArrow.localPosition = new Vector3(-220f,-97f,0f);
                        ///sound
                        selectedOption = "skill_3";
                        break;
                    default:
                        break;
                }
                UpdateMessageBoxSkill();
                break;
            case "item":
                switch (selectedOption)
                {
                    case "item_1":
                        itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                        ///sound
                        selectedOption = "item_2";
                        itemArrowPosition = 1;
                        break;
                    case "item_2":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else {
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        ///sound
                        selectedOption = "item_3";
                        break;
                    case "item_3":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(1);
                        }
                        ///sound
                        selectedOption = "item_4";
                        break;
                    case "item_4":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(2);
                        }
                        ///sound
                        selectedOption = "item_5";
                        break;
                    case "item_5":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(3);
                        }
                        ///sound
                        selectedOption = "item_6";
                        break;
                    case "item_6":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(4);
                        }
                        ///sound
                        selectedOption = "item_7";
                        break;
                    case "item_7":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(5);
                        }
                        ///sound
                        selectedOption = "item_8";
                        break;
                    case "item_8":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(6);
                        }
                        ///sound
                        selectedOption = "item_9";
                        break;
                    case "item_9":
                        if(itemArrowPosition == 0) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,-97f,0f);
                            itemArrowPosition = 2;
                        }
                        else {
                            ChangeItemTexts(7);
                        }
                        ///sound
                        selectedOption = "item_10";
                        break;
                    default:
                        break;
                }
                UpdateMessageBoxItem();
                break;
            default:
                break;
        }
    }
    public void MoveArrowUp() {
        switch (currentScreen)
        {
            case "command":
                commandArrow.localPosition  = new Vector3(-220f,29f,0f);
                ////sound
                selectedOption = "skills";
                break;
            case "skill":
                switch (selectedOption)
                {
                    case "skill_3":
                        skillArrow.localPosition = new Vector3(-220f,-29f,0f);
                        ///sound
                        selectedOption = "skill_2";
                        break;
                    case "skill_2":
                        skillArrow.localPosition = new Vector3(-220f,42f,0f);
                        ///sound
                        selectedOption = "skill_1";
                        break;
                    default:
                        break;
                }
                UpdateMessageBoxSkill();
                break;
            case "item":
                switch (selectedOption)
                {
                    case "item_2":
                        if(itemArrowPosition == 1) {
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(0);
                        }
                        ///sound
                        selectedOption = "item_1";
                        break;
                    case "item_3":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(1);
                        }
                        ///sound
                        selectedOption = "item_2";
                        break;
                    case "item_4":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(2);
                        }
                        ///sound
                        selectedOption = "item_3";
                        break;
                    case "item_5":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(3);
                        }
                        ///sound
                        selectedOption = "item_4";
                        break;
                    case "item_6":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(4);
                        }
                        ///sound
                        selectedOption = "item_5";
                        break;
                    case "item_7":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(5);
                        }
                        ///sound
                        selectedOption = "item_6";
                        break;
                    case "item_8":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else if (itemArrowPosition == 1){
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        else {
                            ChangeItemTexts(6);
                        }
                        ///sound
                        selectedOption = "item_7";
                        break;
                    case "item_9":
                        if(itemArrowPosition == 2) {
                            itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                            itemArrowPosition = 1;
                        }
                        else {
                            itemArrow.localPosition = new Vector3(-220f,42f,0f);
                            itemArrowPosition = 0;
                        }
                        ///sound
                        selectedOption = "item_8";
                        break;
                    case "item_10":
                        itemArrow.localPosition = new Vector3(-220f,-29f,0f);
                        itemArrowPosition = 1;
                        ///sound
                        selectedOption = "item_9";
                        break;
                    default:
                        break;
                }
                UpdateMessageBoxItem();
                break;
            default:
                break;
        }
    }
    public void DoAction() {
        switch (selectedOption)
        {
            case "skills":
                skillArrow.localPosition = new Vector3(-220f,42f,0f);
                commandPannel.SetActive(false);
                skillPanel.SetActive(true);
                currentScreen = "skill";
                selectedOption = "skill_1";
                UpdateMessageBoxSkill();
                break;
            case "items":
                commandPannel.SetActive(false);
                itemPanel.SetActive(true);
                currentScreen = "item";
                selectedOption = "item_1";
                UpdateMessageBoxItem();
                break;
            default:
                switch (currentScreen)
                {
                    case "skill":
                        ProcessPlayerSkill();
                        break;
                    case "item":
                        ProcessItem();
                        break;
                    default:
                        break;
                }
                break;
        }
    }
    public void ReturnToCommand() {
        switch(currentScreen) {
            case "skill":
                commandArrow.localPosition  = new Vector3(-220f,29f,0f);
                skillPanel.SetActive(false);
                commandPannel.SetActive(true);
                skillArrow.localPosition = new Vector3(-220f,42f,0f);
                currentScreen = "command";
                selectedOption = "skills";
                messageBox.text = "It's your turn!\nChoose your action.";
                break;
            case "item":
                commandArrow.localPosition  = new Vector3(-220f,29f,0f);
                itemPanel.SetActive(false);
                commandPannel.SetActive(true);
                itemArrow.localPosition = new Vector3(-220f,42f,0f);
                currentScreen = "command";
                selectedOption = "skills";
                messageBox.text = "It's your turn!\nChoose your action.";
                ChangeItemTexts(0);
                break;
            default:
                break;
        }
    }
    public void ChangeItemTexts(int startingIndex) {
        item_1.text = dataManager.player.itemBag[startingIndex].item.name;
        item_2.text = dataManager.player.itemBag[startingIndex + 1].item.name;
        item_3.text = dataManager.player.itemBag[startingIndex + 2].item.name;

        item_1_quantity.text = dataManager.player.itemBag[startingIndex].quantity.ToString();
        item_2_quantity.text = dataManager.player.itemBag[startingIndex+1].quantity.ToString();
        item_3_quantity.text = dataManager.player.itemBag[startingIndex+2].quantity.ToString();
    }

    public void ProcessPlayerSkill() {
        switch(selectedOption) {
            case "skill_1":
                selectedSkill = dataManager.player.skills[0];
                break;
            case "skill_2":
                selectedSkill = dataManager.player.skills[1];
                break;
            case "skill_3":
                selectedSkill = dataManager.player.skills[2];
                break;
            default:
                break;
        }
        if (selectedSkill.ap_cost > dataManager.player.AP){
            UpdateMessageBoxSimple("You don't have enough AP for using this skill.\nSelect another skill.");
        }
        else {
            string target = selectedSkill.target;
            switch (target) {
                case "select_1":
                    if(dataManager.battleEnemyNumber == 2) {
                        if(enemiesOnScene[1].gameObject.activeSelf == true) {
                            singleTargetArrow.localPosition  = new Vector3(-307f,150.25f,0f);
                            messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                            selectedEnemyStatusCondition = 1;
                            selectedTarget = 0;
                        }
                        else {
                            singleTargetArrow.localPosition  = new Vector3(307f,150.25f,0f);
                            messageBox.text = "Target: " + dataManager.battleEnemies[1].Name;
                            selectedEnemyStatusCondition = 2;
                            selectedTarget = 1;
                        }
                    }
                    else if (dataManager.battleEnemyNumber == 3) {
                        if(enemiesOnScene[0].gameObject.activeSelf == true) {
                            singleTargetArrow.localPosition  = new Vector3(0f,150.25f,0f); //resetting the position if anything
                            messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                            selectedEnemyStatusCondition = 0;
                            selectedTarget = 0;
                        }
                        else if (enemiesOnScene[1].gameObject.activeSelf == true) {
                            singleTargetArrow.localPosition  = new Vector3(-307f,150.25f,0f); //resetting the position if anything
                            messageBox.text = "Target: " + dataManager.battleEnemies[1].Name;
                            selectedEnemyStatusCondition = 1;
                            selectedTarget = 1;
                        }
                        else {
                            singleTargetArrow.localPosition  = new Vector3(307f,150.25f,0f); //resetting the position if anything
                            messageBox.text = "Target: " + dataManager.battleEnemies[2].Name;
                            selectedEnemyStatusCondition = 2;
                            selectedTarget = 2;
                        }
                    }
                    else {
                        singleTargetArrow.localPosition  = new Vector3(0f,150.25f,0f); //resetting the position if anything
                        selectedTarget = 0;
                        messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                        selectedEnemyStatusCondition = 0;
                    }
                    //selectedTarget = 0;
                    //messageBox.text = "Target: " + dataManager.battleEnemies[0].Name;
                    singleTarget.SetActive(true);
                    isPlayerTurn = false;
                    isSelectingEnemy = true;
                    break;
                case "all":
                    multipeTarget.SetActive(true);
                    if (enemiesOnScene[0].gameObject.activeSelf == false) {
                        multiAr1.SetActive(false);
                    }
                    if (enemiesOnScene[1].gameObject.activeSelf == false) {
                        multiAr2.SetActive(false);
                    }
                    if (enemiesOnScene[2].gameObject.activeSelf == false) {
                        multiAr3.SetActive(false);
                    }
                    isPlayerTurn = false;
                    messageBox.text = "Target: All Enemies";
                    isSelectingEnemy = true;
                    break;
                default:
                    break;
            }
        } 
    }
    public void ProcessItem() {
        switch(selectedOption) {
            case "item_1":
                selectedItem = dataManager.player.itemBag[0];
                break;
            case "item_2":
                selectedItem = dataManager.player.itemBag[1];
                break;
            case "item_3":
                selectedItem = dataManager.player.itemBag[2];
                break;
            case "item_4":
                selectedItem = dataManager.player.itemBag[3];
                break;
            case "item_5":
                selectedItem = dataManager.player.itemBag[4];
                break;
            case "item_6":
                selectedItem = dataManager.player.itemBag[5];
                break;
            case "item_7":
                selectedItem = dataManager.player.itemBag[6];
                break;
            case "item_8":
                selectedItem = dataManager.player.itemBag[7];
                break;
            case "item_9":
                selectedItem = dataManager.player.itemBag[8];
                break;
            case "item_10":
                selectedItem = dataManager.player.itemBag[9];
                break;
            default:
                break;
        }
        if (selectedItem.quantity < 1) {
            UpdateMessageBoxSimple("You don't have enough of this item.\nSelect another item.");
        }
        else {
            isPlayerTurn = false;
            Invoke("ExecuteItemEffects",0.7f);
        }
    }

    public void ExecuteItemEffects () {
        switch (selectedItem.item.effect) {
            case "heal":
                if (dataManager.player.current_HP == dataManager.player.HP) {
                    UpdateMessageBoxSimple("Your HP is already at max.");
                }
                else if (dataManager.player.current_HP + selectedItem.item.amount > dataManager.player.HP) {
                    dataManager.player.current_HP = dataManager.player.HP;
                    UpdateMessageBoxSimple("You recovered all your HP!");
                }
                else {
                    dataManager.player.current_HP =  dataManager.player.current_HP + selectedItem.item.amount;
                    UpdateMessageBoxSimple("You recovered " + selectedItem.item.amount + " HP!");
                }
                updatePlayerHP();
                break;
            case "cure":
                if (dataManager.player.status.Equals(selectedItem.item.affected_stat)){
                    dataManager.player.status = "normal";
                    statusImage.sprite = statusImages[0];
                    UpdateMessageBoxSimple("You have been cured from "+ selectedItem.item.affected_stat +"!");
                }
                else {
                    UpdateMessageBoxSimple("The item had no effect");
                }
                break;
            case "increase":
                switch(selectedItem.item.affected_stat) {
                    case "L":
                        dataManager.player.Light_attack_stat = dataManager.player.Light_attack_stat + selectedItem.item.amount;
                        UpdateMessageBoxSimple("You increased your L stat by "+ selectedItem.item.amount +"!");
                        break;
                    case "H":
                        dataManager.player.Heavy_attack_stat = dataManager.player.Heavy_attack_stat + selectedItem.item.amount;
                        UpdateMessageBoxSimple("You increased your H stat by "+ selectedItem.item.amount +"!");
                        break;
                    case "S":
                        dataManager.player.Special_attack_stat = dataManager.player.Special_attack_stat + selectedItem.item.amount;
                        UpdateMessageBoxSimple("You increased your S stat by "+ selectedItem.item.amount +"!");
                        break;
                }
                statusIncrease = selectedItem.item.amount;
                break;
        }
        SFX.clip = soundEffects[7];
        SFX.Play();
        selectedItem.quantity -= 1;
        Invoke("EndingPlayerTurn",0.7f);

    }
    public void UpdateMessageBoxSimple(string text){
        messageBox.text = text;
    }
    public void UpdateMessageBoxSkill() {
        switch (selectedOption) {
            case "skill_1":
                UpdateMessageBoxSimple(dataManager.player.skills[0].description);
                break;
            case "skill_2":
                UpdateMessageBoxSimple(dataManager.player.skills[1].description);
                break;
            case "skill_3":
                UpdateMessageBoxSimple(dataManager.player.skills[2].description);
                break;
            default:
                break;
        }
    }

    public void UpdateMessageBoxItem() {
        switch (selectedOption) {
            case "item_1":
                UpdateMessageBoxSimple(dataManager.player.itemBag[0].item.description);
                break;
            case "item_2":
                UpdateMessageBoxSimple(dataManager.player.itemBag[1].item.description);
                break;
            case "item_3":
                UpdateMessageBoxSimple(dataManager.player.itemBag[2].item.description);
                break;
            case "item_4":
                UpdateMessageBoxSimple(dataManager.player.itemBag[3].item.description);
                break;
            case "item_5":
                UpdateMessageBoxSimple(dataManager.player.itemBag[4].item.description);
                break;
            case "item_6":
                UpdateMessageBoxSimple(dataManager.player.itemBag[5].item.description);
                break;
            case "item_7":
                UpdateMessageBoxSimple(dataManager.player.itemBag[6].item.description);
                break;
            case "item_8":
                UpdateMessageBoxSimple(dataManager.player.itemBag[7].item.description);
                break;
            case "item_9":
                UpdateMessageBoxSimple(dataManager.player.itemBag[8].item.description);
                break;
            case "item_10":
                UpdateMessageBoxSimple(dataManager.player.itemBag[9].item.description);
                break;
            default:
                break;
        }
    }

    public void ShowSkillDice() {
        switch (selectedSkill.type) 
        {
            case "L":
                panelD4.SetActive(true);
                diceType = "D4";
                UpdateMessageBoxSimple("Roll the Dice of Willpower (IV).\nIt's value will be added to your L stat to determine damage.");
                break;
            case "H":
                panelD20.SetActive(true);
                diceType = "D20";
                UpdateMessageBoxSimple("Roll the Dice of Willpower (XX).\nIt's value will be added to your H stat to determine damage.");
                break;
            case "S":
                panelD6.SetActive(true);
                diceType = "D6";
                UpdateMessageBoxSimple("Roll the Dice of Willpower (VI).\nIt's value will be multiplied to your S stat to determine damage.");
                break;
            default:
                break;
        }
        fader.fadeOutUIImage.gameObject.SetActive(false);
        gameoverFader.fadeOutUIImage.gameObject.SetActive(false);
    }

    public void rollDices () {
        switch(diceType) {
            case "D4":
                panelD4_button.interactable = false;
                panelD4_dice.OnPointerClick(null);

                break;
            case "2D4":
                panel2D4_button.interactable = false;
                panel2D4_dice1.OnPointerClick(null);
                panel2D4_dice2.OnPointerClick(null);
                break;
            case "D6":
                panelD6_button.interactable = false;
                panelD6_dice.OnPointerClick(null);
                break;
            case "2D6":
                panel2D6_button.interactable = false;
                panel2D6_dice1.OnPointerClick(null);
                panel2D6_dice2.OnPointerClick(null);
                break;
            case "D20":
                panelD20_button.interactable = false;
                panelD20_dice.OnPointerClick(null);
                break;
            default:
                break;
        }
        SFX.clip = soundEffects[3];
        SFX.Play();
        /*diceButton.interactable = false;
        dice.OnPointerClick(null);
        waitingForRoll = true;*/
    }

    public bool CheckForDiceFinished() {
        bool finishedRolling = false;
        switch(diceType) {
            case "D4":
                finishedRolling = panelD4_dice.finishedRolling;
                break;
            case "2D4":
                finishedRolling = panel2D4_dice1.finishedRolling && panel2D4_dice2.finishedRolling;
                break;
            case "D6":
                finishedRolling = panelD6_dice.finishedRolling;
                break;
            case "2D6":
                finishedRolling = panel2D6_dice1.finishedRolling && panel2D6_dice2.finishedRolling;
                break;
            case "D20":
                finishedRolling = panelD20_dice.finishedRolling;
                break;
            default:
                break;
        }
        return finishedRolling;
    }

    public void GetTotalDiceValue (){
        switch(diceType) {
            case "D4":
                totalDices = panelD4_dice.finalResult;
                break;
            case "2D4":
                totalDices = panel2D4_dice1.finalResult + panel2D4_dice2.finalResult;
                break;
            case "D6":
                totalDices = panelD6_dice.finalResult;
                break;
            case "2D6":
                totalDices = panel2D6_dice1.finalResult + panel2D6_dice2.finalResult;
                break;
            case "D20":
                totalDices = panelD20_dice.finalResult;
                break;
            default:
                break;
        }
    }

    public void ContinueWithAnimation() {
        switch (selectedSkill.type) {
            case "L":
                totalDamage = totalDices + dataManager.player.Light_attack_stat;
                break;
            case "H":
                totalDamage = totalDices + dataManager.player.Heavy_attack_stat;
                break;
            case "S":
                totalDamage = totalDices * dataManager.player.Special_attack_stat;
                break;
            default:
                break;
        }
        ////////SUBSTRACT AP////////////
        dataManager.player.AP = dataManager.player.AP - selectedSkill.ap_cost;
        AP_Text.text = "AP: " + dataManager.player.AP.ToString();
        //////////////////////////////
        switch (selectedSkill.type) 
        {
            case "L":
                panelD4.SetActive(false);
                panelD4_dice.ResetDice();
                panelD4_button.interactable = true;
                break;
            case "H":
                panelD20.SetActive(false);
                panelD20_dice.ResetDice();
                panelD20_button.interactable = true;
                break;
            case "S":
                panelD6.SetActive(false);
                panelD6_dice.ResetDice();
                panelD6_button.interactable = true;
                break;
            default:
                break;
        }
        diceType = "";

        if (selectedSkill.target.Equals("all")) {
            UpdateMessageBoxSimple("You used " + selectedSkill.name + " against all enemies");
        }
        else {
            UpdateMessageBoxSimple("You used " + selectedSkill.name + " against " + dataManager.battleEnemies[selectedTarget].Name);
        }
        switch (selectedSkill.type) 
        {
            case "L":
                SFX.clip = soundEffects[4];
                break;
            case "H":
                SFX.clip = soundEffects[5];
                break;
            case "S":
                SFX.clip = soundEffects[6];
                break;
            default:
                break;
        }
        SFX.Play();
        /////INSERT ANIMATION////
        string target = selectedSkill.target;
        switch (target) {
            case "select_1":
                if(dataManager.battleEnemyNumber == 2) {
                    switch (selectedTarget)
                    {
                        case 0: 
                            enemiesOnScene[1].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                            break;
                        case 1:
                            enemiesOnScene[2].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                            break;
                        default:
                            break;
                    }
                }
                else {
                    enemiesOnScene[selectedTarget].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                }
                break;
            case "all":
                for (int i = 0; i < enemiesOnScene.Length; i++) {
                    if (enemiesOnScene[i].gameObject.activeSelf) {
                        enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                    }
                }
                break;
            default:
                break;
        }
        isPlayingAnimation = true;
        //////////////
    }

    public void ResetMultiArrows() {
        multiAr1.SetActive(true);
        multiAr2.SetActive(true);
        multiAr3.SetActive(true);
    }

    public bool CheckForBlinkEnd() {

        bool isFinished = false;
        string target = selectedSkill.target;

        switch (target) {
            case "select_1":
                if(dataManager.battleEnemyNumber == 2) {
                    switch (selectedTarget)
                    {
                        case 0: 
                            isFinished = enemiesOnScene[1].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                            break;
                        case 1:
                            isFinished = enemiesOnScene[2].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                            break;
                        default:
                            break;
                    }
                }
                else {
                    isFinished = enemiesOnScene[selectedTarget].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                }
                break;
            case "all":
                isFinished = true;
                for (int i = 0; i < enemiesOnScene.Length; i++) {
                    if (enemiesOnScene[i].gameObject.activeSelf) {
                        isFinished = isFinished && enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                    }
                }
                break;
            default:
                break;
        }

        return isFinished;
    }

    public bool CheckForBlinkModificationEnd () {
        bool isFinished = false;
        string target = selectedSkill.effect.damage_modfication.target;

        switch (target) {
            case "same":
                if(dataManager.battleEnemyNumber == 2) {
                    switch (selectedTarget)
                    {
                        case 0: 
                            isFinished = enemiesOnScene[1].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                            break;
                        case 1:
                            isFinished = enemiesOnScene[2].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                            break;
                        default:
                            break;
                    }
                }
                else {
                    isFinished = enemiesOnScene[selectedTarget].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                }
                break;
            case "all":
                isFinished = true;
                for (int i = 0; i < enemiesOnScene.Length; i++) {
                    if (enemiesOnScene[i].gameObject.activeSelf) {
                        isFinished = isFinished && enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().finishedBlinking;
                    }
                }
                break;
            default:
                break;
        }

        return isFinished;
    }

    public void UpdateEnemiesHP() {
        switch (dataManager.battleEnemyNumber)
        {
            case 1:
                //Debug.Log(dataManager.battleEnemies[0].Current_HP);
                if(dataManager.battleEnemies[0].Current_HP <= 0) {
                    enemy1HP.text = "";
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[0].Name + " defeated.");
                    enemiesOnScene[0].gameObject.SetActive(false);
                    allEnemiesDead = true;
                    //Debug.Log("ALL DEAD");
                    //Invoke("Victory",0.7f);
                }
                else {
                    enemy1HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                }
                break;
            case 2:
                bool enemy1Dead = false;
                bool enemy2Dead = false;
                //Debug.Log(dataManager.battleEnemies[0].Current_HP);
                //Debug.Log(dataManager.battleEnemies[1].Current_HP);
                if(dataManager.battleEnemies[0].Current_HP <= 0 ) {
                    enemy2HP.text = "";
                    enemy1Dead = true;
                }
                else {
                    enemy2HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                }
                if(dataManager.battleEnemies[1].Current_HP <= 0) {
                    enemy3HP.text = "";
                    enemy2Dead = true;
                }
                else {
                    enemy3HP.text = "HP" + dataManager.battleEnemies[1].Current_HP.ToString();
                }
                if (enemy1Dead && enemy2Dead) {
                    UpdateMessageBoxSimple("All enemies defeated.");
                    enemiesOnScene[1].gameObject.SetActive(false);
                    enemiesOnScene[2].gameObject.SetActive(false);
                    allEnemiesDead = true;
                    //Debug.Log("ALL DEAD");
                    
                }
                else if (enemy1Dead && enemiesOnScene[1].gameObject.activeSelf == true) {
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[0].Name + " defeated.");
                    enemiesOnScene[1].gameObject.SetActive(false);
                }
                else if (enemy2Dead && enemiesOnScene[2].gameObject.activeSelf == true) {
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[1].Name + " defeated.");
                    enemiesOnScene[2].gameObject.SetActive(false);
                }
                break;
            case 3:
                //////
                bool enemy1IsDead = false;
                bool enemy2IsDead = false;
                bool enemy3IsDead = false;
                //Debug.Log(dataManager.battleEnemies[0].Current_HP);
                //Debug.Log(dataManager.battleEnemies[1].Current_HP);
                //Debug.Log(dataManager.battleEnemies[2].Current_HP);
                if(dataManager.battleEnemies[0].Current_HP <= 0) {
                    enemy1HP.text = "";
                    enemy1IsDead = true;
                }
                else {
                    enemy1HP.text = "HP" + dataManager.battleEnemies[0].Current_HP.ToString();
                }
                if(dataManager.battleEnemies[1].Current_HP <= 0) {
                    enemy2HP.text = "";
                    enemy2IsDead = true;
                }
                else {
                    enemy2HP.text = "HP" + dataManager.battleEnemies[1].Current_HP.ToString();
                }
                if(dataManager.battleEnemies[2].Current_HP <= 0) {
                    enemy3HP.text = "";
                    enemy3IsDead = true;
                }
                else {
                    enemy3HP.text = "HP" + dataManager.battleEnemies[2].Current_HP.ToString();
                }
                if (enemy1IsDead && enemy2IsDead && enemy3IsDead) {
                    UpdateMessageBoxSimple("All enemies defeated.");
                    enemiesOnScene[0].gameObject.SetActive(false);
                    enemiesOnScene[1].gameObject.SetActive(false);
                    enemiesOnScene[2].gameObject.SetActive(false);
                    allEnemiesDead = true;
                    //Debug.Log("ALL DEAD");
                    //Invoke("Victory",0.7f);
                }
                else if (enemy1IsDead && enemy2IsDead) {
                    if (enemiesOnScene[0].gameObject.activeSelf == true  && enemiesOnScene[1].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemyies " + dataManager.battleEnemies[0].Name + " and " + dataManager.battleEnemies[1].Name + " defeated.");
                        enemiesOnScene[0].gameObject.SetActive(false);
                        enemiesOnScene[1].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[0].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[0].Name + " defeated.");
                        enemiesOnScene[0].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[1].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[1].Name + " defeated.");
                        enemiesOnScene[1].gameObject.SetActive(false);
                    }
                    
                }
                else if (enemy1IsDead && enemy3IsDead) {
                    if (enemiesOnScene[0].gameObject.activeSelf == true  && enemiesOnScene[2].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemyies " + dataManager.battleEnemies[0].Name + " and " + dataManager.battleEnemies[2].Name + " defeated.");
                        enemiesOnScene[0].gameObject.SetActive(false);
                        enemiesOnScene[2].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[0].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[0].Name + " defeated.");
                        enemiesOnScene[0].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[2].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[2].Name + " defeated.");
                        enemiesOnScene[2].gameObject.SetActive(false);
                    }
                    
                }
                else if (enemy2IsDead && enemy3IsDead) {
                    if (enemiesOnScene[1].gameObject.activeSelf == true  && enemiesOnScene[2].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemyies " + dataManager.battleEnemies[1].Name + " and " + dataManager.battleEnemies[2].Name + " defeated.");
                        enemiesOnScene[1].gameObject.SetActive(false);
                        enemiesOnScene[2].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[1].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[1].Name + " defeated.");
                        enemiesOnScene[1].gameObject.SetActive(false);
                    }
                    else if (enemiesOnScene[2].gameObject.activeSelf == true) {
                        UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[2].Name + " defeated.");
                        enemiesOnScene[2].gameObject.SetActive(false);
                    }
                    
                }
                else if (enemy1IsDead && enemiesOnScene[0].gameObject.activeSelf == true) {
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[0].Name + " defeated.");
                    enemiesOnScene[0].gameObject.SetActive(false);
                }
                else if (enemy2IsDead && enemiesOnScene[1].gameObject.activeSelf == true) {
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[1].Name + " defeated.");
                    enemiesOnScene[1].gameObject.SetActive(false);
                }
                else if (enemy3IsDead && enemiesOnScene[2].gameObject.activeSelf == true) {
                    UpdateMessageBoxSimple("Enemy " + dataManager.battleEnemies[2].Name + " defeated.");
                    enemiesOnScene[2].gameObject.SetActive(false);
                }
                break;
                //////
            default:
                break;
        }
        isUpdatingEnemiesHP = false;
    }

    public void updatePlayerHP() {
        HP_text.text = "HP: " + dataManager.player.current_HP.ToString() + "/" + dataManager.player.HP.ToString();
        shield_text.text = "Shield: " + dataManager.player.currentShield.ToString();
    }

    public void ContinueWithDamage(){
        for (int i = 0; i < enemiesOnScene.Length; i++) {
            if (enemiesOnScene[i].gameObject.activeSelf) {
                enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().ResetBlink();
            }
        }
        //reduce HP from enemies.
        string target = selectedSkill.target;
        switch (target)
        {
            case "select_1":
                dataManager.battleEnemies[selectedTarget].Current_HP = dataManager.battleEnemies[selectedTarget].Current_HP - totalDamage;
                UpdateMessageBoxSimple("You did " + totalDamage + " to " + dataManager.battleEnemies[selectedTarget].Name);
                break;
            case "all":
                for(int i = 0; i < dataManager.battleEnemies.Length; i++) {
                    dataManager.battleEnemies[i].Current_HP = dataManager.battleEnemies[i].Current_HP - totalDamage;
                }
                UpdateMessageBoxSimple("You did " + totalDamage + " to all enemies");
                break;
            default:
                break;
        }
        Invoke("StartUpdatingEnemiesHP",0.5f);
        
    }

    public void finishingDamage () {
        if((selectedSkill.has_effect && 
        selectedSkill.effect.has_damage_modification && 
        selectedSkill.effect.damage_modfication.target.Equals("same")
        && dataManager.battleEnemies[selectedTarget].Current_HP <= 0) ||
        (selectedSkill.has_effect &&
        selectedSkill.effect.has_status_condition &&
        dataManager.battleEnemies[selectedTarget].Current_HP <= 0)) {
            //Debug.Log("ENEMY KILLED");
            //Debug.Log(allEnemiesDead);
            if (allEnemiesDead == false) {
                Invoke("EndingPlayerTurn",0.7f);
            }
            else {
                Invoke("Victory",0.7f);
            }
        }
        else {
            //Debug.Log("ENEMY NOT KILLED");
            //Debug.Log(allEnemiesDead);
            if (allEnemiesDead == false) {
                Invoke("ProcessAdditionalEffects",1f);
            }
            else {
                Invoke("Victory",0.7f);
            }
        }
    }

    public void ProcessAdditionalEffects() {
        if (selectedSkill.has_effect) {
            //Debug.Log("Has Effects");
            if(selectedSkill.effect.has_damage_modification){
                ProcessDamageModification();
            }
            if(selectedSkill.effect.has_status_condition){
                ProcessStatusCondition();
            }
            if(selectedSkill.effect.has_cure){
                ProcessCure();
            }
            if(selectedSkill.effect.has_damage_mitigation){
                ProcessDamageMitigation();
            }
        }
        else {
            if (allEnemiesDead == false) {
            Invoke("EndingPlayerTurn",0.7f);
        }
        }
    }

    public void ProcessDamageModification() {
        switch(selectedSkill.effect.damage_modfication.type) {
            case "variable":
                typeOfRolling = "modification";
                diceType = selectedSkill.effect.damage_modfication.modification;
                waitingForRoll = true;
                ShowEffectDice("modification");
                break;
            case "fixed":
                ContinueWithModification();
                break;
        }
    }

    public void ProcessStatusCondition() {
        switch(selectedSkill.effect.status_condition.type) {
            case "variable":
                typeOfRolling = "status";
                diceType = "D20";
                waitingForRoll = true;
                ShowEffectDice("status");
                break;
            case "fixed":
                ContinueWithStatusCondition();
                break;
        }
    }

    public void ProcessCure() {
        switch(selectedSkill.effect.cure.cure_type) {
            case "variable":
                typeOfRolling = "cure";
                diceType = selectedSkill.effect.cure.amount;
                waitingForRoll = true;
                ShowEffectDice("cure");
                break;
            case "fixed":
                ContinueWithCure();
                break;
        }
    }

    public void ProcessDamageMitigation() {
        typeOfRolling = "mitigation";
        diceType = selectedSkill.effect.damage_mitigation.amount;
        waitingForRoll = true;
        ShowEffectDice("mitigation");
    }

    public void ShowEffectDice(string origin) {
        switch (diceType) 
        {
            case "D4":
                panelD4.SetActive(true);
                UpdateMessageBoxSimple("Roll the Dice of Willpower (IV).\n");
                break;
            case "2D4":
                panel2D4.SetActive(true);
                UpdateMessageBoxSimple("Roll the Dices of Willpower (IV).\n");
                break;
            case "D6":
                panelD6.SetActive(true);
                UpdateMessageBoxSimple("Roll the Dice of Willpower (VI).\n");
                break;
            case "2D6":
                panel2D6.SetActive(true);
                UpdateMessageBoxSimple("Roll the Dices of Willpower (VI).\n");
                break;
            case "D20":
                panelD20.SetActive(true);
                UpdateMessageBoxSimple("Roll the Dice of Willpower (XX).\n");
                break;
            default:
                break;
        }
        ShowEffectMessage(origin);
    }

    public void ShowEffectMessage(string origin) {
        switch (origin) {
            case "modification":
                messageBox.text = messageBox.text + "The value will be added as damage to the target(s).";
                break;
            case "status":
                messageBox.text = messageBox.text + "Rolling a number higher than 10 will apply the status to the target(s).";
                break;
            case "cure":
                messageBox.text = messageBox.text + "The value will be restored from your current HP.";
                break;
            case "mitigation":
                messageBox.text = messageBox.text + "The value will added as a shield to your character.";
                break;
        }
    }

    public void ContinueWithModification() {
        if(selectedSkill.effect.damage_modfication.type == "variable") {
            totalDamage = totalDices;
        }
        else {
            totalDamage = Mathf.CeilToInt((float)totalDamage * float.Parse(selectedSkill.effect.damage_modfication.modification));
        }
        switch (diceType) 
        {
            case "D4":
                panelD4.SetActive(false);
                panelD4_dice.ResetDice();
                panelD4_button.interactable = true;
                break;
            case "2D4":
                panel2D4.SetActive(false);
                panel2D4_dice1.ResetDice();
                panel2D4_dice2.ResetDice();
                panel2D4_button.interactable = true;
                break;
            case "D6":
                panelD6.SetActive(false);
                panelD6_dice.ResetDice();
                panelD6_button.interactable = true;
                break;
            case "2D6":
                panel2D6.SetActive(false);
                panel2D6_dice1.ResetDice();
                panel2D6_dice2.ResetDice();
                panel2D6_button.interactable = true;
                break;
            case "D20":
                panelD20.SetActive(false);
                panelD20_dice.ResetDice();
                panelD20_button.interactable = true;
                break;
            default:
                break;
        }
        diceType = "";

        if (selectedSkill.effect.damage_modfication.target.Equals("all")) {
            UpdateMessageBoxSimple("Additional damage against all enemies");
        }
        else {
            UpdateMessageBoxSimple("Additional damage against " + dataManager.battleEnemies[selectedTarget].Name);
        }
        switch (selectedSkill.type) 
        {
            case "L":
                SFX.clip = soundEffects[4];
                break;
            case "H":
                SFX.clip = soundEffects[5];
                break;
            case "S":
                SFX.clip = soundEffects[6];
                break;
            default:
                break;
        }
        SFX.Play();
        /////INSERT ANIMATION////
        string target = selectedSkill.effect.damage_modfication.target;
        switch (target) {
            case "same":
                if(dataManager.battleEnemyNumber == 2) {
                    switch (selectedTarget)
                    {
                        case 0: 
                            enemiesOnScene[1].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                            break;
                        case 1:
                            enemiesOnScene[2].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                            break;
                        default:
                            break;
                    }
                }
                else {
                    enemiesOnScene[selectedTarget].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                }
                break;
            case "all":
                for (int i = 0; i < enemiesOnScene.Length; i++) {
                    if (enemiesOnScene[i].gameObject.activeSelf) {
                        enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().StartBlinking();
                    }
                }
                break;
            default:
                break;
        }
        isPlayingAnimationModification = true;
        //////////////
    }

    public void ContinueWithDamageModification(){
        for (int i = 0; i < enemiesOnScene.Length; i++) {
            if (enemiesOnScene[i].gameObject.activeSelf) {
                enemiesOnScene[i].gameObject.GetComponent<EnemyBlinking>().ResetBlink();
            }
        }
        //reduce HP from enemies.
        string target = selectedSkill.effect.damage_modfication.target;
        switch (target)
        {
            case "same":
                dataManager.battleEnemies[selectedTarget].Current_HP = dataManager.battleEnemies[selectedTarget].Current_HP - totalDamage;
                UpdateMessageBoxSimple("You did " + totalDamage + " additional damage to " + dataManager.battleEnemies[selectedTarget].Name);
                break;
            case "all":
                for(int i = 0; i < dataManager.battleEnemies.Length; i++) {
                    dataManager.battleEnemies[i].Current_HP = dataManager.battleEnemies[i].Current_HP - totalDamage;
                }
                UpdateMessageBoxSimple("You did " + totalDamage + " additional damage to all enemies");
                break;
            default:
                break;
        }
        Invoke("StartUpdatingEnemiesHPModification",0.5f);
    }

    public void finishingDamageModification() {
        //Debug.Log(allEnemiesDead);
        if (allEnemiesDead == false) {
            Invoke("EndingPlayerTurn",0.7f);
        }
        else {
            Invoke("Victory",0.7f);
        }
    }

    public void ContinueWithCure() {
        if(selectedSkill.effect.cure.cure_type == "variable") {
            totalDamage = totalDices;
        }
        else {
            totalDamage = Mathf.CeilToInt((float)totalDamage * float.Parse(selectedSkill.effect.cure.amount));
        }
        switch (diceType) 
        {
            case "D4":
                panelD4.SetActive(false);
                panelD4_dice.ResetDice();
                panelD4_button.interactable = true;
                break;
            case "2D4":
                panel2D4.SetActive(false);
                panel2D4_dice1.ResetDice();
                panel2D4_dice2.ResetDice();
                panel2D4_button.interactable = true;
                break;
            case "D6":
                panelD6.SetActive(false);
                panelD6_dice.ResetDice();
                panelD6_button.interactable = true;
                break;
            case "2D6":
                panel2D6.SetActive(false);
                panel2D6_dice1.ResetDice();
                panel2D6_dice2.ResetDice();
                panel2D6_button.interactable = true;
                break;
            case "D20":
                panelD20.SetActive(false);
                panelD20_dice.ResetDice();
                panelD20_button.interactable = true;
                break;
            default:
                break;
        }
        diceType = "";

        /*if (selectedSkill.effect.damage_modfication.target.Equals("all")) {
            UpdateMessageBoxSimple("Additional damage against all enemies");
        }
        else {
            UpdateMessageBoxSimple("Additional damage against " + dataManager.battleEnemies[selectedTarget].Name);
        }*/
        if (dataManager.player.current_HP + totalDamage >= dataManager.player.HP) {
            dataManager.player.current_HP = dataManager.player.HP;
        }
        else {
            dataManager.player.current_HP = dataManager.player.current_HP + totalDamage;
        }
        SFX.clip = soundEffects[7];
        SFX.Play();

        UpdateMessageBoxSimple("You healed " + totalDamage + " to yourself");
        updatePlayerHP();
        if (allEnemiesDead == false) {
            Invoke("EndingPlayerTurn",0.7f);
        }
        //////////////
    }

    public void ContinueWithStatusCondition() {

        switch (diceType) 
        {
            case "D4":
                panelD4.SetActive(false);
                panelD4_dice.ResetDice();
                panelD4_button.interactable = true;
                break;
            case "2D4":
                panel2D4.SetActive(false);
                panel2D4_dice1.ResetDice();
                panel2D4_dice2.ResetDice();
                panel2D4_button.interactable = true;
                break;
            case "D6":
                panelD6.SetActive(false);
                panelD6_dice.ResetDice();
                panelD6_button.interactable = true;
                break;
            case "2D6":
                panel2D6.SetActive(false);
                panel2D6_dice1.ResetDice();
                panel2D6_dice2.ResetDice();
                panel2D6_button.interactable = true;
                break;
            case "D20":
                panelD20.SetActive(false);
                panelD20_dice.ResetDice();
                panelD20_button.interactable = true;
                break;
            default:
                break;
        }
        diceType = "";

        string status = selectedSkill.effect.status_condition.status;
        string [] statusArray = status.Split(',');

        if (totalDices > 10 || selectedSkill.effect.status_condition.type.Equals("fixed"))
        {    
            dataManager.battleEnemies[selectedTarget].status = statusArray[0];
            //Debug.Log(selectedTarget);
            //Debug.Log(selectedEnemyStatusCondition);
            switch(dataManager.battleEnemies[selectedTarget].status) {
                    case "Poison": 
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 4;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[6];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    case "Burn":
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 3;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[1];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    case "Bleed":
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 2;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[4];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    case "Paralysis":
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 3;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[2];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    case "Sleep":
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 1;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[5];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    case "Freeze":
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 2;
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = statusImages[3];
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(true);
                        break;
                    default:
                        dataManager.battleEnemies[selectedTarget].status_condition_duration = 0;
                        break;
                }
            UpdateMessageBoxSimple("You applied " + statusArray[0] + " to " + dataManager.battleEnemies[selectedTarget].Name);
        }
        else {
            UpdateMessageBoxSimple("You failed to appply " + statusArray[0] + " to " + dataManager.battleEnemies[selectedTarget].Name);
        }
        if (allEnemiesDead == false) {
            Invoke("EndingPlayerTurn",0.7f);
        }
        //////////////
    }

    public void ContinueWithMitigation() {
        totalDamage = totalDices;
        switch (diceType) 
        {
            case "D4":
                panelD4.SetActive(false);
                panelD4_dice.ResetDice();
                panelD4_button.interactable = true;
                break;
            case "2D4":
                panel2D4.SetActive(false);
                panel2D4_dice1.ResetDice();
                panel2D4_dice2.ResetDice();
                panel2D4_button.interactable = true;
                break;
            case "D6":
                panelD6.SetActive(false);
                panelD6_dice.ResetDice();
                panelD6_button.interactable = true;
                break;
            case "2D6":
                panel2D6.SetActive(false);
                panel2D6_dice1.ResetDice();
                panel2D6_dice2.ResetDice();
                panel2D6_button.interactable = true;
                break;
            case "D20":
                panelD20.SetActive(false);
                panelD20_dice.ResetDice();
                panelD20_button.interactable = true;
                break;
            default:
                break;
        }
        diceType = "";

        /*if (selectedSkill.effect.damage_modfication.target.Equals("all")) {
            UpdateMessageBoxSimple("Additional damage against all enemies");
        }
        else {
            UpdateMessageBoxSimple("Additional damage against " + dataManager.battleEnemies[selectedTarget].Name);
        }*/
        dataManager.player.currentShield = dataManager.player.currentShield + totalDamage;

        SFX.clip = soundEffects[8];
        SFX.Play();
        UpdateMessageBoxSimple("You gained a " + totalDamage + " point shield.");
        updatePlayerHP();
        if (allEnemiesDead == false) {
            Invoke("EndingPlayerTurn",0.7f);
        }
        //////////////
    }

    public void EndingPlayerTurn() {
        fader.fadeOutUIImage.gameObject.SetActive(true);
        gameoverFader.fadeOutUIImage.gameObject.SetActive(true);
        /////reset type of roll variable///
        typeOfRolling = "normal";
        totalDamage = 0;
        ////insert player back to the end of the queue
        turnOrder.Enqueue("player");
        ////process damage status effects here////

        //////////////////////////////////////////
        /////restore 1 AP if AP isn't full (3)///
        if(dataManager.player.AP < 3) {
            dataManager.player.AP = dataManager.player.AP + 1;
            AP_Text.text = "AP: " + dataManager.player.AP.ToString();
        }
        switch(dataManager.player.status) {
            case "Poison":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (dataManager.player.current_HP - 3 < 1) {
                    dataManager.player.current_HP = 1;
                }
                else {
                    dataManager.player.current_HP -= 3;
                }
                updatePlayerHP();
                dataManager.player.status_condition_duration -= 1;
                if (dataManager.player.status_condition_duration == 0 || dataManager.player.current_HP == 1) {
                    dataManager.player.status_condition_duration = 0;
                    UpdateMessageBoxSimple("You lost 3 HP because of the Poison.\nYour Poison status has been cured!");
                    statusImage.sprite = statusImages[0];
                    dataManager.player.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("You lost 3 HP because of the Poison.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",0.7f);
                break;
            case "Burn":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (dataManager.player.current_HP - 5 < 1) {
                    dataManager.player.current_HP = 1;
                }
                else {
                    dataManager.player.current_HP -= 5;
                }
                updatePlayerHP();
                dataManager.player.status_condition_duration -= 1;
                if (dataManager.player.status_condition_duration == 0 || dataManager.player.current_HP == 1) {
                    dataManager.player.status_condition_duration = 0;
                    UpdateMessageBoxSimple("You lost 5 HP because of the Burn.\nYour Burn status has been cured!");
                    statusImage.sprite = statusImages[0];
                    dataManager.player.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("You lost 5 HP because of the Burn.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",0.7f);
                break;
            case "Bleed":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (dataManager.player.current_HP - 10 < 1) {
                    dataManager.player.current_HP = 1;
                }
                else {
                    dataManager.player.current_HP -= 10;
                }
                updatePlayerHP();
                dataManager.player.status_condition_duration -= 1;
                if (dataManager.player.status_condition_duration == 0 || dataManager.player.current_HP == 1) {
                    dataManager.player.status_condition_duration = 0;
                    UpdateMessageBoxSimple("You lost 10 HP because of the Bleed.\nYour Bleed status has been cured!");
                    statusImage.sprite = statusImages[0];
                    dataManager.player.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("You lost 10 HP because of the Bleed.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",0.7f);
                break;
            default:
                movingTurns = true;
                break;

        }
        /////keep moving turns///
        
    }

    public void Victory() {
        fader.fadeOutUIImage.gameObject.SetActive(true);
        gameoverFader.fadeOutUIImage.gameObject.SetActive(true);
        UpdateMessageBoxSimple("You won the battle!\nYou recovered 10 HP after resting a bit.\nThe enemy dropped:\n1 Small Potion");
        dataManager.player.itemBag[0].quantity += 1;
        dataManager.player.AP = 3;
        dataManager.player.currentShield = 0;
        if (dataManager.player.current_HP + 10 > dataManager.player.HP) {
            dataManager.player.current_HP = dataManager.player.HP;
        }
        else {
            dataManager.player.current_HP += 10;
        }   
        dataManager.player.status = "normal";
        Invoke("GoToMap",1.0f);
    }

    public void GoToMap() {
        //SceneManager.LoadScene("MapScene");
        StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"MapScene"));
    }

    public void StartUpdatingEnemiesHP() {
        isUpdatingEnemiesHP = true;
    }

    public void StartUpdatingEnemiesHPModification() {
        isUpdatingEnemiesHPModification = true;
    }

    public void continueAfterStatusCure () {
        ReturnToCommand();
        isPlayerTurn = true;
    }

    public void continueAfterStatusDamage () {
        movingTurns = true;
    }
    
    ////////////////ENEMY LOGIC//////////////////////
    public void StartEnemyTurn (string actualTurn){
        switch (actualTurn) {
            case "enemy0":
                //Debug.Log("enemy0");
                selectedEnemy = dataManager.battleEnemies[0];
                if(dataManager.battleEnemyNumber == 2) {
                    selectedEnemyStatusCondition = 1;
                }
                else {
                    selectedEnemyStatusCondition = 0;
                }
                
                break;
            case "enemy1": 
                //Debug.Log("enemy1");
                selectedEnemy = dataManager.battleEnemies[1];
                if(dataManager.battleEnemyNumber == 2) {
                    selectedEnemyStatusCondition = 2;
                }
                else {
                    selectedEnemyStatusCondition = 1;
                }
                break;
            case "enemy2": 
                //Debug.Log("enemy2");
                selectedEnemy = dataManager.battleEnemies[2];
                selectedEnemyStatusCondition = 2;
                break;
            default:
                break;
        }
        if (selectedEnemy.Current_HP > 0) {
            turnOrder.Enqueue(actualTurn);
            UpdateMessageBoxSimple(selectedEnemy.Name + "'s Turn!");
            Invoke("verifyEnemyCanMove",1.2f);
        }
        else {
            movingTurns = true;
        }
    }

    public void verifyEnemyCanMove() {
        if (selectedEnemy.status.Equals("Sleep") ||selectedEnemy.status.Equals("Freeze")) {
            switch(selectedEnemy.status) {
                case "Sleep":
                    if (selectedEnemy.status_condition_duration == 0) {
                        UpdateMessageBoxSimple(selectedEnemy.Name + " is cured from Sleep!");
                        selectedEnemy.status = "normal";
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                        Invoke("EnemyFollowUpTurn",1.2f);  
                    }
                    else {
                        UpdateMessageBoxSimple(selectedEnemy.Name + " is asleep. It's unable to act!");
                        selectedEnemy.status_condition_duration -= 1;
                        ///invoke the end of player turn
                        Invoke("endingEnemyTurn",1.2f); 
                    }
                    break;
                case "Freeze": 
                    if (selectedEnemy.status_condition_duration == 0) {
                        UpdateMessageBoxSimple(selectedEnemy.Name + " is cured from Freeze!");
                        selectedEnemy.status = "normal";
                        enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                        enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                        Invoke("EnemyFollowUpTurn",1.2f);  
                    }
                    else {
                        UpdateMessageBoxSimple(selectedEnemy.Name + " is frozen. It's unable to act!");
                        selectedEnemy.status_condition_duration -= 1;
                        ///invoke the end of player turn
                        Invoke("endingEnemyTurn",1.2f); 
                    }
                    break;
            }
        }
        else if (selectedEnemy.status.Equals("Paralysis")) {
            int probabilityOfCure = Random.Range(1,101);
            if (probabilityOfCure > 50 || selectedEnemy.status_condition_duration == 0) {
                UpdateMessageBoxSimple(selectedEnemy.Name + " is cured form Paralysis!");
                selectedEnemy.status = "normal";
                enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                Invoke("EnemyFollowUpTurn",1.2f);  
            }
            else {
                UpdateMessageBoxSimple(selectedEnemy.Name + " is paralyzed. It's unable to act!");
                selectedEnemy.status_condition_duration -= 1;
                Invoke("endingEnemyTurn",1.2f);  
            }
        }
        else {
            EnemyFollowUpTurn();
        }
    }

    public void EnemyFollowUpTurn() {
        //////////SELECT THE ENEMY SKILL///////////
        int skillNumber = Random.Range(1,7);
        string heavyNumbers = selectedEnemy.heavy_numbers;
        string specialNumbers = selectedEnemy.special_numbers;
        string [] heavyNumbersSplit = heavyNumbers.Split(',');
        string [] specialNumbersSplit = specialNumbers.Split(',');
        
        if (heavyNumbersSplit.Contains(skillNumber.ToString())){
            selectedEnemySkill = selectedEnemy.skills[1];
        }
        else if (specialNumbersSplit.Contains(skillNumber.ToString())) {
            selectedEnemySkill = selectedEnemy.skills[2];
        }
        else {
            selectedEnemySkill = selectedEnemy.skills[0];
        }
        //////Calculate damage depending on type of skill and stat
        totalDamage = 0;
        int enemyDice = 0;
        switch(selectedEnemySkill.type) {
            case "L":
                enemyDice = Random.Range(1,5);
                totalDamage = enemyDice + selectedEnemy.Light_attack_stat;
                SFX.clip = soundEffects[4];
                SFX.Play();
                break;
            case "H":
                enemyDice = Random.Range(1,21);
                totalDamage = enemyDice + selectedEnemy.Heavy_attack_stat;
                SFX.clip = soundEffects[5];
                SFX.Play();
                break;
            case "S":
                enemyDice = Random.Range(1,7);
                totalDamage = enemyDice * selectedEnemy.Special_attack_stat;
                SFX.clip = soundEffects[6];
                SFX.Play();
                break;
        }
        ////////Damage the player/////////
        if (dataManager.player.currentShield > 0) { //player has shield on
            int damageWithMitigation = totalDamage - dataManager.player.currentShield;
            if (damageWithMitigation < 0) { ///shield didn't broke
                dataManager.player.currentShield -= totalDamage;
                UpdateMessageBoxSimple(selectedEnemy.Name + " used " +selectedEnemySkill.name +"!\nYour shield received " + totalDamage + " damage!");
                updatePlayerHP();
            }   
            else if (damageWithMitigation == 0){ ///shield broke exactly 
                dataManager.player.currentShield = 0;
                UpdateMessageBoxSimple(selectedEnemy.Name + " used " +selectedEnemySkill.name +"!\nYour shield received " + totalDamage + " damage and it worn off!");
                updatePlayerHP();
            }
            else { ///damage passed the shield
                dataManager.player.currentShield = 0;
                dataManager.player.current_HP -= damageWithMitigation;
                if (dataManager.player.current_HP <= 0) {
                    dataManager.player.current_HP = 0;
                } 
                UpdateMessageBoxSimple(selectedEnemy.Name + " used " +selectedEnemySkill.name +"!\nYour shield received " + totalDamage + " damage and it worn off!\nYou received " + damageWithMitigation + " damage!");
                updatePlayerHP();
            }
        }
        else { //player has no shield
            dataManager.player.current_HP -= totalDamage;
            if (dataManager.player.current_HP <= 0) {
                dataManager.player.current_HP = 0;
            } 
            UpdateMessageBoxSimple(selectedEnemy.Name + " used " +selectedEnemySkill.name +"!\nYou received " + totalDamage + " damage!");
            updatePlayerHP();
        }
        //////////verify if player is dead or not to send to defeat or go with special effects////////
        if (dataManager.player.current_HP == 0) {
            Invoke("PlayerDead",1.2f);
            ///JUMP TO MESSAGE OF PLAYER DEAD AND THEN GAME OVER
        }
        else {
            ///JUMP TO ADDITIONAL EFFECTS//
            Invoke("CheckForSpecialEffectsEnemy",1.2f);
        }
    }

    public void PlayerDead() {
        UpdateMessageBoxSimple("You died...");
        Invoke("GameOver",1.2f);
    }

    public void GameOver() {
        ///LOAD GAME OVER SCENE WHEN IT'S CREATED...
        StartCoroutine(gameoverFader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"GameOverScene"));
        //SceneManager.LoadScene("GameOverScene");
    }

    public void CheckForSpecialEffectsEnemy() {
        if (selectedEnemySkill.has_effect) {
            if (selectedEnemySkill.effect.has_damage_modification) {
                int additionalDamage = Mathf.CeilToInt((float)totalDamage * float.Parse(selectedEnemySkill.effect.damage_modfication.modification));
                switch(selectedEnemySkill.type) {
                    case "L":
                        SFX.clip = soundEffects[4];
                        SFX.Play();
                        break;
                    case "H":
                        SFX.clip = soundEffects[5];
                        SFX.Play();
                        break;
                    case "S":
                        SFX.clip = soundEffects[6];
                        SFX.Play();
                        break;
                }
                if (dataManager.player.currentShield > 0) { //player has shield on
                    int damageWithMitigation = additionalDamage - dataManager.player.currentShield;
                    if (damageWithMitigation < 0) { ///shield didn't broke
                        dataManager.player.currentShield -= additionalDamage;
                        UpdateMessageBoxSimple("Your shield received " + additionalDamage + " additional damage!");
                        updatePlayerHP();
                    }   
                    else if (damageWithMitigation == 0){ ///shield broke exactly 
                        dataManager.player.currentShield = 0;
                        UpdateMessageBoxSimple("Your shield received " + additionalDamage + " additional damage!\nYour shield worn off!");
                        updatePlayerHP();
                    }
                    else { ///damage passed the shield
                        dataManager.player.currentShield = 0;
                        dataManager.player.current_HP -= damageWithMitigation;
                        if (dataManager.player.current_HP <= 0) {
                            dataManager.player.current_HP = 0;
                        } 
                        UpdateMessageBoxSimple("Your shield received " + additionalDamage + " additional damage!\nYour shield worn off!\nYou received " + damageWithMitigation + " damage");
                        updatePlayerHP();
                    }
                }
                else { //player has no shield
                    dataManager.player.current_HP -= additionalDamage;
                    if (dataManager.player.current_HP <= 0) {
                        dataManager.player.current_HP = 0;
                    } 
                    UpdateMessageBoxSimple("You received " + additionalDamage + " additional damage!");
                    updatePlayerHP();
                }

                if (dataManager.player.current_HP == 0) {
                    ///JUMP TO MESSAGE OF PLAYER DEAD AND THEN GAME OVER
                    Invoke("PlayerDead",1.2f);
                }
                else {
                    ///JUMP TO END ENEMY TURN
                    Invoke("endingEnemyTurn",1.2f);
                }
            }
            if (selectedEnemySkill.effect.has_status_condition) {
                if (selectedEnemySkill.effect.status_condition.type.Equals("fixed")) {
                    dataManager.player.status = selectedEnemySkill.effect.status_condition.status;
                    switch(dataManager.player.status) {
                        case "Poison": 
                            statusImage.sprite = statusImages[6];
                            dataManager.player.status_condition_duration = 4;
                            break;
                        case "Burn":
                            statusImage.sprite = statusImages[1];
                            dataManager.player.status_condition_duration = 3;
                            break;
                        case "Bleed":
                            statusImage.sprite = statusImages[4];
                            dataManager.player.status_condition_duration = 2;
                            break;
                        case "Paralysis":
                            statusImage.sprite = statusImages[2];
                            dataManager.player.status_condition_duration = 3;
                            break;
                        case "Sleep":
                            statusImage.sprite = statusImages[5];
                            dataManager.player.status_condition_duration = 1;
                            break;
                        case "Freeze":
                            statusImage.sprite = statusImages[3];
                            dataManager.player.status_condition_duration = 2;
                            break;
                        default:
                            statusImage.sprite = statusImages[0];
                            dataManager.player.status_condition_duration = 0;
                            break;
                    }
                    UpdateMessageBoxSimple("You have been affected with " + dataManager.player.status + "!");
                }
                else {
                    int valueForStatus = Random.Range(1,21);
                    if (valueForStatus > 10) {
                        dataManager.player.status = selectedEnemySkill.effect.status_condition.status;
                        switch(dataManager.player.status) {
                            case "Poison": 
                                statusImage.sprite = statusImages[6];
                                dataManager.player.status_condition_duration = 4;
                                break;
                            case "Burn":
                                statusImage.sprite = statusImages[1];
                                dataManager.player.status_condition_duration = 3;
                                break;
                            case "Bleed":
                                statusImage.sprite = statusImages[4];
                                dataManager.player.status_condition_duration = 2;
                                break;
                            case "Paralysis":
                                statusImage.sprite = statusImages[2];
                                dataManager.player.status_condition_duration = 3;
                                break;
                            case "Sleep":
                                statusImage.sprite = statusImages[5];
                                dataManager.player.status_condition_duration = 1;
                                break;
                            case "Freeze":
                                statusImage.sprite = statusImages[3];
                                dataManager.player.status_condition_duration = 2;
                                break;
                            default:
                                statusImage.sprite = statusImages[0];
                                dataManager.player.status_condition_duration = 0;
                                break;
                        }
                        UpdateMessageBoxSimple("You have been affected with " + dataManager.player.status + "!");
                    }
                    else {
                        UpdateMessageBoxSimple("The enemy failed to apply a status condition on you!");
                    }
                }
                Invoke("endingEnemyTurn",1.2f);
                ////JUMP TO END OF ENEMY TURN
            }
            if (selectedEnemySkill.effect.has_cure) {
                SFX.clip = soundEffects[7];
                SFX.Play();
                int enemyCure = Mathf.CeilToInt((float)totalDamage * float.Parse(selectedEnemySkill.effect.cure.amount));
                if (selectedEnemy.Current_HP + enemyCure > selectedEnemy.Max_HP){
                    selectedEnemy.Current_HP = selectedEnemy.Max_HP;
                }
                else {
                    selectedEnemy.Current_HP += enemyCure;
                }
                UpdateEnemiesHP();
                UpdateMessageBoxSimple("The enemy healed " +enemyCure+ "of HP!");
                ////jump to end turn
                Invoke("endingEnemyTurn",1.2f);
            }
        }
        else {
            endingEnemyTurn();
        }
    }

    public void endingEnemyTurn () {
        ////process damage status effects here////
        switch(selectedEnemy.status) {
            case "Poison":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (selectedEnemy.Current_HP - 3 < 1) {
                    selectedEnemy.Current_HP = 1;
                }
                else {
                    selectedEnemy.Current_HP -= 3;
                }
                UpdateEnemiesHP();
                selectedEnemy.status_condition_duration -= 1;
                if (selectedEnemy.status_condition_duration == 0 || selectedEnemy.Current_HP == 1) {
                    selectedEnemy.status_condition_duration = 0;
                    UpdateMessageBoxSimple("The enemy lost 3 HP because of the Poison.\nThe enemy Poison status has been cured!");
                    enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                    enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                    selectedEnemy.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("The enemy lost 3 HP because of the Poison.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",1.2f);
                break;
            case "Burn":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (selectedEnemy.Current_HP - 5 < 1) {
                    selectedEnemy.Current_HP = 1;
                }
                else {
                    selectedEnemy.Current_HP -= 5;
                }
                UpdateEnemiesHP();
                selectedEnemy.status_condition_duration -= 1;
                if (selectedEnemy.status_condition_duration == 0 || selectedEnemy.Current_HP == 1) {
                    selectedEnemy.status_condition_duration = 0;
                    UpdateMessageBoxSimple("The enemy lost 5 HP because of the Burn.\nThe enemy Burn status has been cured!");
                    enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                    enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                    selectedEnemy.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("The enemy lost 5 HP because of the Burn.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",1.2f);
                break;
            case "Bleed":
                SFX.clip = soundEffects[4];
                SFX.Play();
                if (selectedEnemy.Current_HP - 10 < 1) {
                    selectedEnemy.Current_HP = 1;
                }
                else {
                    selectedEnemy.Current_HP -= 10;
                }
                UpdateEnemiesHP();
                selectedEnemy.status_condition_duration -= 1;
                if (selectedEnemy.status_condition_duration == 0 || selectedEnemy.Current_HP == 1) {
                    selectedEnemy.status_condition_duration = 0;
                    UpdateMessageBoxSimple("The enemy lost 10 HP because of the Bleed.\nThe enemy Bleed status has been cured!");
                    enemyStatusConditions[selectedEnemyStatusCondition].sprite = null;
                    enemyStatusConditions[selectedEnemyStatusCondition].gameObject.SetActive(false);
                    selectedEnemy.status = "normal";
                }
                else {
                    UpdateMessageBoxSimple("The enemy lost 10 HP because of the Bleed.");
                }
                ////INVOKING THE MOVING TURNS
                Invoke("continueAfterStatusDamage",1.2f);
                break;
            default:
                movingTurns = true;
                break;

        }
        /////keep moving turns///
    }
}
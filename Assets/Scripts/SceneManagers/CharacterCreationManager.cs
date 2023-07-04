using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreationManager : MonoBehaviour
{
    public DataManager dataManager;

    public GameObject dialogWindow;

    public GameObject nameInputBox;

    public GameObject classSelector;

    public GameObject diceRoller;

    public GameObject StatAssigner;

    public GameObject SkillAssigner;

    public Text totalPointsText;

    //public bool awaitingInput = false;

    public StoryTextData textData;

    public ClassesSkillHolder playerSkillData;

    //public bool isInDialog = true;
    //bool proceedToInput = false;

    ScrollingText textScript;

    public ScreenFader fader;

    public bool[] structuredEvents = {
        false,
        false,
        false,
        false,
        false,
        false,
        false
    };

    public D4_Dice [] statDices;
    public bool waitingForRolls = false;

    public bool [] finishedDices = {
        false,false,false,false
    };

    public int [] results = {
        0,0,0,0
    };

    public Button diceButton;

    public AudioSource SFX;

    public AudioClip[] soundEffects;

    // Start is called before the first frame update
    void Start()
    {
        dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
        dataManager.player.skills = new Skill [3];
        dataManager.player.itemBag = new PlayerItem[10];
        ////////////
        string storyTextPath = Path.Combine(Application.streamingAssetsPath,"StoryTexts");
        string characterTextPath = Path.Combine(storyTextPath,"CharacterTexts.json");
        string characterTextJSON = File.ReadAllText(characterTextPath);
        textData = JsonUtility.FromJson<StoryTextData>(characterTextJSON);
        textScript = dialogWindow.GetComponent<ScrollingText>();
        string skillTextPath = Path.Combine(Application.streamingAssetsPath,"PlayerSkills");
        string playerSkillTextPath = Path.Combine(skillTextPath,"PlayerSkillFile.json");
        string playerSkillTextJSON = File.ReadAllText(playerSkillTextPath);
        playerSkillData = JsonUtility.FromJson<ClassesSkillHolder>(playerSkillTextJSON);
        string itemsTextPath = Path.Combine(Application.streamingAssetsPath,"Items");
        string itemTextPath = Path.Combine(itemsTextPath,"Items.json");
        string itemsTextJSON = File.ReadAllText(itemTextPath);
        dataManager.gameItems = JsonUtility.FromJson<ItemList>(itemsTextJSON);
        ///////////////////
        string enemiesTextPath = Path.Combine(Application.streamingAssetsPath,"Enemies");
        string enemyTextPath = Path.Combine(enemiesTextPath,"Enemies.json");
        string enemiesTextJSON = File.ReadAllText(enemyTextPath);
        dataManager.enemyList = JsonUtility.FromJson<EnemiesGrop>(enemiesTextJSON);
        ///////////////////
        string greaterEnemiesTextPath = Path.Combine(Application.streamingAssetsPath,"Enemies");
        string greaterEnemyTextPath = Path.Combine(greaterEnemiesTextPath,"GreatEnemies.json");
        string greaterEnemiesTextJSON = File.ReadAllText(greaterEnemyTextPath);
        dataManager.greaterEnemyList = JsonUtility.FromJson<EnemiesGrop>(greaterEnemiesTextJSON);
        /////////PLAYER ITEM BAG ITEMS//////////
        dataManager.player.itemBag[0] = new PlayerItem(dataManager.gameItems.items[0],1);
        dataManager.player.itemBag[1] = new PlayerItem(dataManager.gameItems.items[1],1);
        dataManager.player.itemBag[2] = new PlayerItem(dataManager.gameItems.items[2],0);
        dataManager.player.itemBag[3] = new PlayerItem(dataManager.gameItems.items[3],0);
        dataManager.player.itemBag[4] = new PlayerItem(dataManager.gameItems.items[4],0);
        dataManager.player.itemBag[5] = new PlayerItem(dataManager.gameItems.items[5],0);
        dataManager.player.itemBag[6] = new PlayerItem(dataManager.gameItems.items[6],0);
        dataManager.player.itemBag[7] = new PlayerItem(dataManager.gameItems.items[7],0);
        dataManager.player.itemBag[8] = new PlayerItem(dataManager.gameItems.items[8],0);
        dataManager.player.itemBag[9] = new PlayerItem(dataManager.gameItems.items[9],0);
        //Debug.Log(playerSkillTextJSON);
        CharacterScript();
    }

    // Update is called once per frame
    void Update()
    {
        if (structuredEvents[0] == false && textScript.currentlyDisplayingText == 1 && textScript.finishedLine == true){
            textScript.signalActivity(true);
            structuredEvents[0] = true;
            nameInputBox.SetActive(true);
            nameInputBox.GetComponentInChildren<InputField>().Select();
            nameInputBox.GetComponentInChildren<InputField>().ActivateInputField();
            //Debug.Log("HERE TO INPUT NAME");
        }
        if (structuredEvents[1] == false && textScript.currentlyDisplayingText == 5 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[1] = true;
            classSelector.SetActive(true);
        }
        if (structuredEvents[2] == false && textScript.currentlyDisplayingText == 8 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[2] = true;
            diceRoller.SetActive(true);
        }
        if (structuredEvents[3] == false && textScript.currentlyDisplayingText == 11 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[3] = true;
            startAssigning();
            StatAssigner.SetActive(true);
            //totalPointsText.gameObject.SetActive(false);
        }
        if (structuredEvents[4] == false && textScript.currentlyDisplayingText == 14 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[4] = true;
            SkillAssigner.SetActive(true);
            startLightAssignation();
            
            //totalPointsText.gameObject.SetActive(false);
        }
        if (structuredEvents[5] == false && textScript.currentlyDisplayingText == 15 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[5] = true;
            SkillAssigner.SetActive(true);
            startHeavyAssignation();
            
            //totalPointsText.gameObject.SetActive(false);
        }
        if (structuredEvents[6] == false && textScript.currentlyDisplayingText == 16 && textScript.finishedLine == true) {
            textScript.signalActivity(true);
            structuredEvents[6] = true;
            SkillAssigner.SetActive(true);
            startSpecialAssignation();
            
            //totalPointsText.gameObject.SetActive(false);
        }
        if (textScript.currentlyDisplayingText == 19 && textScript.finishedEverythingCompletely == true) {
            Invoke("toMapStage",0.7f);
        }
        if (waitingForRolls) {
            for (int i = 0; i < statDices.Length; i++) {
                finishedDices[i] = statDices[i].finishedRolling;
            }
            if (finishedDices[0] && finishedDices[1] && finishedDices[2] && finishedDices[3]){
                waitingForRolls = false;
                //int diceResult = 0;
                totalPointsText.text = "Imbued Powers:\n";
                for (int i = 0; i < statDices.Length; i++) {
                    if ( i <statDices.Length - 1) {
                        totalPointsText.text = totalPointsText.text + statDices[i].finalResult + "\t\t";
                    }
                    else {
                        totalPointsText.text = totalPointsText.text + statDices[i].finalResult;
                    }
                    results[i] = statDices[i].finalResult;
                }
                //Debug.Log(diceResult);
                //totalPointsText.text = "Total: " + diceResult;
                Invoke("continueStory",1f);
            }
        }



    }

    void CharacterScript () {
        startDialogue(textData);
    }

    public void startDialogue(StoryTextData text) {
        dialogWindow.SetActive(true);
        textScript.setTextToShow(text);
    }

    public void savePlayerName() {
        SFX.clip = soundEffects[1];
        SFX.Play();
        if (string.IsNullOrEmpty(nameInputBox.GetComponentInChildren<InputField>().text)) {
            dataManager.player.Name = "Asuka";
        }
        else {
            dataManager.player.Name = nameInputBox.GetComponentInChildren<InputField>().text;
        }
        /////Change variable in all texts so Player.Name displays player name.
        textScript.ChangeVariableInText("Player.Name",dataManager.player.Name);
        nameInputBox.SetActive(false);
        textScript.signalActivity(false);
    }

    public void savePlayerClass() {
        SFX.clip = soundEffects[1];
        SFX.Play();
        Selector selection = classSelector.GetComponentInChildren<Selector>();
        string className = selection.classText.text.Substring(0,1) + selection.classText.text.Substring(1).ToLower();
        dataManager.player.Class = className; 
        dataManager.player.image = selection.selectedImage;
        if(selection.selectedImage == 0 || selection.selectedImage == 3) {
            dataManager.player.canEvade = true;
        }
        else {
            dataManager.player.canEvade = false;
        }
        selection.classPanel.SetActive(false);
        dataManager.player.HP = 60;
        dataManager.player.current_HP = 60;
        dataManager.player.Light_attack_stat = 0;
        dataManager.player.Heavy_attack_stat = 0;
        dataManager.player.Special_attack_stat = 0;
        switch (selection.selectedImage)
        {
            case 0:
                dataManager.player.Light_attack_stat++;
                break;
            case 1:
                dataManager.player.Heavy_attack_stat++;
                break;
            case 2:
                dataManager.player.HP = dataManager.player.HP + 20;
                dataManager.player.current_HP = dataManager.player.current_HP + 20;
                break;
            case 3:
                dataManager.player.Special_attack_stat++;
                break;
            default:
                break;
        }
        totalPointsText.gameObject.SetActive(true);
        classSelector.SetActive(false);
        textScript.signalActivity(false);
    }

    public void rollDices () {
        diceButton.interactable = false;
        SFX.clip = soundEffects[3];
        SFX.Play();
        for (int i = 0; i < statDices.Length; i++) {
            statDices[i].OnPointerClick(null);
        }
        waitingForRolls = true;
    }

    public void continueStory() {
        diceRoller.SetActive(false);
        textScript.signalActivity(false);
    }

    public void startAssigning () {
        StatAssignationScript assignation = StatAssigner.GetComponent<StatAssignationScript>();
        assignation.startValues(results[0],results[1],results[2],results[3]);
    }

    public void SaveAssignedStatsOnPlayer () {
        SFX.clip = soundEffects[1];
        SFX.Play();
        StatAssignationScript assignation = StatAssigner.GetComponent<StatAssignationScript>();
        int assigned_hp = int.Parse(assignation.hp_text.text);
        int assigned_l = int.Parse(assignation.l_text.text);
        int assigned_h = int.Parse(assignation.h_text.text);
        int assigned_s = int.Parse(assignation.s_text.text);

        dataManager.player.HP += (assigned_hp*20);
        dataManager.player.current_HP += (assigned_hp*20);
        dataManager.player.Light_attack_stat += assigned_l;
        dataManager.player.Heavy_attack_stat += assigned_h;
        dataManager.player.Special_attack_stat += assigned_s;
        StatAssigner.SetActive(false);
        textScript.signalActivity(false);
        totalPointsText.gameObject.SetActive(false);
    }

    public void startLightAssignation () {
        int playerClass = dataManager.player.image;
        SkillAssignationScript skillAssignation = SkillAssigner.GetComponent<SkillAssignationScript>();
        skillAssignation.CreateAssignation(playerSkillData.playerSkills[playerClass].class_skills[0].skills,dataManager.player.Light_attack_stat,"L");
    }

    public void startHeavyAssignation () {
        int playerClass = dataManager.player.image;
        SkillAssignationScript skillAssignation = SkillAssigner.GetComponent<SkillAssignationScript>();
        skillAssignation.CreateAssignation(playerSkillData.playerSkills[playerClass].class_skills[1].skills,dataManager.player.Heavy_attack_stat,"H");
    }

    public void startSpecialAssignation () {
        int playerClass = dataManager.player.image;
        SkillAssignationScript skillAssignation = SkillAssigner.GetComponent<SkillAssignationScript>();
        skillAssignation.CreateAssignation(playerSkillData.playerSkills[playerClass].class_skills[2].skills,dataManager.player.Special_attack_stat,"S");
    }

    public void AssignPlayerSkill () {
        SFX.clip = soundEffects[1];
        SFX.Play();
        string skilltype = SkillAssigner.GetComponent<SkillAssignationScript>().skillType;
        //Debug.Log(skilltype);
        switch (skilltype)
        {
            case "L":
                dataManager.player.skills[0] = SkillAssigner.GetComponent<SkillAssignationScript>().chosenSkill;
                break;
            case "H":
                dataManager.player.skills[1] = SkillAssigner.GetComponent<SkillAssignationScript>().chosenSkill;
                break;
            case "S":
                dataManager.player.skills[2] = SkillAssigner.GetComponent<SkillAssignationScript>().chosenSkill;
                break;
            default:
                break;
        }
        SkillAssigner.GetComponent<SkillAssignationScript>().confirmationPanel.SetActive(false);
        SkillAssigner.SetActive(false);
        SkillAssigner.GetComponent<SkillAssignationScript>().ResetObject();
        textScript.signalActivity(false);
        
    }

    public void toMapStage () {
        StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"MapScene"));
        //SceneManager.LoadScene("MapScene");
    }
}

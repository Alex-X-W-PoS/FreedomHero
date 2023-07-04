using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapScript : MonoBehaviour
{
    public DataManager dataManager;

    public BlockRow [] rows;

    public TileGeneratorScript tileGenerator;

    public Vector3 playerPosition;

    public Sprite[] imageArray;

    //public int blocksInMap = 1;

    //public int greaterEnemiesOnMap = 0;

    public bool eventTrigger = false;

    public float speed = 10f;

    public GameObject player;

    public bool movingCharacter = false;
    
    public Vector3 newPlayerPosition; 

    //public int counterNonEventBlocks = 0;

    //public int numberOfItemsOnMap = 0;

    //public int enemiesOnMap = 0;

    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject rightArrow;
    public GameObject leftArrow;

    public GameObject dicePanel;

    public int movementLeft = 0;
    public D4_Dice dice;

    public bool waitingForRoll = false;

    public Text movement;

    public bool startingTurn = true;
    public bool isProcessingEvent = false;
    public Image playerImage;
    public Text playerName;
    public Text playerHPShowcase;

    public Sprite [] characterOnMapSprite; 

    public BlockData newBlockData;

    public RectTransform eventCard;
    public GameObject eventCardPannel;
    public bool finishedDrawingCard = false;

    public GameObject eventEffectPanel;
    public Text eventEffectText;
    public EventData currentEvent = null;

    public GameObject itemEventPanel;
    public Text itemReceivingText;

    public GameObject pausePanel;

    public bool isPaused = false;

    public Button diceButton;

    public ScreenFader fader;

    public bool isTest;
    // Start is called before the first frame update

    public AudioSource SFX;

    public AudioClip[] soundEffects;
    void Start()
    {
        isTest = false;
        if (isTest) {
            int playerDataImg = 3;
            //playerImage.sprite = dataManager.playerImages[playerDataImg];
            playerName.text = "Asuka";
            playerHPShowcase.text = "HP: 80/80";
            tileGenerator = new TileGeneratorScript();
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<SpriteRenderer>().sprite = characterOnMapSprite[playerDataImg];
            newPlayerPosition = playerPosition;
        }
        else {
            dataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<DataManager>();
            int playerDataImg = dataManager.player.image;
            playerImage.sprite = dataManager.playerImages[playerDataImg];
            playerName.text = dataManager.player.Name;
            playerHPShowcase.text = "HP: " + dataManager.player.current_HP + "/" + dataManager.player.HP;
            tileGenerator = new TileGeneratorScript();
            ///GENERATING THE MAP//
            if(dataManager.hasRowData) {
                for(int i = 0; i < this.rows.Length; i++) {
                    for(int j = 0; j < this.rows[i].blocks.Length; j++){
                        this.rows[i].blocks[j].SetBlockData(dataManager.rows[i][j]);
                        this.rows[i].blocks[j].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[dataManager.rows[i][j].imageNumber];
                        //dataManager.rows[i][j] = new BlockData(this.rows[i].blocks[j].isVisible,this.rows[i].blocks[j].imageNumber,this.rows[i].blocks[j].canMoveUp,this.rows[i].blocks[j].canMoveDown,this.rows[i].blocks[j].canMoveRight,this.rows[i].blocks[j].canMoveLeft,this.rows[i].blocks[j].type);
                    }
                }
                //playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<SpriteRenderer>().sprite = characterOnMapSprite[playerDataImg];
                player.transform.position = dataManager.playerPosition;
                playerPosition = dataManager.playerPosition;
                newPlayerPosition = dataManager.newPlayerPosition;
            }
            else {
                playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
                player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<SpriteRenderer>().sprite = characterOnMapSprite[playerDataImg];
                newPlayerPosition = playerPosition;
                /////RESETTING VALUES////
                dataManager.playerPosition = playerPosition;
                dataManager.blocksInMap = 1;
                dataManager.greaterEnemiesOnMap = 0;
                dataManager.newPlayerPosition = newPlayerPosition;
                dataManager.counterNonEventBlocks = 0;
                dataManager.numberOfItemsOnMap = 0;
                dataManager.enemiesOnMap = 0;
                
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPaused) {
            if (startingTurn) {
                dicePanel.SetActive(true);
                startingTurn = false;
            }
            if(waitingForRoll) {
                if(dice.finishedRolling) {
                    movementLeft = dice.finalResult;
                    waitingForRoll = false;
                    Invoke("continueTurn",1f);
                }
            }
            if (eventTrigger) { 
                if (Input.GetKeyDown("up")) { //Creating a new block
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    eventTrigger = false;
                    Vector3 currentGrindPosition =tileGenerator.WorldToGrid(playerPosition,true);
                    int currentX = (int) currentGrindPosition.x;
                    int currentY = (int) currentGrindPosition.y;
                    if(checkMovement(currentX,currentY,"up")) {
                        hideArrows();
                        newPlayerPosition = new Vector3 (playerPosition.x+2,playerPosition.y+1,playerPosition.z);
                        Vector3 newGridPosition = tileGenerator.WorldToGrid(newPlayerPosition,true);      
                        int x = (int) newGridPosition.x;
                        int y = (int) newGridPosition.y;
                        if (rows[x].blocks[y].isVisible == false) {
                            newBlockData = tileGenerator.generateRandomBlock(x,y,"up",dataManager.blocksInMap,dataManager.greaterEnemiesOnMap,dataManager.counterNonEventBlocks,dataManager.numberOfItemsOnMap,dataManager.enemiesOnMap);
                            rows[x].blocks[y].SetBlockData(newBlockData);
                            rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[newBlockData.imageNumber];
                            dataManager.blocksInMap++;
                            if(newBlockData.type != "normal") {
                                dataManager.counterNonEventBlocks = 0;
                            }
                            else {
                                dataManager.counterNonEventBlocks++;
                            }
                            if(newBlockData.type == "greater_enemy") {
                                dataManager.greaterEnemiesOnMap++;
                            }
                            if(newBlockData.type == "enemy") {
                                dataManager.enemiesOnMap++;
                            }
                            if(newBlockData.type == "store") {
                                dataManager.numberOfItemsOnMap++;
                            }
                            Invoke("movePlayer",0.75f);
                        }
                        else if (rows[x].blocks[y].isVisible && rows[x].blocks[y].canMoveDown) {
                            Invoke("movePlayer",0f);
                        }
                        movementLeft--;
                        updateText();
                    }  
                    else {
                        eventTrigger = true;
                    }
                }
                if (Input.GetKeyDown("down")) { //Creating a new block
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    eventTrigger = false;
                    Vector3 currentGrindPosition =tileGenerator.WorldToGrid(playerPosition,true);
                    int currentX = (int) currentGrindPosition.x;
                    int currentY = (int) currentGrindPosition.y;
                    if(checkMovement(currentX,currentY,"down")) {
                        //eventTrigger = false;
                        hideArrows();
                        newPlayerPosition = new Vector3 (playerPosition.x-2,playerPosition.y-1,playerPosition.z);
                        Vector3 newGridPosition = tileGenerator.WorldToGrid(newPlayerPosition,true);
                        int x = (int) newGridPosition.x;
                        int y = (int) newGridPosition.y;
                        if (rows[x].blocks[y].isVisible == false) {
                            newBlockData = tileGenerator.generateRandomBlock(x,y,"down",dataManager.blocksInMap,dataManager.greaterEnemiesOnMap,dataManager.counterNonEventBlocks,dataManager.numberOfItemsOnMap,dataManager.enemiesOnMap);
                            rows[x].blocks[y].SetBlockData(newBlockData);
                            rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[newBlockData.imageNumber];
                            dataManager.blocksInMap++;
                            if(newBlockData.type != "normal") {
                                dataManager.counterNonEventBlocks = 0;
                            }
                            else {
                                dataManager.counterNonEventBlocks++;
                            }
                            if(newBlockData.type == "greater_enemy") {
                                dataManager.greaterEnemiesOnMap++;
                            }
                            if(newBlockData.type == "enemy") {
                                dataManager.enemiesOnMap++;
                            }
                            if(newBlockData.type == "store") {
                                dataManager.numberOfItemsOnMap++;
                            }
                            Invoke("movePlayer",0.75f);
                        }
                        else if (rows[x].blocks[y].isVisible && rows[x].blocks[y].canMoveUp) {
                            Invoke("movePlayer",0f);
                        }
                        movementLeft--;
                        updateText();
                    } 
                    else {
                        eventTrigger = true;
                    }
                }
                if (Input.GetKeyDown("right")) { //Creating a new block
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    eventTrigger = false;
                    Vector3 currentGrindPosition =tileGenerator.WorldToGrid(playerPosition,true);
                    int currentX = (int) currentGrindPosition.x;
                    int currentY = (int) currentGrindPosition.y;
                    if(checkMovement(currentX,currentY,"right")) {
                        //eventTrigger = false;
                        hideArrows();
                        newPlayerPosition = new Vector3 (playerPosition.x+2,playerPosition.y-1,playerPosition.z);
                        Vector3 newGridPosition = tileGenerator.WorldToGrid(newPlayerPosition,true);
                        int x = (int) newGridPosition.x;
                        int y = (int) newGridPosition.y;
                        if (rows[x].blocks[y].isVisible == false) {
                            newBlockData = tileGenerator.generateRandomBlock(x,y,"right",dataManager.blocksInMap,dataManager.greaterEnemiesOnMap,dataManager.counterNonEventBlocks,dataManager.numberOfItemsOnMap,dataManager.enemiesOnMap);
                            rows[x].blocks[y].SetBlockData(newBlockData);
                            rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[newBlockData.imageNumber];
                            dataManager.blocksInMap++;
                            if(newBlockData.type != "normal") {
                                dataManager.counterNonEventBlocks = 0;
                            }
                            else {
                                dataManager.counterNonEventBlocks++;
                            }
                            if(newBlockData.type == "greater_enemy") {
                                dataManager.greaterEnemiesOnMap++;
                            }
                            if(newBlockData.type == "enemy") {
                                dataManager.enemiesOnMap++;
                            }
                            if(newBlockData.type == "store") {
                                dataManager.numberOfItemsOnMap++;
                            }
                            Invoke("movePlayer",0.75f);
                        }
                        else if (rows[x].blocks[y].isVisible && rows[x].blocks[y].canMoveLeft) {
                            Invoke("movePlayer",0f);
                        }
                        movementLeft--;
                        updateText();
                    } 
                    else {
                        eventTrigger = true;
                    }
                }
                if (Input.GetKeyDown("left")) { //Creating a new block
                    SFX.clip = soundEffects[0];
                    SFX.Play();
                    eventTrigger = false;
                    Vector3 currentGrindPosition =tileGenerator.WorldToGrid(playerPosition,true);
                    int currentX = (int) currentGrindPosition.x;
                    int currentY = (int) currentGrindPosition.y;
                    if(checkMovement(currentX,currentY,"left")) {
                        //eventTrigger = false;
                        hideArrows();
                        newPlayerPosition = new Vector3 (playerPosition.x-2,playerPosition.y+1,playerPosition.z);
                        Vector3 newGridPosition = tileGenerator.WorldToGrid(newPlayerPosition,true);
                        int x = (int) newGridPosition.x;
                        int y = (int) newGridPosition.y;
                        if (rows[x].blocks[y].isVisible == false) {
                            newBlockData = tileGenerator.generateRandomBlock(x,y,"left",dataManager.blocksInMap,dataManager.greaterEnemiesOnMap,dataManager.counterNonEventBlocks,dataManager.numberOfItemsOnMap,dataManager.enemiesOnMap);
                            rows[x].blocks[y].SetBlockData(newBlockData);
                            rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[newBlockData.imageNumber];
                            dataManager.blocksInMap++;
                            if(newBlockData.type != "normal") {
                                dataManager.counterNonEventBlocks = 0;
                            }
                            else {
                                dataManager.counterNonEventBlocks++;
                            }
                            if(newBlockData.type == "greater_enemy") {
                                dataManager.greaterEnemiesOnMap++;
                            }
                            if(newBlockData.type == "enemy") {
                                dataManager.enemiesOnMap++;
                            }
                            if(newBlockData.type == "store") {
                                dataManager.numberOfItemsOnMap++;
                            }
                            Invoke("movePlayer",0.75f);
                        }
                        else if (rows[x].blocks[y].isVisible && rows[x].blocks[y].canMoveRight) {
                            Invoke("movePlayer",0f);
                        }
                        movementLeft--;
                        updateText();
                    } 
                    else {
                        eventTrigger = true;
                    }
                }
            }
            if(movingCharacter) {
                player.transform.position = Vector3.MoveTowards(player.transform.position, newPlayerPosition, speed * Time.deltaTime);
                if (Vector3.Distance(player.transform.position, newPlayerPosition) < 0.001f)
                {
                    player.transform.position = newPlayerPosition;
                    playerPosition = newPlayerPosition;
                    movingCharacter = false;      
                    Vector3 GridPosition;
                    int x;
                    int y;
                    GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
                    x = (int) GridPosition.x;
                    y = (int) GridPosition.y;
                    if (rows[x].blocks[y].type != "normal"){
                        ProcessEvent(rows[x].blocks[y].type);
                        //isProcessingEvent = true;
                    }    
                    else {
                        ContinueMovement();
                    }  
                }
            }
            /*if(isProcessingEvent) {
                isProcessingEvent = false;
                ProcessEvent();
            }*/
            if(finishedDrawingCard) {
                finishedDrawingCard = false;
                GetEventEffect();
            }
        }
    }

    public void movePlayer () {
        //eventTrigger = false;
        movingCharacter = true;
    }

    public void showArrows(int actualX, int actualY) {
        Block currentBlock = rows[actualX].blocks[actualY];
        if (actualX == 0) {
            upArrow.SetActive(false);
        }
        else {
            upArrow.SetActive(currentBlock.canMoveUp && (rows[actualX-1].blocks[actualY].canMoveDown || !rows[actualX-1].blocks[actualY].isVisible));
        }

        if (actualX == 8) {
            downArrow.SetActive(false);
        }
        else {
            downArrow.SetActive(currentBlock.canMoveDown && (rows[actualX+1].blocks[actualY].canMoveUp || !rows[actualX+1].blocks[actualY].isVisible));
        }

        if(actualY == 8) {
            rightArrow.SetActive(false);
        }
        else {
            rightArrow.SetActive(currentBlock.canMoveRight && (rows[actualX].blocks[actualY+1].canMoveLeft || !rows[actualX].blocks[actualY+1].isVisible));
        }

        if(actualY == 0) {
            leftArrow.SetActive(false);
        }
        else {
            leftArrow.SetActive(currentBlock.canMoveLeft && (rows[actualX].blocks[actualY-1].canMoveRight || !rows[actualX].blocks[actualY-1].isVisible));
        }
    }

    public void hideArrows() {
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        rightArrow.SetActive(false);
        leftArrow.SetActive(false);
    }

    public bool checkMovement(int actualX, int actualY, string direction) {
        Block currentBlock = rows[actualX].blocks[actualY];
        bool canMove = false;
        switch (direction)
        {  
            case "up":
                if (actualX == 0) {
                    canMove = false;
                }
                else {
                    canMove = (currentBlock.canMoveUp && (rows[actualX-1].blocks[actualY].canMoveDown || !rows[actualX-1].blocks[actualY].isVisible));
                }
                break;
            case "down":
                if (actualX == 8) {
                    canMove = false;
                }
                else {
                    canMove = (currentBlock.canMoveDown && (rows[actualX+1].blocks[actualY].canMoveUp || !rows[actualX+1].blocks[actualY].isVisible));
                }
                break;
            case "right":
                if(actualY == 8) {
                    canMove = false;
                }
                else {
                    canMove = (currentBlock.canMoveRight && (rows[actualX].blocks[actualY+1].canMoveLeft || !rows[actualX].blocks[actualY+1].isVisible));
                }
                break;
            case "left":
                if(actualY == 0) {
                    canMove = false;
                }
                else {
                    canMove = (currentBlock.canMoveLeft && (rows[actualX].blocks[actualY-1].canMoveRight || !rows[actualX].blocks[actualY-1].isVisible));
                }
                break;
            default:
                break;
        }
        return canMove;
    }

    public void rollDices () {
        SFX.clip = soundEffects[3];
        SFX.Play();
        diceButton.interactable = false;
        dice.OnPointerClick(null);
        waitingForRoll = true;
    }

    public void CallToShowArrows() {
        Vector3 actualPlayerPosition =tileGenerator.WorldToGrid(playerPosition,true);
        int actualX = (int) actualPlayerPosition.x;
        int actualY = (int) actualPlayerPosition.y;
        showArrows(actualX, actualY);
    }

    public void continueTurn() {
        dice.finishedRolling = false;
        dicePanel.SetActive(false);
        diceButton.interactable = true;
        updateText();
        CallToShowArrows();
        eventTrigger = true;
    }

    public void updateText() {
        movement.text = movementLeft.ToString();
    }

    public void ProcessEvent(string eventType) {
        switch (eventType)
        {
            case "event":
                eventCardPannel.SetActive(true);
                break;
            case "trap":
                ProcessTrap();
                break;
            case "enemy":
                /////ENTER COMBAT SCENE///
                MoveToCombat(eventType);
                //ContinueMovement();
                break;
            case "greater_enemy":
                /////ENTER COMBAT SCENE///
                MoveToCombat(eventType);
                break;
            case "exit":
                /////WIN GAME/////
                Victory();
                break;
            case "store":
                ItemEvent();
                //ContinueMovement();
                break;
            default:
                ContinueMovement();
                break;
        }
        
        
    }

    public void DrawCard(){

        StartCoroutine("MoveCard");
    }

    public IEnumerator MoveCard() {
        Vector3 newLocalPosition = new Vector3(-709.7f,-393.7f,0f);
        while (Vector3.Distance(eventCard.localPosition, newLocalPosition) > 0.001f){
            eventCard.localPosition = Vector3.MoveTowards(eventCard.localPosition, newLocalPosition, 2500 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        finishedDrawingCard = true;    
    }

    public void GetEventEffect() {
        EventCardScript eventGenerator = new EventCardScript();
        currentEvent = eventGenerator.GenerateRandomEvent(dataManager);
        eventEffectText.text = currentEvent.description_text;
        eventCardPannel.SetActive(false);
        /////resetting card to its position
        eventCard.localPosition = new Vector3(-9.7f,7.7f,0f);
        ///////////////////////////////////
        eventEffectPanel.SetActive(true);

    }

    public void ApplyEventAndMoveOn () {
        string eventType = currentEvent.type;
        Vector3 GridPosition;
        int x;
        int y;
        switch (eventType)
        {
            case "item":
                dataManager.player.itemBag[currentEvent.itemCode].quantity ++;
                GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
                x = (int) GridPosition.x;
                y = (int) GridPosition.y;
                rows[x].blocks[y].type = "normal";
                rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[rows[x].blocks[y].imageNumber-15];
                rows[x].blocks[y].imageNumber = rows[x].blocks[y].imageNumber-15;
                break;
            case "encounter":
                MoveToCombat("event");
                break;
            case "heal":
                if (currentEvent.hp_amount + dataManager.player.current_HP >= dataManager.player.HP) {
                    dataManager.player.current_HP = dataManager.player.HP;
                }
                else {
                    dataManager.player.current_HP += currentEvent.hp_amount;
                }
                GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
                x = (int) GridPosition.x;
                y = (int) GridPosition.y;
                rows[x].blocks[y].type = "normal";
                rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[rows[x].blocks[y].imageNumber-15];
                playerHPShowcase.text = "HP: " + dataManager.player.current_HP + "/" + dataManager.player.HP;
                rows[x].blocks[y].imageNumber = rows[x].blocks[y].imageNumber-15;
                break;
            default:
                if (dataManager.player.current_HP - currentEvent.hp_amount <= 0) {
                    dataManager.player.current_HP = 1;
                }
                else {
                    dataManager.player.current_HP -= currentEvent.hp_amount;
                }
                SFX.clip = soundEffects[5];
                SFX.Play();
                GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
                x = (int) GridPosition.x;
                y = (int) GridPosition.y;
                rows[x].blocks[y].type = "trap";
                rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[rows[x].blocks[y].imageNumber+45];
                playerHPShowcase.text = "HP: " + dataManager.player.current_HP + "/" + dataManager.player.HP;
                rows[x].blocks[y].damageToPlayer = currentEvent.hp_amount;
                rows[x].blocks[y].imageNumber = rows[x].blocks[y].imageNumber+45;
                break;
        }
        currentEvent = null;
        eventEffectPanel.SetActive(false);
        ContinueMovement();
    }

    public void ProcessTrap() {
        Vector3 GridPosition;
        int x;
        int y;
        GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
        x = (int) GridPosition.x;
        y = (int) GridPosition.y;
        int hpDamage = rows[x].blocks[y].damageToPlayer;
        //Debug.Log(rows[x].blocks[y].damageToPlayer);
        if (dataManager.player.current_HP - hpDamage <= 0) {
            dataManager.player.current_HP = 1;
        }
        else {
            dataManager.player.current_HP -= hpDamage;
        }
        SFX.clip = soundEffects[5];
        SFX.Play();
        /////////play sound//////////////
        playerHPShowcase.text = "HP: " + dataManager.player.current_HP + "/" + dataManager.player.HP;
        ContinueMovement();
        
    }

    public void ItemEvent() {
        EventCardScript eventGenerator = new EventCardScript();
        int itemId = 0;
        int probability = Random.Range(1,101);
        if(probability >85) {
            itemId = 6;
        }
        else if (probability >75) {
            itemId = 5;
        }
        else if (probability >60) {
            itemId = 4;
        }
        else if (probability >45) {
            itemId = 3;
        }
        else if (probability >40) {
            itemId = 2;
        }
        else if (probability >25) {
            itemId = 1;
        }
        dataManager.player.itemBag[itemId].quantity ++;
        itemReceivingText.text = eventGenerator.ReturnItemPanelText(dataManager.gameItems.items[itemId]);
        itemEventPanel.SetActive(true);
    }

    public void AfterReceivingItem() {
        Vector3 GridPosition;
        int x;
        int y;
        GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
        x = (int) GridPosition.x;
        y = (int) GridPosition.y;
        rows[x].blocks[y].type = "normal";
        rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[rows[x].blocks[y].imageNumber-75];
        rows[x].blocks[y].imageNumber = rows[x].blocks[y].imageNumber-75;
        itemEventPanel.SetActive(false);
        ContinueMovement();
    }

    public void pauseMenu() {
        SFX.clip = soundEffects[1];
        SFX.Play();
        isPaused = true;
        pausePanel.SetActive(true);
    }

    public void resumeGame() {
        SFX.clip = soundEffects[2];
        SFX.Play();
        pausePanel.SetActive(false);
        isPaused = false;    
    }

    public void QuitGame() {
        SFX.clip = soundEffects[1];
        SFX.Play();
        Application.Quit();
    }

    public void ContinueMovement() {
        if (movementLeft == 0) {
            eventTrigger = false;
            startingTurn = true;
        }
        else {
            eventTrigger = true;
            CallToShowArrows();
        }
    }

    public void MoveToCombat(string eventType) {
        //DEPENDING IF IT'S A STRONG ENEMY OR ENEMY SEARCH THE ARRAYS FOR THE ITEMS
        int numberOfEnemies = 0;
        if (eventType.Equals("enemy") || eventType.Equals("event")) {
            int probability = Random.Range(1, 101);
            if (probability > 75) {
                numberOfEnemies = 3;
            }
            else if (probability > 35) {
                numberOfEnemies = 2;
            }
            else {
                numberOfEnemies = 1;
            }
            dataManager.battleEnemyNumber = numberOfEnemies;
            for (int k = 0; k< numberOfEnemies; k++) {
                int randomEnemy = Random.Range(0, 3);
                dataManager.battleEnemies[k] = new BattleEnemy(dataManager.enemyList.enemies[randomEnemy]);
            }
            dataManager.battleMusicToChoose = 0;
        }
        else { ///greater enemy
            dataManager.battleEnemyNumber = 1;
            dataManager.battleEnemies[0] = new BattleEnemy(dataManager.greaterEnemyList.enemies[dataManager.greaterEnemiesOnMap-1]);
            dataManager.battleMusicToChoose = 1;
        }
        /////THE TILE OF BATTLE RETURNS TO NORMAL//
        Vector3 GridPosition;
        int x;
        int y;
        GridPosition = tileGenerator.WorldToGrid(playerPosition,true);      
        x = (int) GridPosition.x;
        y = (int) GridPosition.y;
        rows[x].blocks[y].type = "normal";
        int numberToSubstract = 45;
        if (eventType.Equals("enemy")) {
            numberToSubstract = 30;
        }
        else if (eventType.Equals("event")) {
            numberToSubstract = 15;
        }
        rows[x].blocks[y].gameObject.GetComponent<SpriteRenderer>().sprite = imageArray[rows[x].blocks[y].imageNumber-numberToSubstract];
        rows[x].blocks[y].imageNumber = rows[x].blocks[y].imageNumber-numberToSubstract;
        //SAVE THE MAP, AND POSITION. MOVEMENT WILL RESET TO 0 AT LANDING ON COMBAT
        for(int i = 0; i < this.rows.Length; i++) {
            for(int j = 0; j < this.rows[i].blocks.Length; j++){
                dataManager.rows[i][j] = new BlockData(this.rows[i].blocks[j].isVisible,this.rows[i].blocks[j].imageNumber,this.rows[i].blocks[j].canMoveUp,this.rows[i].blocks[j].canMoveDown,this.rows[i].blocks[j].canMoveRight,this.rows[i].blocks[j].canMoveLeft,this.rows[i].blocks[j].type,this.rows[i].blocks[j].damageToPlayer);
            }
        }
        dataManager.hasRowData = true;
        dataManager.playerPosition = this.playerPosition;
        dataManager.newPlayerPosition = this.newPlayerPosition;
        StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"CombatScene"));
        //SceneManager.LoadScene("CombatScene");
    }

    public void Victory () {
        dataManager.hasRowData = false;
        StartCoroutine(fader.FadeAndLoadScene(ScreenFader.FadeDirection.In,"GameClearScene"));
    }
}

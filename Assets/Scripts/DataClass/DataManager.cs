using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public PlayerData player;

    public Sprite[] playerImages;

    public ItemList gameItems;

    public EnemiesGrop enemyList;

    public EnemiesGrop greaterEnemyList;

    public int battleEnemyNumber;

    public BattleEnemy[] battleEnemies = new BattleEnemy[3];

    //////////MOVING MAP DATA TO MANAGER SPACE FOR KEEPING BETWEEN SCENES/////////////
    public BlockData[][] rows = new BlockData[9][];
    public bool hasRowData = false;
    public Vector3 playerPosition;
    public int blocksInMap = 1;
    public int greaterEnemiesOnMap = 0;
    public Vector3 newPlayerPosition;
    public int counterNonEventBlocks = 0;
    public int numberOfItemsOnMap = 0;
    public int enemiesOnMap = 0;

    public int battleMusicToChoose = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        player = new PlayerData();
        rows[0] = new BlockData[9];
        rows[1] = new BlockData[9];
        rows[2] = new BlockData[9];
        rows[3] = new BlockData[9];
        rows[4] = new BlockData[9];
        rows[5] = new BlockData[9];
        rows[6] = new BlockData[9];
        rows[7] = new BlockData[9];
        rows[8] = new BlockData[9];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}

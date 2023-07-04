using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isVisible = false;
    public int imageNumber = 0;

    public bool canMoveUp = false;
    public bool canMoveDown = false;
    public bool canMoveRight = false;
    public bool canMoveLeft = false;
    public string type = "none";

    public int damageToPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlockData (BlockData data) {
        this.isVisible = data.isVisible;
        this.canMoveUp = data.canMoveUp;
        this.canMoveDown = data.canMoveDown;
        this.canMoveRight = data.canMoveRight;
        this.canMoveLeft = data.canMoveLeft;
        this.imageNumber = data.imageNumber;
        this.type = data.type;
        this.damageToPlayer = data.damageToPlayer;
    }
}

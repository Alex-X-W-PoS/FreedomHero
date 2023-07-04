[System.Serializable]
public class BlockData
{
    public bool isVisible = false;
    public int imageNumber = 0;

    public bool canMoveUp = false;
    public bool canMoveDown = false;
    public bool canMoveRight = false;
    public bool canMoveLeft = false;
    public string type = "none";

    public int damageToPlayer = 0;

    public BlockData (bool isVisible, int imageNumber, bool canMoveUp, bool canMoveDown, bool canMoveRight, bool canMoveLeft, string type, int damageToPlayer) {
        this.isVisible = isVisible;
        this.imageNumber = imageNumber;
        this.canMoveUp = canMoveUp;
        this.canMoveDown = canMoveDown;
        this.canMoveRight = canMoveRight;
        this.canMoveLeft = canMoveLeft;
        this.type = type;
        this.damageToPlayer = damageToPlayer;
    }
}

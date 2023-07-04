[System.Serializable]
public class PlayerItem
{
    public ItemData item;
    public int quantity;

    public PlayerItem (ItemData item, int quantity) {
        this.item = item;
        this.quantity = quantity;
    }
}

[System.Serializable]

public class ItemData
{
    public int id;
    public string name;

    public string description;
    public string effect; //heal, cure, increase
    public string affected_stat; // L, H, S, HP, Condition To Cure
    public int amount;

    public ItemData (int id, string name, string description, string effect, string affected_stat, int amount) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.effect = effect;
        this.affected_stat = affected_stat;
        this.amount = amount;
    }
}



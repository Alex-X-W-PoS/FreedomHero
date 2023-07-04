[System.Serializable]

public class EventData
{
    public string type; //trap, encounter, heal, item
    public string description_text;
    public string event_action; //substract or add
    public int hp_amount;
    public int itemCode;

    public EventData (string type, string description_text, string event_action, int hp_amount, int itemCode) {
        this.type = type;
        this.description_text = description_text;
        this.event_action = event_action;
        this.hp_amount = hp_amount;
        this.itemCode = itemCode;
    }
}

[System.Serializable]
public class PlayerData
{
    //Data related to the player
    public string Name;
    public string Class;

    public int image; //Is an integer because it is the position of the image array

    ///STATS///
    public int HP;

    public int Light_attack_stat;

    public int Heavy_attack_stat;

    public int Special_attack_stat;

    //////////////

    public int current_HP;

    public bool canEvade; //TRUE = Evasion, FALSE = Defends.
    public int AP = 3;

    public string status = "normal";

    public int currentShield = 0;

    public int status_condition_duration = 0;

    public Skill[] skills;

    public PlayerItem[] itemBag;
}

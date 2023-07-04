[System.Serializable]
public class Skill
{
    public string name;
    public string description; 
    public string type;
    public int ap_cost;
    public int minimum_required;
    public string target;
    public bool has_effect;
    public SkillEffect effect;
}

[System.Serializable]
public class BattleEnemy
{
    public string Name;
    public int image;
    public int Max_HP;
    public int Current_HP;
    public int Light_attack_stat;
    public int Heavy_attack_stat;
    public int Special_attack_stat;
    public string heavy_numbers;
    public string special_numbers;
    public Skill[] skills;
    public string status;

    public int status_condition_duration;

    public BattleEnemy (Enemy enemy) {
        this.Name = enemy.Name;
        this.image = enemy.image;
        this.Max_HP = enemy.HP;
        this.Current_HP = enemy.HP;
        this.Light_attack_stat = enemy.Light_attack_stat;
        this.Heavy_attack_stat = enemy.Heavy_attack_stat;
        this.Special_attack_stat = enemy.Special_attack_stat;
        this.heavy_numbers = enemy.heavy_numbers;
        this.special_numbers = enemy.special_numbers;
        this.skills = enemy.skills;
        this.status = "normal";
        this.status_condition_duration = 0;
    }

}

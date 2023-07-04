[System.Serializable]
public class SkillEffect
{
    public bool has_damage_modification;
    public EffectDamageModification damage_modfication; 
    public bool has_status_condition;
    public EffectStatusCondition status_condition;
    public bool has_cure;
    public EffectCure cure;
    public bool has_damage_mitigation;
    public EffectDamageMitigation damage_mitigation;
}
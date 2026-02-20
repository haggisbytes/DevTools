using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Monster/Create New Monster", order = 1)]
public class MonsterData : ScriptableObject
{
    [Header("Basic Info")]
    public string Name;
    public float HP;
    public float Attack;
    public float Speed;
    public bool IsBoss;

    [Header("Special Settings")]
    public bool CanFly;
    [ShowIf("CanFly", true)]
    public float FlightSpeed;

    public bool HasMagic;
    [ShowIf("HasMagic", true)]
    public int SpellPower;

    public bool IsUndead;
    [ShowIf("IsUndead", true)]
    public float DecayRate;


    [Header("Non Boss Settings")]
    [HideIf("IsBoss", true)]
    public float NonBossDamageMultiplier;
    [HideIf("IsBoss", true)]
    public int XPReward;

    [Header("Boss Settings")]
    [ShowIf("IsBoss", true)]
    public string BossAbility;
    [ShowIf("IsBoss", true)]
    public float BossDamageMultiplier;


}

using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/CharacterData")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Life")]
    public int maxLife;

    [Header("Movement")]
    public int speed;

    [Header("Attack")]
    public int range;
    public int meleeDamage;
    public int distanceDamage;

    [Header("Healing")]
    public int healingPoints;
    public bool canHealAllies;
    public int healingRange;
}
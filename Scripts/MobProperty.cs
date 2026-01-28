using UnityEngine;

// Defines the categories of Mobs
public enum MobType { Worker, Soldier, Major }

[CreateAssetMenu(fileName = "MobProperty", menuName = "Mob/Mob Property")]
public class MobProperty : ScriptableObject
{
    public string mobName;
    public int mobId;
    public MobType type;

    [TextArea(3, 10)]
    public string description;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float healthRecoveryPerDay = 5f;
    public int maxAge = 0;

    [Header("Settings")]
    public float moveSpeed = 1f;
    public float viewRadius = 3f;
    
    public float damage = 10f;
    public float attackCooldown = 1.5f; 

    public float foodConsumptionPerDay = 10f;
    public float nutrientWorth = 20f; 
}
using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Stats/EntityData")]
public class EntityStatsData : ScriptableObject
{
    [Header("Base Stats")]
    [SerializeField] private string entityName;
    [SerializeField] private float maxHp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float damage;
    
    public string Entity => entityName;
    public float MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float Damage => damage;
}

using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "DestroyableEntity", order = 1)]
public class DestroyableEntity : ScriptableObject {
    public string name_;

    public int health_;
    public float dodge_;
    public float attackRating_;
    public float criticalChance_;
    public int damage_;
    public int armor_;
    public int armorPiercing_;
    public float speed_ = 1f;


}

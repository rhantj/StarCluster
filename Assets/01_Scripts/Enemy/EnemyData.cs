using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName ="Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int hp;
    public float moveSpeed;
}

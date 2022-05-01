using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCreator : MonoBehaviour
{
 
    public class EnemyProperties
    {
        public int ID;
        public int Lives;

    }

    static public List<EnemyProperties> Enemies = new List<EnemyProperties>()
    {
        new EnemyProperties(){ ID = 0, Lives = 1},
        new EnemyProperties(){ ID = 1, Lives = 3},
        new EnemyProperties(){ ID = 2, Lives = 5},
        new EnemyProperties(){ ID = 3, Lives = 6},
        new EnemyProperties(){ ID = 4, Lives = 7},
        new EnemyProperties(){ ID = 5, Lives = 9},
    };

    static public EnemyProperties CloneEnemy(EnemyProperties _enemy)
    {
        EnemyProperties NewEnemy = new EnemyProperties()
        {
            ID = _enemy.ID,
            Lives = _enemy.Lives,
        };

        return NewEnemy;
    }

    static public EnemyProperties GetEnemyByID(int _id)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].ID == _id)
            {
                return CloneEnemy(Enemies[i]);
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletType { PLAYER, ENEMY }

public class BulletControl : MonoBehaviour
{
    private BulletType Type;
    private float Speed;
    private float Direction;
    private GameManager Manager;


    public void InitBullet(BulletType _type, float _speed, GameManager _manager)
    {
        Type = _type;
        Speed = _speed;
        Manager = _manager;

        if (_type == BulletType.PLAYER)
        {
            Direction = 1;
        }
        else
        {
            Direction = -1;
        }
    }
    
    void Update()
    {
        transform.Translate(Vector2.up * Direction * Speed * Time.deltaTime);
    }

    // Bullet Collision
    private void OnTriggerEnter2D(Collider2D other)                                 // nach other ändern, da andere Funktionen auch eine "collision" haben und dies zu Fehlern führt
    {
        // Erkennen und unterscheiden, ob Bullet für Player || Enemy bestimmt ist   // im GameManager eine Klasse (EnemmyProperties) instanzieren
        switch(Type)
        {
            case BulletType.ENEMY:
                if(other.tag == "Player")
                {
                    Manager.GetDamagePlayer();
                    Destroy(gameObject);
                }
                if (other.tag == "Bullet")
                {
                    if (other.tag != "Player")
                    {
                        Destroy(gameObject);
                    }
                }

                break;
            case BulletType.PLAYER:
                if (other.tag == "Enemy")
                {
                    Manager.GetDamageEnemy(other.gameObject);
                    Destroy(gameObject);
                }
                if (other.tag == "Bullet")
                {
                    if (other.tag != "Player")
                    {
                        Destroy(gameObject);
                    }
                }
                break;
        }
    }
}

using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider2D collider2d;
    public int attackValue;
    void Start()
    {
        collider2d = GetComponent<Collider2D>();
        //collider2d.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //var Melee = collision.gameObject.GetComponent<MeleeController>();
        var enemycontroller_Melee = collision.gameObject.GetComponent<MeleeController>();
        var enemycontroller_archer = collision.gameObject.GetComponent<ArcherController>();
        if (enemycontroller_Melee != null)
        {
            enemycontroller_Melee.Hurt(attackValue);
        }

        if (enemycontroller_archer != null)
        {
            enemycontroller_archer.Hurt(attackValue);
        }
    }
}

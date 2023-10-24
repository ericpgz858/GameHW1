using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private Collider2D collider2d;
    public Transform Source;
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
        var playercontroller = collision.gameObject.GetComponent<PlayerController>();
        if (playercontroller != null)
        {
            playercontroller.Hurt(Source.position.x, 10);
        }
    }
}

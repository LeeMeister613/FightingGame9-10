using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBallScript : BallScript
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeIceDamage(damage, owner);
        }
        Destroy(gameObject);
    }
}

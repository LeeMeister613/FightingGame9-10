using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private AudioSource audio;
    public AudioClip[] clips;
    public float damage;
    public float lifetime;
    public float speed;
    public PlayerController owner;
    public Rigidbody2D rig;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        rig = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeDamage(damage, owner);
        }
        Destroy(gameObject);
    }

    public void spawnStdBall(float damage, float speed, PlayerController owner, float direction)
    {
        setDamage(damage);
        setOwner(owner);
        setSpeed(speed);
        rig.velocity = new Vector2(direction * speed, 0);
    }
    public void setOwner(PlayerController owner)
    {
        this.owner = owner;
    }
    public void setDamage (float damage)
    {
        this.damage = damage;
    }
    public void setDamage(int damage)
    {
        this.damage = damage;
    }
    public void setSpeed (float speed)
    {
        this.speed = speed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Max Values")]
    public int maxHealth;
    public float maxSpeed;
    public int maxJumps;
    public float hitDuration;
    public float slowTime;
    public float maxCharge;

    [Header("Current Values")]
    public int curHealth;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public float lastHit;
    public float lastIceHit;
    public bool isSlowed;
    public float curCharge;
    public bool isCharging;
    public float chargeTime;

    [Header("Attack Variables")]
    public PlayerController curAttacker;
    public float attackDmg;
    public float attackSpeed;
    public float iceAttackSpeed;
    public float AttackRate;
    public float lastAttackTime;
    public GameObject[] attackPrefabs;

    [Header("Modifiers")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Audio")]
    public AudioClip[] playerfx;
    //jump 0, land 1, shoot 2, die 3, point 4, tauntuno 5

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AudioSource audio;
    private Transform muzzle;
    private GameManager gameManager;
    private PlayerContainerUI playerUI;
    public GameObject deathEffect;
    public GameObject stepEffect;
    public GameObject jumpEffect;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        muzzle = FindObjectOfType<MuzzleScript>().transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        curJumps = maxJumps;
        isCharging = false;
        score = 0;
        moveSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (curHealth <= 0)
        {
            die();
        }
        if(transform.position.y < -15)
        {
            die();
        }
        if (curAttacker)
        {
            if(Time.time - lastHit > hitDuration)
            {
                curAttacker = null;
            }
        }
        if (isSlowed)
        {
            if (Time.time - lastIceHit > slowTime)
            {
                isSlowed = false;
                moveSpeed = maxSpeed;
            }
        }
        if (isCharging)
        {
            curCharge += chargeTime;
            if(curCharge > maxCharge)
            {
                curCharge = maxCharge;
            }
            playerUI.updateChargeFill(curCharge, maxCharge);
        }
    }

    private void FixedUpdate()
    {
        move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D hit in collision.contacts)
        {
            if (hit.collider.CompareTag("ground")|| hit.collider.CompareTag("Player"))
            {
                if(hit.point.y < transform.position.y)
                {
                    Destroy(Instantiate(stepEffect, transform.position, Quaternion.identity), 1f);
                    audio.PlayOneShot(playerfx[1]);
                    curJumps = maxJumps;
                }
            }
            if ((hit.point.x > transform.position.x || hit.point.x < transform.position.x)&&curJumps==0&& hit.point.y < transform.position.y)
            {
                Destroy(Instantiate(stepEffect, transform.position, Quaternion.identity), 1f);
                curJumps++;
            }
        } 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void move()
    {
        rig.velocity = new Vector2(curMoveInput * moveSpeed, rig.velocity.y);

        if(curMoveInput != 0)
        {
            transform.localScale = new Vector3(curMoveInput>0?1:-1, 1, 1);
        }
    }

    private void jump()
    {
        Destroy(Instantiate(jumpEffect, transform.position, Quaternion.identity), 1f);
        audio.PlayOneShot(playerfx[0]);
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void die()
    {
        Destroy(Instantiate(deathEffect, transform.position, Quaternion.identity), 1f);
        print("ha lol, you died");
        if(curAttacker != null)
        {
            curAttacker.addScore();
        }
        else
        {
            score--;
            if (score > 0)
            {
                score = 0;
            }
        }
        respawn();
    }

    public void addScore()
    {
        score++;
        playerUI.updateScoreText(score);
    }
    public void dropOut()
    {
        Destroy(playerUI.gameObject);
        Destroy(gameObject);
    }
    public void takeDamage(int amount, PlayerController attacker)
    {
        curHealth -= amount;
        curCharge /= 4;
        curAttacker = attacker;
        lastHit = Time.time;
        playerUI.updateHealthbarFill(curHealth, maxHealth);
    }
    public void takeDamage(float amount, PlayerController attacker)
    {
        curHealth -= (int)amount;
        curCharge /= 4;
        curAttacker = attacker;
        lastHit = Time.time;
        playerUI.updateHealthbarFill(curHealth, maxHealth);
    }
    public void takeIceDamage(float amount, PlayerController attacker)
    {
        curHealth -= (int)amount;
        curAttacker = attacker;
        curCharge /= 4;
        lastHit = Time.time;
        lastIceHit = Time.time;
        isSlowed = true;
        moveSpeed /= 2;
        playerUI.updateHealthbarFill(curHealth, maxHealth);
    }
    public void takeChargeDamage(float amount, PlayerController attacker)
    {
        curHealth -= (int)amount;
        curCharge /= 4;
        curAttacker = attacker;
        lastHit = Time.time;
        playerUI.updateHealthbarFill(curHealth, maxHealth);
        playerUI.updateChargeFill(curCharge, maxCharge);
    }
    private void respawn()
    {
        curHealth = maxHealth;
        curJumps = maxJumps;
        moveSpeed = maxSpeed;
        curCharge = 0;
        isCharging = false;
        isSlowed = false;
        curAttacker = null;
        rig.velocity = Vector2.zero;
        transform.position = gameManager.spawnpoints[Random.Range(0, gameManager.spawnpoints.Length)].position;
        playerUI.updateScoreText(score);
        playerUI.updateHealthbarFill(curHealth, maxHealth);
        playerUI.updateChargeFill(curCharge, maxCharge);
    }

    // input action map methods
    //move input
    public void onMoveInput(InputAction.CallbackContext context)
    {
        curMoveInput = context.ReadValue<float>();
    }
    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (curJumps > 0)
            {
                curJumps--;
                jump();
            }
        }
    }
    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }
    }
    public void onAttackStandardInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed&&Time.time - lastAttackTime > AttackRate)
        {
            lastAttackTime = Time.time;
            spawnStdAttack();
        }
    }
    public void spawnStdAttack()
    {
        if (isCharging==false)
        {
            GameObject stdBall = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
            stdBall.GetComponent<BallScript>().spawnStdBall(attackDmg, attackSpeed, this, transform.localScale.x);
        }
        else
        {
            isCharging = false;
        }
    }

    public void spawnIceAttack()
    {
        if (isCharging == false)
        {
            GameObject IceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
            IceBall.GetComponent<BallScript>().spawnStdBall(attackDmg, iceAttackSpeed, this, transform.localScale.x);
        }
        else
        {
            isCharging = false;
        }
    }
    public void spawnChargeAttack()
    {
        GameObject deathBall = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        deathBall.GetComponent<BallScript>().spawnStdBall(curCharge, attackSpeed, this, transform.localScale.x);
    }

    public void setUI(PlayerContainerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void onAttackChargeInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
                isCharging = true;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
                isCharging = false;
                spawnChargeAttack();
                curCharge = 0;
                playerUI.updateChargeFill(curCharge, maxCharge);
        }
        
    }
    public void onAttackIceInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > AttackRate)
            {
                lastAttackTime = Time.time;
                spawnIceAttack();
            }
        }
    }
    public void onPauseInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
        }
    }
    public void onTauntUnoInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            audio.PlayOneShot(playerfx[5]);
        }
    }
    public void onTauntDosInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("Taunt Nombre Dos button");
        }
    }
    public void onTauntTresInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt Nombre Tres button");
        }
    }
    public void onTauntQuatroInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt Nombre Quatro button");
        }
    }



}

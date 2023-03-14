using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("lists")]
    public Color[] playercolors;
    public List<PlayerController> playerslist = new List<PlayerController>();
    public Transform[] spawnpoints;

    [Header("prefab references")]
    public GameObject deatheffect;
    public GameObject playerContainerPrefab;

    [Header("components")]
    private AudioSource audio;
    public AudioClip[] gamefx;
    public Transform containerGroup;
    public TextMeshProUGUI timeText;

    [Header("level vars")]
    public float startTime;
    public float curTime;
    List<PlayerController> winningPlayers;
    public bool canJoin;

    public static GameManager instance;


    private void Awake()
    {
        canJoin = true;
        instance = this;
        audio = GetComponent<AudioSource>();
        containerGroup = GameObject.FindGameObjectWithTag("UIContainer").GetComponent<Transform>();
        startTime = PlayerPrefs.GetFloat("roundTime",  100);
        winningPlayers = new List<PlayerController>();

    }
    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        timeText.text = ((int)curTime).ToString();
    }

    public void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        timeText.text = ((int)curTime).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0)
        {
            int highScore = 0;
            int index = 0;
            foreach(PlayerController player in playerslist)
            {
                if (player.score > highScore)
                {
                    winningPlayers.Clear();
                    highScore = player.score;
                    index = playerslist.IndexOf(player);
                    winningPlayers.Add(player);
                }
                else if (player.score == highScore)
                {
                    winningPlayers.Add(player);
                }
            }
            if (winningPlayers.Count > 1)
            {
                canJoin = false;
                foreach(PlayerController player in playerslist)
                {
                    if (!winningPlayers.Contains(player))
                    {
                        player.dropOut();
                    }
                }
                curTime = 60;
            }
            else
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("Results");
            }
        }
    }

    public void onPlayerJoined(PlayerInput player)
    {
        if (canJoin)
        {
            audio.PlayOneShot(gamefx[0]);
            player.GetComponentInChildren<SpriteRenderer>().color = playercolors[playerslist.Count];

            PlayerContainerUI cont = Instantiate(playerContainerPrefab, containerGroup).GetComponent<PlayerContainerUI>();
            player.GetComponent<PlayerController>().setUI(cont);
            cont.initialize(playercolors[playerslist.Count]);

            playerslist.Add(player.GetComponent<PlayerController>());
            player.transform.position = spawnpoints[Random.Range(0, spawnpoints.Length)].position;
        }
    }
}

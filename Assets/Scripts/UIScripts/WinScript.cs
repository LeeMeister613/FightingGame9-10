using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScript : MonoBehaviour
{
    public TextMeshProUGUI winMessage;
    public Color[] playerColors;

    // Start is called before the first frame update
    void Start()
    {
      
        winMessage.color = playerColors[PlayerPrefs.GetInt("colorIndex", 0)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

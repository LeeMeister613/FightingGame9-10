using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContainerUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Image healthbarFill;
    public Image chargeFill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
    public void updateHealthbarFill(int curHP, int maxHP)
    {
        healthbarFill.fillAmount = (float)curHP / (float)maxHP;
    }
    public void updateChargeFill(float curCharge, float maxCharge)
    {
        chargeFill.fillAmount = curCharge / maxCharge;
    }
    public void initialize(Color color)
    {
        scoreText.color = color;
        healthbarFill.color = color;
        scoreText.text = "0";
        healthbarFill.fillAmount = 1;
        chargeFill.fillAmount = 0;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeHolder : MonoBehaviour
{
    public TextMeshProUGUI player1Name, player1Amount;
    public Image player1Panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int internalID, string name, int currentLife, float maxLife) 
    {
        player1Name.text = name;
        player1Amount.text = currentLife.ToString();
        
    }

    public void UpdatePlayerUI(int internalID,int currentLife)
    {
        player1Amount.text = currentLife.ToString();
        player1Panel.fillAmount = (float)(currentLife / 100f);
        Debug.Log("My life is crazy: " + (float)currentLife / 100f);

    }
}

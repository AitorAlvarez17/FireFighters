using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeHolder : MonoBehaviour
{
    public TextMeshProUGUI nameText, lifeText;
    public Image uiBar;

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
        nameText.text = name;
        lifeText.text = currentLife.ToString();
        uiBar.fillAmount = 1f;
    }

    public void UpdatePlayerUI(int internalID,int currentLife)
    {
        lifeText.text = currentLife.ToString();
        this.uiBar.fillAmount = (float)(currentLife / 100f);
        Debug.Log("My life is crazy: " + (float)currentLife / 100f);

    }
}

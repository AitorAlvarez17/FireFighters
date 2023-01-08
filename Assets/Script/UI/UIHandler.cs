using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net;

public class UIHandler : MonoBehaviour
{

    //TODO: Generate bars per player.
    // Replace Names
    // Replace Quantity
    // Make Bar move.
    public List<Color> colorList;

    public Transform lifeParent;

    private static Dictionary<int, LifeHolder> lifeHolders = new Dictionary<int, LifeHolder>();
    public GameObject LifePrefab;
    private TextMeshProUGUI numberOfPlayers;

    // Start is called before the first frame update
    void Start()
    {
        numberOfPlayers = ServerController.MyServerInstance.numberOfPlayers;

    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePlayersLife();
    }

    void UpdatePlayersLife()
    {

    }

    public void createPlayerUI(int internalID, string name, int currentLife, float maxLife)
    {
        if(lifeHolders.ContainsKey(internalID)) return;
        
        GameObject newLifeBar = Instantiate(LifePrefab, lifeParent);

        RectTransform rect = (RectTransform)newLifeBar.transform;
        newLifeBar.GetComponent<LifeHolder>().Init(internalID, name, currentLife, maxLife);
        lifeHolders.Add(internalID, newLifeBar.GetComponent<LifeHolder>());
        ManagePivots(rect, lifeHolders.Count);

    }
    public void UpdatePlayerUI(int internalID, int currentLife)
    {
        if (lifeHolders.ContainsKey(internalID) == true)
        {
            lifeHolders[internalID].transform.GetComponent<LifeHolder>().UpdatePlayerUI(internalID, currentLife);
        }
        else
        {
            Debug.Log("Key doesnt exists!!");
        }
    }

    private void ManagePivots(RectTransform rect, int id)
    {

        switch (id)
        {
            case 1:
                rect.anchorMin = new Vector2(0f, 1.0f);
                rect.anchorMax = new Vector2(0f, 1.0f);
                rect.pivot = new Vector2(0f, 1.0f);
                lifeHolders[id].uiBar.color = colorList[0];
                
                //UP left
                break;
            case 2:
                rect.anchorMin = new Vector2(1.0f, 1.0f);
                rect.anchorMax = new Vector2(1.0f, 1.0f);
                rect.pivot = new Vector2(1.0f, 1.0f);
                lifeHolders[id].uiBar.color = colorList[1];
                //UP right
                break;
            case 3:
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0f, 0f);
                lifeHolders[id].uiBar.color = colorList[2];
                //DOWN left
                break;
            case 4:
                rect.anchorMin = new Vector2(1.0f, 0f);
                rect.anchorMax = new Vector2(1.0f, 0f);
                rect.pivot = new Vector2(1.0f, 0f);
                lifeHolders[id].uiBar.color = colorList[3];
                //DOWN right
                break;
            default:
                break;
        }
        rect.anchoredPosition = Vector3.zero;
        Debug.Log(rect);
    }
}

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
        GameObject newLifeBar = Instantiate(LifePrefab, lifeParent);

        RectTransform rect = (RectTransform)newLifeBar.transform;
        newLifeBar.GetComponent<LifeHolder>().Init(internalID,name,currentLife,maxLife);
        lifeHolders.Add(internalID,  newLifeBar.GetComponent<LifeHolder>());
        ManagePivots(rect, lifeHolders.Count);

    }
    public void UpdatePlayerUI(int internalID, string name, int currentLife, float maxLife)
    {
        
        LifePrefab.transform.GetComponent<LifeHolder>().UpdatePlayerUI(internalID,name,currentLife,maxLife);
    }

    private void ManagePivots(RectTransform rect, int id)
    {
        switch (id)
        {
            case 1:
                rect.pivot = new Vector2(0f, 1.0f);
                rect.position = Vector3.zero;
                //UP left
                break;
            case 2:
                rect.pivot = new Vector2(1.0f, 1.0f);
                rect.position = Vector3.zero;
                //UP right
                break;
            case 3:
                rect.pivot = new Vector2(0f, 0f);  
                rect.position = Vector3.zero; 
                //DOWN left
                break;
            case 4:
                rect.pivot = new Vector2(1.0f, 0f);
                rect.position = Vector3.zero;
                //DOWN right
                break;
            default:
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GeneralDebug : MonoBehaviour
{
    public WorldController controller;
    public TextMeshProUGUI display;
    int nPlayers;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    void DebugOn()
    {
    }

    void DebugOff()
    {

    }
    void Display()
    {
        nPlayers = controller.worldDolls.Count;
        display.text = "";
        for(int i = 1; i<nPlayers+1; i++)
        {
            display.text += "Name: " + controller.worldDolls[i].name + " ";
            display.text += "Key ID:" + controller.worldDolls[i].internalId + " ";
            display.text += "Posx:" + controller.worldDolls[i].gameObject.transform.localPosition.x + " ";
            display.text += "Posy:" + controller.worldDolls[i].gameObject.transform.localPosition.y + " ";
            display.text += "Posz:" + controller.worldDolls[i].gameObject.transform.localPosition.z + "/n";
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (active) Display();
    }
}

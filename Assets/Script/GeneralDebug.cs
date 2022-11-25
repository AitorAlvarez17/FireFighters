using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GeneralDebug : MonoBehaviour
{
    public WorldController controller;
    public TextMeshProUGUI display;
    int nPlayers;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        nPlayers = controller.worldDolls.Count;
        if(nPlayers == 1)
        {
            display.text = "";
            display.text += "Name: " + controller.worldDolls[1].lumberjack.name + "\n";
            display.text += "Key ID:" + controller.worldDolls[1].lumberjack.internalId + "\n";
            display.text += "Posx:" + controller.worldDolls[1].lumberjack.gameObject.transform.localPosition.x +"\n";
            display.text += "Posy:" + controller.worldDolls[1].lumberjack.gameObject.transform.localPosition.y + "\n";
            display.text += "Posz:" + controller.worldDolls[1].lumberjack.gameObject.transform.localPosition.z + "\n";

        }
    }
}

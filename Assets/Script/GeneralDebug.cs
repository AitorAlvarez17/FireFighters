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
        display.text = "";

        if (nPlayers == 1)
        {
            display.text += "Name: " + controller.worldDolls[1].name + "\n";
            display.text += "Key ID:" + controller.worldDolls[1].internalId + "\n";
            display.text += "Posx:" + controller.worldDolls[1].gameObject.transform.localPosition.x +"\n";
            display.text += "Posy:" + controller.worldDolls[1].gameObject.transform.localPosition.y + "\n";
            display.text += "Posz:" + controller.worldDolls[1].gameObject.transform.localPosition.z + "\n";

        }
    }
}

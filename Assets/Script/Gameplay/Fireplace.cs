using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fireplace : MonoBehaviour
{
    public GameObject GC;
    public int internalID;
    public string fireName;

    public TextMeshPro lifeText;

    public float life;
    public float maxLife = 100;

    public Fireplace()
    {

    }

    public void Init(int _key, string _name)
    {
        GC = GameObject.FindGameObjectWithTag("GameController");
        internalID = _key;
        fireName = _name;
        life = 100f;
        lifeText.text = "Life: " + life;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealBar(int _type, int _amount)
    {
        switch (_type)
        {
            case 1:
                life += _amount;
                lifeText.text = "Life: " + _amount;
                FirePlaceActions(life / maxLife);
                break;
            case 2:
                life -= _amount;
                lifeText.text = "Life: " + _amount;
                FirePlaceActions(life / maxLife);
                break;
            default:
                break;
        }
        
        //Ping Life();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<Lumberjack>().interacter != true)
            return;

        //get the action from the lumberjack and put it into it
        if (GC.GetComponent<UDPClient>() != null)
        {
            GC.GetComponent<UDPClient>().PingFireAction(internalID, other.transform.GetComponent<Lumberjack>().charge.Type, other.transform.GetComponent<Lumberjack>().charge.Amount);
            other.transform.GetComponent<Lumberjack>().charge.ClearCharge();
            other.transform.GetComponent<Lumberjack>().PrintDebug();

        }
        else if (GC.GetComponent<UDPServer>() != null)
        {
            GC.GetComponent<UDPServer>().PingFireAction(internalID, other.transform.GetComponent<Lumberjack>().charge.Type, other.transform.GetComponent<Lumberjack>().charge.Amount);
            other.transform.GetComponent<Lumberjack>().charge.ClearCharge();
            other.transform.GetComponent<Lumberjack>().PrintDebug();
        }
        //PingFireAction(int action, int amount);

    }

    public void FirePlaceActions(float lifeFraction)
    {

    }
}

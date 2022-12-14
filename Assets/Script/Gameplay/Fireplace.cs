using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Fireplace : MonoBehaviour
{
    public GameObject GC;
    public int internalID;
    public string fireName;
    public AudioClip FX;
    AudioSource audioSource;

    public TextMeshPro lifeText;

    public SpriteRenderer circularSpriteImage;

    private int life = 300;

    public int GetLife()
    {
        return life;
    }
    public float maxLife = 100;
    bool soundMade = false;

    public Fireplace()
    {

    }

    public void Init(int _key, string _name)
    {
        GC = GameObject.FindGameObjectWithTag("GameController");
        internalID = _key;
        fireName = _name;
        life = 100;
        lifeText.text = "LIFE: " + life;
        GC.GetComponent<UIHandler>().createPlayerUI(_key, _name, life, maxLife);

        circularSpriteImage.color = GC.GetComponent<UIHandler>().colorList[--_key];
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HealBar(int _type, int _amount)
    {
        if (_amount > 100)
        {
            life = 100;
            return;
        }
        switch (_type)
        {
            case 0:
                Debug.Log("No charge");
                break;
            case 1:
                life += _amount;
                lifeText.text = "LIFE: " + life;
                FirePlaceActions(life / maxLife);
                break;
            case 2:
                life -= _amount;
                lifeText.text = "LIFE: " + life;
                FirePlaceActions(life / maxLife);
                break;
            default:
                break;
        }

        //Ping Life();
    }

    public void SetLife(int _life)
    {
        if (_life > 100)
        {
            life = 100;
            return;
        }
        Debug.Log("Life set in Fireplace [ID: " + internalID + "] with" + "[LIFE: ]" + _life);
        life = _life;
        lifeText.text = "LIFE: " + life;
        GC.GetComponent<UIHandler>().UpdatePlayerUI(internalID, _life);
    }

    public void OnTriggerEnter(Collider other)
    {

        Debug.Log("Colliding with" + other.transform.tag);
        if (other.transform.tag == "Lumber")
        {
            if (other.transform.GetComponent<Lumberjack>().interacter == false)
            {
                other.transform.GetComponent<Lumberjack>().charge.ClearCharge();
                other.transform.GetComponent<Lumberjack>().PrintDebug();

                audioSource.PlayOneShot(FX);
                return;
            }

            if (GC == null)
            {
                GC = GameObject.FindGameObjectWithTag("GameController");
            }

            //get the action from the lumberjack and put it into it
            if (GC.transform.GetComponent<UDPClient>() != null)
            {
                if (other.transform.GetComponent<Lumberjack>().charge.Amount == 0)
                    return;

                //IMPORTANT! - this is prediction
                HealBar(other.transform.GetComponent<Lumberjack>().charge.Type, other.transform.GetComponent<Lumberjack>().charge.Amount);

                GC.GetComponent<UDPClient>().PingFireAction(internalID, other.transform.GetComponent<Lumberjack>().charge.Type, other.transform.GetComponent<Lumberjack>().charge.Amount, life);
                other.transform.GetComponent<Lumberjack>().charge.ClearCharge();
                other.transform.GetComponent<Lumberjack>().PrintDebug();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {

    }

    public void FirePlaceActions(float lifeFraction)
    {

    }
}

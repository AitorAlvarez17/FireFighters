using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Charge
{
    private int amount;

    //0 for nothing, 1 for wood, 2 for water
    private int type;


    public Charge(int _amount, int _type)
    {
        amount = _amount;
        type = _type;
    }

    public int Amount   
    {
        get { return amount; }   
        set { amount = value; }  
    }

    public int Type
    {
        get { return type; }
        set { type = value; }
    }

    public void SumWood(int _amount)
    {
        if (type != 1)
            amount = 0;

        type = 1;
        amount += _amount;
    }

    public void SumWater(int _amount)
    {
        if (type != 2)
            amount = 0;

        type = 2;
        amount += _amount;
    }

    public void ClearCharge()
    {
        type = 0;
        amount = 0;
    }
}
public class Lumberjack : MonoBehaviour
{
    public bool deadRackoning = false;

    // Start is called before the first frame update
    public int internalId;
    public Transform trans;
    public TextMeshPro textInfo;
    public TextMeshPro resDebug;
    public string Username;
    public Material[] hats;
    public Material[] shirts;
    public Material[] hair;

    //duration of the interpolation
    float IP = 0f;
    public bool isLerping = false;
    public float rtt = 0f;

    public float time = 0f;
    public float lastTime = 0f;

    public Vector3 aimPosition;
    public Vector3 transBuffer;


    public bool isMoving = false; 
    public bool isPredictingMovement = false;
    public bool isCorrectingMov = false;

    public bool interacter = false;
    public float deltaTime = 0f;
    
    public Transform cameraTrans;

    //0 for nothing, 1 for wood, 2 for water
    public Charge charge = null;
    //visuals
    public GameObject avatar;

    //animation
    Animator animator;
    bool isWalking;
    
    public Lumberjack()
    {

    }

    public void Init(int _id, bool reckoning , bool interacter = false)
    {
        SetUsername("Player" + _id);
        SetId(_id);
        SetOutfit(_id);
        SetInteracter(interacter);
        SetRackoning(reckoning);
        charge = new Charge(0, 0);
        PrintDebug();
    }

    public void SetId(int _id)
    {
        internalId = _id;
        //textInfo.text += "Key ID:" + _id + "\n";
    }
    public void SetUsername(string username)
    {
        Username = username;
        textInfo.text += "Name: " + username + "\n"; 
    }

    public void SetInteracter(bool value)
    {
        Debug.Log("Interacter set to: " + value);
        interacter = value;
    }

    public void SetRackoning(bool rack)
    {
        deadRackoning = rack;
    }

    public void PrintDebug()
    {
        switch (charge.Type)
        {
            case 0:
                resDebug.text = "Empty!";
                resDebug.color = Color.grey;
                break;
            case 1:
                Debug.Log("Printing WOOD");
                resDebug.text = "Wood: " + charge.Amount;
                resDebug.color = Color.red;
                break;
            case 2:
                resDebug.text = "Water: " + charge.Amount;
                resDebug.color = Color.cyan;
                break;
            default:
                break;
        }
    }
    public void SetOutfit(int id)
    {
        Material[] mats = avatar.GetComponent<Renderer>().materials;
        switch (id)
        {
            case 2:
                mats[0] = hats[0];
                mats[2] = hair[0];
                mats[3] = shirts[0];
                break;
            case 3:
                mats[0] = hats[1];
                mats[2] = hair[1];
                mats[3] = shirts[1];
                break;
            case 4:
                mats[0] = hats[2];
                mats[2] = hair[2];
                mats[3] = shirts[2];
                break;
            default:
                Debug.Log("No id supported");
                break;
        }
        avatar.GetComponent<Renderer>().materials = mats;
    }
    void Start()
    {
        trans = this.gameObject.GetComponent<Transform>();

        animator = GetComponent<Animator>();
        isWalking = false;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        time += Time.deltaTime;


        if (deadRackoning == true)
        {
            //Debug.Log("Is reckoning");
        }

        if(isWalking && IsGettingInputFromPlayer()==false)
            animator.SetBool("isWalking", false);
        
    }

    public void Move(float[] _positions, Vector3 _directions, float _IP)
    {
        animator.SetBool("isWalking", true);
        isWalking=true;

        //Debug.Log("Time difference of:" + (time - lastTime) + " s");
        //Debug.Log("The RTT added to IP is: " + ((time - lastTime) - (_IP / 1000)));
        //possible upgrade: use RTT in order to lerp having in accountance lag.
        Vector3 newPositions = new Vector3(_positions[0], _positions[1], _positions[2]);

        lastTime = time;

        SmoothRotation(_directions, IP);

        if (isLerping == true && IP > _IP)
        {
            //here IP > PP
            //+1000 is simply a matter of smoothness, as i lerp with delta time it's interesting to scale the ms to s
            IP += _IP + 1000f;
            aimPosition = newPositions;
            transBuffer = trans.transform.position;
            //Debug.Log("Is lerping with a accumulated IP of" + IP);
            //i will try to stay here but with the IP closely to PP
        }
        else
        {
            //here IP < PP
            IP = _IP + 1000f;
            aimPosition = newPositions;
            transBuffer = trans.transform.position;
            //Debug.Log("[NEW] lerping with a accumulated IP of" + IP);

            StartCoroutine(Lerp());
        }

    }

    IEnumerator Lerp()
    {
        isLerping = true;
        Debug.Log("LERPING!");

        float timeElapsed = 0f;
        while (timeElapsed < (IP / 1000))
        {
            trans.position = Vector3.Lerp(trans.position, aimPosition, timeElapsed / (IP / 1000));
            //Debug.Log("Position" + trans.position);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("Time elapsed: " + timeElapsed);
        //Debug.Log("Time elapsed:" + timeElapsed + " s");

        trans.position = aimPosition;

        isLerping = false;

        if (rtt > 0)
        {
            Debug.Log("It should dead reckon a little " + rtt);
        }
        yield break;
    }

    public void MovementPrediction()
    {
        //take quaternion direction and go through it
    }

    public void MovementCorrection()
    {
        //correct the movement 
    }

    public void SmoothRotation(Vector3 directions, float IP)
    {
        //Debug.Log("Rotating" + directions);
        Quaternion rotation = Quaternion.LookRotation(directions, Vector3.up);
        trans.rotation = Quaternion.RotateTowards(trans.rotation, rotation, 360f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (interacter == false)
            return;

        Debug.Log("Lumberjack Entered the Trigger");
        if (other.tag == "Water")
        {
            charge.SumWater(5);
            
        }
        else if(other.tag == "Three")
        {
            charge.SumWood(5);
        }

        PrintDebug();
    }

    public bool IsGettingInputFromPlayer()
    {
        bool ret = false;
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
            ret = true;
        return ret;
    }
}

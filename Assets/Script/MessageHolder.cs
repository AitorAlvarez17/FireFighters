using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageHolder : MonoBehaviour
{
    public TextMeshProUGUI message;
    public TextMeshProUGUI username;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string _message, string _username)
    {
        message.text = _message;
        username.text += _username;
    }
}

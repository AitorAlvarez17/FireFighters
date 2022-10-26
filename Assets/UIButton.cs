using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public enum ButtonType
    {
        CREATE, 
        JOIN
    }

    [SerializeField]
    private ButtonType buttonType;

    public ButtonType ButtonType1 { get => buttonType; set => buttonType = value; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

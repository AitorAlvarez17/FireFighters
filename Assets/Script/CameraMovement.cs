using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public GameObject focusedPlayer;  
    
    public float rotSpeed;
    public float angle;

    private float remainingAngle;

    public Vector3 initOffset;
    public Vector3 topViewOffset;
    [HideInInspector] public Vector3 transitionOffset;

    public float scrollSpeed;

    private Camera playerCamera;

    void Start()
    {
        transform.position = transform.position + initOffset;
        playerCamera = GetComponent<Camera>();
        transitionOffset = initOffset;
    }

    void LateUpdate()
    {
        if(focusedPlayer != null)
        {
            
            playerCamera.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown("e"))
                    remainingAngle += angle;

                if (Input.GetKeyDown("q"))
                    remainingAngle -= angle;

                float newRemainingAngle = Mathf.MoveTowards(remainingAngle, 0, rotSpeed * Time.deltaTime);
                float delta = remainingAngle - newRemainingAngle;
                remainingAngle = newRemainingAngle;
                transitionOffset = Quaternion.AngleAxis(delta, Vector3.up) * transitionOffset;
                transform.position = focusedPlayer.transform.position + transitionOffset;
                transform.LookAt(focusedPlayer.transform);
            } else if (Input.GetKey(KeyCode.LeftShift))
            {
                transitionOffset = Quaternion.AngleAxis(Input.GetAxis("Rotation") * rotSpeed * 0.001f, Vector3.up) * transitionOffset;
                transform.position = focusedPlayer.transform.position + transitionOffset;
                transform.LookAt(focusedPlayer.transform);
            }
        }
    }
}
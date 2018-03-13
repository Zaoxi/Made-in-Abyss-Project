using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    private GameObject playerCamera;
    private Rigidbody rb;
    public float speed;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
        float moveV = Input.GetAxis("Vertical") * speed;
        float moveH = Input.GetAxis("Horizontal") * speed;
        moveV *= Time.deltaTime;
        moveH *= Time.deltaTime;

        if (moveV != 0 || moveH != 0)
        {
            //transform.Translate(moveH, 0.0f, moveV);
            //rb.MoveRotation(Quaternion.Euler(0.0f, playerCamera.transform.rotation.eulerAngles.y, 0.0f));
            //transform.rotation = Quaternion.Euler(0.0f, playerCamera.transform.eulerAngles.y, 0.0f);
            if(playerCamera.transform.eulerAngles.y != transform.eulerAngles.y)
            {
                float angleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y;
                if(angleTerm > 180f)
                {
                    angleTerm = playerCamera.transform.eulerAngles.y - (transform.eulerAngles.y + 360f);
                }
                else if(angleTerm < -180f)
                {
                    angleTerm = playerCamera.transform.eulerAngles.y + 360f - transform.eulerAngles.y;
                }
                else
                {
                    angleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y;
                }

                transform.Rotate(0f, angleTerm / 10, 0f);
            }
            transform.Translate(moveH, 0.0f, moveV);
        }
        
        


    }
}


public static class PlayerConstant
{
    public static float PLAYER_HEIGHT = 1.2f;
}

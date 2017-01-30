using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float speed;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float dashSpeed = 5.0f;
    public float dashDuration = 1.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private bool dashing = false;
    private float dashStartTime;
    private Vector3 dashDirection;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        Move();
	}

    void Move()
    {
        if(dashing)
        {
            if(Time.time - dashStartTime > dashDuration)
            {
                dashing = false;
            } else
            {
                transform.Translate(dashDirection.normalized * Time.deltaTime * dashSpeed);
                return;
            }

        }
        //rotation
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        //translation
        Vector3 move_direction = new Vector3(0, 0, 0);

        if(Input.GetKey(KeyCode.Z))
        {
            move_direction.z += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            move_direction.z -= 1;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            move_direction.x -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            move_direction.x += 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            move_direction.y += 1;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            move_direction.y -= 1;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            dashing = true;
            dashStartTime = Time.time;
            dashDirection = move_direction;
            transform.Translate(dashDirection.normalized * Time.deltaTime * dashSpeed);
        } else transform.Translate(move_direction.normalized * Time.deltaTime * speed);


    }
}

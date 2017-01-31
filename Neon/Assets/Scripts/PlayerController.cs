﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float speed = 10.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float dashSpeed = 40.0f;
    public float dashDuration = 0.2f;
    public float shootDuration = 0.02f;
    public float maxLaserDistance = 2000.0f;
    public float hitDuration = 0.1f;
    public float hitSpeed = 100.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private bool dashing = false;
    private float dashStartTime;
    private Vector3 dashDirectionWorldSpace;

    private bool shooting = false;
    private float shootingStartTime;

    private bool affectedByHit = false;
    private float hitStartTime;
    private Vector3 hitDirectionWorldSpace;

    private GameObject laser;
    private Rigidbody rb;

    private AudioSource audioSource;
    public AudioClip dashSound;
    public AudioClip shootSound;

    private ParticleSystem particleSystem;

    //pour debug
    public bool isLocal = false;

    // Use this for initialization
    void Start () {
        laser = transform.Find("Laser").gameObject;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();

        //pour debug
        Screen.lockCursor = true;
    }
	
	// Update is called once per frame
	void Update () {
        

        Hit();
        if (!isLocal)
            return;
        if (!affectedByHit)
        {
            Move();
            Shoot();
        }
        if (Input.GetKey(KeyCode.Escape))
            Screen.lockCursor = false;
    }

    void Move()
    {
        //rotation
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);

        if (dashing)
        {
            if (Time.time - dashStartTime > dashDuration)
            {
                dashing = false;
            }
            else
            {
                transform.Translate(dashDirectionWorldSpace.normalized * Time.deltaTime * dashSpeed, Space.World);
                return;
            }

        }

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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            dashDirectionWorldSpace = transform.TransformDirection(move_direction);
            if (DashCheck(dashDirectionWorldSpace))
            {
                audioSource.PlayOneShot(dashSound);
                dashing = true;
                dashStartTime = Time.time;
                transform.Translate(dashDirectionWorldSpace.normalized * Time.deltaTime * dashSpeed, Space.World);
            }

        }
        else
        {
            Vector3 move_direction_worldSpace = transform.TransformDirection(move_direction);
            if (MoveCheck(transform.TransformDirection(move_direction_worldSpace))) transform.Translate(move_direction.normalized * Time.deltaTime * speed);
        }


    }
    
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shooting = true;
            shootingStartTime = Time.time;

            audioSource.PlayOneShot(shootSound);


            //raycast
            Vector3 fwd = laser.transform.TransformDirection(Vector3.forward);

            RaycastHit hit;
            float dist = maxLaserDistance;

            if (Physics.Raycast(transform.position, fwd, out hit, dist)) {
                dist = hit.distance;
                if (hit.transform != null)
                    if(hit.transform.gameObject.tag == "Player")
                    {
                        hit.transform.gameObject.GetComponent<PlayerController>().SetHit(fwd);
                    }
            }
            laser.transform.localScale = new Vector3(1, 1, dist);
            laser.GetComponent<LineRenderer>().enabled = true;
            

        } if (shooting)
        {
            if(Time.time - shootingStartTime > shootDuration)
            {
                shooting = false;
                laser.GetComponent<LineRenderer>().enabled = false;
            }
        }
    }

    void SetHit(Vector3 hitDirection)
    {
        if (affectedByHit)
            return;
        else affectedByHit = true;
        hitDirectionWorldSpace = hitDirection;
        hitStartTime = Time.time;
    }

    void Hit()
    {
        if (!affectedByHit)
            return;
        if(Time.time - hitStartTime > hitDuration)
        {
            affectedByHit = false;
            return;
        }
        transform.Translate(hitDirectionWorldSpace.normalized * Time.deltaTime * hitSpeed, Space.World);
    }

    bool DashCheck(Vector3 dashDirection)
    {
        RaycastHit hit;
        float dist = dashDuration * dashSpeed;

        if (Physics.Raycast(transform.position, dashDirection, out hit, dist))
        {
            return false;
        }
        return true;
    }

    bool MoveCheck(Vector3 moveDirection)
    {
        RaycastHit hit;
        float dist = transform.Find("playerMesh").GetComponent<SphereCollider>().radius;

        if (Physics.Raycast(transform.position, moveDirection, out hit, dist))
        {
            return false;
        }
        return true;
    }

    public void Kill()
    {
        particleSystem.Emit(0);
        enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(affectedByHit)
        {
            if (collision.gameObject.tag == "DeathWall")
                Kill();
        }
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void OnCollisionExit(Collision collision)
    {
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;
    public float speed = 10.0f;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float dashSpeed = 40.0f;
    public float dashDuration = 0.2f;
    public float shootDuration = 0.02f;
    public float maxLaserDistance = 2000.0f;
    public float hitDuration = 0.1f;
    public float hitSpeed = 100.0f;

    public float maxEnergy = 100.0f;
    public float energyRecovery = 20.0f; //per seconds
    public float dashEnergy = 50.0f;
    public float shootEnergy = 50.0f;
    private EnergyBarScript GUIEnergybar;
    public RespawnScript respawn;
    //public float dashRecoveryTime = 1.0f;
    //public float shootRecoveryTime = 1.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private bool dashing = false;
    private float dashStartTime;
    private Vector3 dashDirectionWorldSpace;

    private bool shooting = false;
    private float shootingStartTime;

    private float currentEnergie;

    private bool affectedByHit = false;
    private float hitStartTime;
    private Vector3 hitDirectionWorldSpace;

    private GameObject laser;
    private Rigidbody rb;

    private AudioSource audioSource;
    public AudioClip dashSound;
    public AudioClip shootSound;
    public AudioClip errorSound;

    private ParticleSystem particleSystem;

    //pour debug
    public bool isLocal = false;

    // Use this for initialization
    void Start () {
        laser = transform.Find("Laser").gameObject;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
        //GUIEnergybar = transform.Find("EnergyBar").gameObject.GetComponent<EnergyBarScript>();

        currentEnergie = maxEnergy;

        //pour debug
        Screen.lockCursor = true;
        currentHealth = maxHealth;
    }

    void TakeDamage()
    {
        currentHealth -= 10;
        Debug.Log(transform.name + " now has " + currentHealth + "hps");
        if(currentHealth <= 0)
        {
            Kill();
        }
    }
	
	// Update is called once per frame
	void Update () {
        

        Hit();
        if (!isLocal)
            return;
        EnergyUpdate();
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
            //check energy
            if(currentEnergie < dashEnergy)
            {
                //pas assez d'energie
                audioSource.PlayOneShot(errorSound);
                return;
            }
            dashDirectionWorldSpace = transform.TransformDirection(move_direction);
            if (DashCheck(dashDirectionWorldSpace))
            {
                audioSource.PlayOneShot(dashSound);
                dashing = true;
                currentEnergie -= dashEnergy;
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
    
    [Client]
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //check energy
            if (currentEnergie < shootEnergy)
            {
                //pas assez d'energie
                audioSource.PlayOneShot(errorSound);
                return;
            }
            currentEnergie -= shootEnergy;
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
                        CmdPlayerShot(hit.transform.name, fwd);
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

    [Command]
    void CmdPlayerShot(string _ID, Vector3 fwd)
    {
        Debug.Log(_ID + " has been shot.");
        PlayerController _player = GameController.GetPlayer(_ID);
        _player.transform.position += fwd * Time.deltaTime * hitSpeed;
        _player.TakeDamage();
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

    void EnergyUpdate()
    {
        currentEnergie += energyRecovery * Time.deltaTime;
        if (currentEnergie > maxEnergy)
            currentEnergie = maxEnergy;
        //GUIEnergybar.BarDisplay = currentEnergie / maxEnergy;
    }

    public void Kill()
    {
        particleSystem.Play();
        if(respawn != null)
        {
            respawn.SetRespawnTimer();
        }
        
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

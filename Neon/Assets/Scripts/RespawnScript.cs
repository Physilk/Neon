using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour {

    public float respawnTime = 1f;
    private float respawnTimer;
    private bool isRespawning = false;
     
    public GameObject player;
    private Vector3 position;
    private Quaternion rotation;

    // Use this for initialization
    void Start () {
        position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        rotation = new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w);
        player.GetComponent<PlayerController>().respawn = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetRespawnTimer()
    {
        if (!isRespawning)
        {
            isRespawning = true;
            respawnTimer = Time.time;
        }
    }

    void checkRespawn()
    {
        if(isRespawning)
        {
            if(Time.time - respawnTimer > respawnTime)
            {
                Respawn();
            }
        }
    }

    void Respawn()
    {
        player.SetActive(true);
        player.transform.position = new Vector3(position.x, position.y, position.z);
        player.transform.rotation = new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
    }
}

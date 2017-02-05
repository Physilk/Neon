using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour {

    public GameObject player;
    private Transform transform_spawn;

	// Use this for initialization
	void Start () {
        transform_spawn = player.transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

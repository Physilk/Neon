using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {
    [SerializeField]
    Behaviour[] componentsTodisable;

    Camera sceneCamera;

    void Start()
    {
        if(!isLocalPlayer)
        {
            for(int i = 0; i < componentsTodisable.Length; i++)
            {
                componentsTodisable[i].enabled = false;
            }
        }else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerController player = GetComponent<PlayerController>();

        GameController.RegisterPlayer(_netId, player);
    }

    void OnDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameController.UnRegisterPlayer(transform.name);
    }
}

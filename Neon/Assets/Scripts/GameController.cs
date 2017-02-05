using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    private const string PLAYER_ID_PREFIX = "Player";

    private static Dictionary<string, PlayerController> players = new Dictionary<string, PlayerController>();
    private static int WIN_SCORE = 3;

    public static void RegisterPlayer(string _netID, PlayerController _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public bool CheckWin()
    {
        //if (score_player_1 >= WIN_SCORE || score_player_2 >= WIN_SCORE)
        //    return true;
        return false;
    }

    public static PlayerController GetPlayer(string _playerID)
    {
        return players[_playerID];
    }
    // Use this for initialization
    void Start () {
        //player_host = 
        //player_client = 

        //score_player_1 = 0;
        //score_player_2 = 0;
    }
    bool isDead(PlayerController player)
    {
        //if ...
        return false;
    }
    // Update is called once per frame
    void Update () {
        /*if (isDead(player_1))
            score_player_2++;
        else if (isDead(player_2))
            score_player_1++;*/
	}
}

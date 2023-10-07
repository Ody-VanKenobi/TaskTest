using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _player;

    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject newPlayer;

        if (PhotonNetwork.IsMasterClient)
        {
            newPlayer = PhotonNetwork.Instantiate(_player.name, new Vector2(-40f, -10f), Quaternion.identity);
        }
        else
        {
            newPlayer = PhotonNetwork.Instantiate(_player.name, new Vector2(-25f, -10f), Quaternion.identity);
            newPlayer.GetComponent<PhotonView>().RPC("ChangeColorToRed", RpcTarget.AllBuffered);
        }
    }
}

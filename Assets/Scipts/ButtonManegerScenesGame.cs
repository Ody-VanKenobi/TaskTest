using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


public class ButtonManegerScenesGame : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _buttonOpenMenu;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private int _playersReady = 0;
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _winerPopUp;
    [SerializeField] private Text _resultText; 
    private List<Player> _players;
    private void Start()
    {
        Initializations();
        StartGame();
    }
    private void LateUpdate()
    {
       
    }
    private void Initializations()
    {
        _resultText = GameObject.FindGameObjectWithTag("WinLoseText").GetComponent<Text>();
    }
    private void StartGame()
    {
        _players = new List<Player>(FindObjectsOfType<Player>());
        _winerPopUp.SetActive(false);
        _photonView = GetComponent<PhotonView>();
        _menu.SetActive(false);
    }
    public void OpenMenu()
    {
        _menu.SetActive(true);
    }
    public void CloseMenu()
    {
        _menu.SetActive(false);
    }
    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
    }
    public void ButtonRestarGame()
    {
        _photonView.RPC("IncrementPlayersReady", RpcTarget.AllBuffered);
    }
    private void RefreshPlayersList()
    {
        _players = new List<Player>(FindObjectsOfType<Player>());
    }
    public void GameOver()
    {
        RefreshPlayersList();
        if (PhotonNetwork.IsMasterClient)
        {
            Player winner = null;
            foreach (Player player in _players)
            {
                if (player.GetHealth() > 0 && (winner == null || player.GetHealth() > winner.GetHealth()))
                {
                    winner = player;
                }
            }
            string winnerMessage = winner != null ? $"{winner.GetNickName()} выиграл с {winner.GetHealth()} здоровьем и {winner.GetCoinCount()} монетами." : "Нет победителя";
            _photonView.RPC("DeclareWinner", RpcTarget.AllBuffered, winnerMessage);
        }
    }
    [PunRPC]
    void DeclareWinner(string winnerMessage)
    {
        _resultText.text = winnerMessage;
        _winerPopUp.SetActive(true);
        _buttonOpenMenu.SetActive(false);
    }
    [PunRPC]
    void IncrementPlayersReady()
    {
        _playersReady++;

        if (_playersReady >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
    }

}

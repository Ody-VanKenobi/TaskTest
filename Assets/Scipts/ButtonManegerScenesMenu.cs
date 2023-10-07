using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ButtonManegerScenesMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _connectToServer;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private InputField _inputCreate;
    [SerializeField] private InputField _inputJoin;
    [SerializeField] private InputField _inputName;
    private void Start()
    {
        StartMenu();
        Save();
    }
    private void StartMenu()
    {
        _mainMenu.SetActive(true);
        _connectToServer.SetActive(false);
    }
    public void ClicButtonPlayOpenConnectToServer()
    {
        _mainMenu.SetActive(false);
        _connectToServer.SetActive(true);
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(_inputCreate.text, roomOptions);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_inputJoin.text);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
    public void SaveName()
    {
        PlayerPrefs.SetString("name", _inputName.text);
        PhotonNetwork.NickName = _inputName.text;
    }
    private void Save()
    {
        _inputName.text = PlayerPrefs.GetString("name");
        PhotonNetwork.NickName = _inputName.text;
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}

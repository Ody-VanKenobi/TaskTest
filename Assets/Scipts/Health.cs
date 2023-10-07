using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using Photon.Pun.Demo.PunBasics;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth = 100;
    public event Action<float> HealthChanged;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _currentHealth = _maxHealth;
    }
    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

    private void ChangeHealth(int value)
    {
        _currentHealth += value;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            HealthChanged?.Invoke(0);
        }
        else
        {
            float _currentHealthAsPercentage = (float)_currentHealth / _maxHealth;
            HealthChanged?.Invoke(_currentHealthAsPercentage);
        }
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.F))
                photonView.RPC(nameof(Damage), RpcTarget.AllBuffered);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
        }
        else
        {
            this._currentHealth = (int)stream.ReceiveNext();
        }
    }
   [PunRPC]
    public void Damage()
    {
        ChangeHealth(-10);
    }
}
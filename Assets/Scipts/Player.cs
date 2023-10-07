using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks
{
    private int _coinCount = 0;
    private bool _isAlive = true;
    private float _dirX, _dirY;
    [SerializeField] private float _speed;
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private PhotonView _view;
    [SerializeField] private Text _textName;
    [SerializeField] private Text _coinCountText;
    [SerializeField] private ButtonManegerScenesGame _buttonManager;
    [SerializeField] private Health _health;
    [SerializeField] private Animator _animator;
    private void Start()
    {
        Initializations();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void LateUpdate()
    {
        if (_view.IsMine)
        {
            _coinCountText.text = "Coins: " + _coinCount.ToString();
        }
    }
    private void Initializations()
    {
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _health.HealthChanged += OnHealthChanged;
        _buttonManager = GameObject.FindObjectOfType<ButtonManegerScenesGame>();
        _textName.text = _view.Owner.NickName;
        _joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _view = GetComponent<PhotonView>();
        _coinCountText = GameObject.FindGameObjectWithTag("ÑoinCountText").GetComponent<Text>();
    }
    private void MovePlayer()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2 || !_isAlive) return;

        _dirX = _joystick.Horizontal * _speed;
        _dirY = _joystick.Vertical * _speed;
        if (_view.IsMine)
        {
            _rigidbody.velocity = new Vector2(_dirX, _dirY);
            if (_dirX != 0 || _dirY != 0)
            {
                float angle = Mathf.Atan2(_dirY, _dirX) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                _animator.SetBool("Walk", true);
            }
            else
            {
                _animator.SetBool("Walk", false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_view.IsMine && collision.gameObject.CompareTag("Coin"))
        {
            _view.RPC("DeactivateCoin", RpcTarget.All, collision.gameObject.GetComponent<PhotonView>().ViewID);
            _coinCount++;   
        }
    }
    private void OnHealthChanged(float valueAsPercentage)
    {
        if (valueAsPercentage <= 0)
        {
            _isAlive = false;
            photonView.RPC("DisableJoystick", RpcTarget.AllBuffered);
            _buttonManager.GameOver();
        }
    }
    public int GetCoinCount()
    {
        return _coinCount;
    }

    public float GetHealth()
    {
        return _health.GetCurrentHealth();
    }

    public string GetNickName()
    {
        return _view.Owner.NickName;
    }
    private void OnDestroy()
    {
        _health.HealthChanged -= OnHealthChanged;
    }
    [PunRPC]
    public void DisableJoystick()
    {
        _joystick.enabled = false;
    }

    [PunRPC]
    public void ChangeColorToRed()
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = Color.red;
        }
    }
    [PunRPC]
    void DeactivateCoin(int coinViewId)
    {
        PhotonView.Find(coinViewId).gameObject.SetActive(false);
    }
}
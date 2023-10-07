using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shoot : MonoBehaviourPunCallbacks
{
    private bool _isAlive = true;
    [SerializeField] private Health _health;
    [SerializeField] private PhotonView _view;
    [SerializeField] private Button shootButton;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;

    private void Start()
    {
        Initializations();
    }
    private void Initializations()
    {
        _health = GetComponent<Health>();
        _health.HealthChanged += OnHealthChanged;
        _view = GetComponent<PhotonView>();
        shootButton = GameObject.FindWithTag("Shoot").GetComponent<Button>();
        shootButton.onClick.AddListener(OnShootButtonClick);
    }
    private void OnShootButtonClick()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2 || !_isAlive) return;

        if (photonView.IsMine)
            photonView.RPC("SpawnBullet", RpcTarget.All);
    }
    private void OnDestroy()
    {
        _health.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float valueAsPercentage)
    {
        if (valueAsPercentage <= 0)
        {
            _isAlive = false;
            photonView.RPC("DisableShootButton", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DisableShootButton()
    {
        shootButton.interactable = false;
    }

    [PunRPC]
    private void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
        bullet.GetComponent<Bullet>().SetDirection(Vector2.right);
    }
}



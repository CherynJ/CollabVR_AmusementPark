using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

public class CF_Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public Team team;
    public TextMeshProUGUI nameText;

    public int maxHealth = 100;
    public int health = 100;
    private GameObject XROrigin;

    public static GameObject localPlayerInstance;
    public static event Action OnRespawn;

    private void Awake()
    {
        CF_GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }


    // Start is called before the first frame update
    void Start()
    {
        XROrigin = GameObject.FindGameObjectWithTag("Player");

        if (photonView.IsMine)
        {
            localPlayerInstance = gameObject;
            nameText.text = PhotonNetwork.LocalPlayer.CustomProperties["Name"].ToString();
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        // If the player is killed
        if (health <= 0)
        {
            if (photonView.IsMine)
            {
                // Trigger Respawn Event
                health = maxHealth;
                OnRespawn?.Invoke();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Damage taken: " + damage);
    }


    // Confirm Team Assignment when team is selected
    private void GameManagerOnOnGameStateChanged(GameState obj)
    {
        if (obj == GameState.TeamSelected && photonView.IsMine)
        {
            var teamProp = PhotonNetwork.LocalPlayer.CustomProperties["Team"];
            if (teamProp.ToString() == "BLUE")
            {
                team = Team.BLUE;
            }
            else if (teamProp.ToString() == "RED")
            {
                team = Team.RED;
            }
            else team = Team.NONE;
            Debug.Log("Network Player assigned to team: " + team.ToString());
        }
        
        if (obj == GameState.GameStart && photonView.IsMine)
        {
            // Trigger Respawn Event
            Debug.Log("Invoking Respawn");
            health = maxHealth;
            OnRespawn?.Invoke();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }
}

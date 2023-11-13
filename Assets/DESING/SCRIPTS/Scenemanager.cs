using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Scenemanager : MonoBehaviourPunCallbacks
{
    
    public List<Transform> startPoints;
    public GameObject pointeffect;
    public GameObject level;
    public List<UnityEngine.Color> colors;


    [Header("CANVAS/UI")]
    [Space(30)]
    [Header("PANELS")]
    public GameObject mainPanel;
    public GameObject lobyPanel;
    public GameObject roomPanel;
    public GameObject ingamePanel;
    [Space(30)]
    [Header("MAIN/UI")]
    public GameObject singleButton;
    public GameObject multiButton;
    public InputField nickname;
    public GameObject rotateIcon;
    [Space(30)]
    [Header("LOBBY/UI")]
    public InputField roomInput;
    public GameObject roomPrefab;
    public Transform targetRoom;
    public GameObject[] allRooms;
    [Space(30)]
    [Header("ROOM/UI")]
    public Transform targetParent;
    public GameObject playerPrefab;
    public Text roomnameText;
    public GameObject startIcon;
    public GameObject waitIcon;
    public GameObject error;
    [Space(30)]
    [Header("INGAME/UI")]
    public GameObject infoPanel;
    public GameObject infoPrefab;
    public Image background;

    [Space(40)]
    int infoCount = 0;
    public List<string> players;
    // Start is called before the first frame update
    public void InGame()
    {
        PhotonNetwork.AddCallbackTarget(this);
        
        infoCount = PhotonNetwork.PlayerList.Length;
        Invoke(nameof(DelayInGame), 1f);
    }

    public void DelayInGame()
    {
       
        for (int i = 0; i < infoCount; i++)
        {           
            players.Add(GameObject.Find(PhotonNetwork.PlayerList[i].NickName).name);                        
            GameObject info = Instantiate(infoPrefab, Vector3.zero, Quaternion.identity, infoPanel.transform);

        }


        for (int b = 0; b < players.Count; b++)
        {

            GameObject.Find(players[b]).GetComponent<PlayerC>().mycolor = colors[b];
           

            infoPanel.transform.GetChild(b).transform.GetChild(0).transform.GetComponent<Text>().text = players[b];
            infoPanel.transform.GetChild(b).transform.GetChild(0).transform.GetComponent<Text>().color = GameObject.Find(players[b]).GetComponent<PlayerC>().mycolor;
            infoPanel.transform.GetChild(b).transform.GetChild(1).transform.GetComponent<Text>().text = GameObject.Find(players[b]).GetComponent<PlayerC>().score.ToString();

            GameObject.Find(players[b]).transform.position = startPoints[b].position;
            photonView.RPC("SetColor", RpcTarget.AllBuffered, b );
            infoPanel.transform.GetChild(b).gameObject.SetActive(true);

            GameObject.Find(players[b]).GetComponent<PlayerC>().PlayerStart();


        }

        ingamePanel.SetActive(true);
        PlayerUpdate();
    }

    [PunRPC]
    void SetColor(int index)
    {

        GameObject.Find(players[index]).GetComponent<PlayerC>().mycolor = colors[index];
        GameObject.Find(players[index]).transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colors[index];
        GameObject.Find(players[index]).transform.position = startPoints[index].position;

    }
    
    public void PlayerUpdate()
    {

        for (int b  = 0; b < players.Count; b++)
        {
            infoPanel.transform.GetChild(b).transform.GetChild(0).transform.GetComponent<Text>().text = players[b];
            infoPanel.transform.GetChild(b).transform.GetChild(0).transform.GetComponent<Text>().color = GameObject.Find(players[b]).GetComponent<PlayerC>().mycolor;
            infoPanel.transform.GetChild(b).transform.GetChild(1).transform.GetComponent<Text>().text = GameObject.Find(players[b]).GetComponent<PlayerC>().score.ToString();

        }
     
    }


   
      
}

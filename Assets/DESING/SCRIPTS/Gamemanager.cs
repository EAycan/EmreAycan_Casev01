using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

using Unity.VisualScripting;
using UnityEngine.UI;
//using UnityEditor.Animations;
using TMPro;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviourPunCallbacks
{
    public static Gamemanager gm;
    
    public Scenemanager scenemanager;
    public PhotonView photonview;
    

    private bool roomPlaying = false;
    // Start is called before the first frame update
    void Start()
    {

        PhotonNetwork.AddCallbackTarget(this);
        Application.targetFrameRate = 60;
        gm = this;
        scenemanager.nickname.placeholder.GetComponent<Text>().text = "guest"+Random.Range(10,400);
    
    }

    


    void ConnectPhoton()
    {
        
    }

    public void SetName()
    {
        scenemanager.singleButton.SetActive(true);
        scenemanager.multiButton.SetActive(true);
    }


    public void MultiPlayerConnect()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (!PhotonNetwork.InLobby)
        {
            scenemanager.rotateIcon.SetActive(true);
            if (scenemanager.nickname.textComponent.text.Length > 0)
            {
                PhotonNetwork.NickName = scenemanager.nickname.textComponent.text;
            }
            else
            {
                PhotonNetwork.NickName = scenemanager.nickname.placeholder.GetComponent<Text>().text;
            }


            scenemanager.singleButton.transform.DOMove(new Vector3(-750, scenemanager.singleButton.transform.position.y, 0), 1f);
            scenemanager.multiButton.transform.DOMove(new Vector3(-750, scenemanager.multiButton.transform.position.y, 0), 1f);
        }
        else
        {
            scenemanager.rotateIcon.SetActive(false);
            scenemanager.mainPanel.SetActive(false);
            scenemanager.lobyPanel.SetActive(true);
        }
       
    }

    public void CreateRoom()
    {
        scenemanager.roomnameText.text = "Room : " + scenemanager.roomInput.textComponent.text;
        RoomOptions roomOpts = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 3 };
        PhotonNetwork.CreateRoom(scenemanager.roomInput.textComponent.text, roomOpts);
        scenemanager.roomPanel.SetActive(true);
        scenemanager.lobyPanel.SetActive(false);
    }
    public override void OnCreatedRoom()
    {
      
        UpdatePlayerList();

    }

    public void JoinCreateRoom()
    {
        PhotonNetwork.JoinRoom(scenemanager.roomInput.textComponent.text);
        
    }
    public override void OnConnectedToMaster()
    {

        scenemanager.mainPanel.SetActive(false);
        scenemanager.lobyPanel.SetActive(true);
        scenemanager.rotateIcon.SetActive(false);

        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobiye girildi !");

        //oda yoksa yeni oda olustur
        //PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions { MaxPlayers = 3, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
 
        if (PhotonNetwork.CurrentRoom.IsVisible)
        {
            UpdatePlayerList();
        }
        else
        {
            scenemanager.error.SetActive(true);
        }
            
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    { 
        UpdatePlayerList();
    }


     
    private Dictionary<int, GameObject> playerListItems = new Dictionary<int, GameObject>();
    void UpdatePlayerList()
    {

        foreach (var item in playerListItems.Values)
        {
            Destroy(item);
        }
        playerListItems.Clear();


        string playerList = "";
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerList  = player.NickName;
            GameObject newPlayer = Instantiate(scenemanager.playerPrefab.gameObject, Vector3.zero, Quaternion.identity, scenemanager.targetParent);
            newPlayer.transform.GetChild(0).GetComponent<Text>().text = playerList;
            newPlayer.gameObject.SetActive(true);
            playerListItems.Add(player.ActorNumber, newPlayer);
            Debug.Log("PlayerList :" + playerList);
 
        }


        if (PhotonNetwork.PlayerList.Length>=2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                scenemanager.startIcon.SetActive(true);
                scenemanager.waitIcon.SetActive(false);
            }
            else
            {
                scenemanager.startIcon.SetActive(false);
                scenemanager.waitIcon.SetActive(true);
            }
            
        }
        
    }

    public void LeaveRoom()
    {    
        if (PhotonNetwork.PlayerList.Length>1 && PhotonNetwork.CurrentRoom.IsVisible)
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[1]);
                Debug.Log("Yeni oda lideri atandý: " + PhotonNetwork.PlayerList[1].NickName);
            }
        }
        else
        {
            if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("odayi kapat ! ");
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.RemovedFromList = true;
                PhotonNetwork.CurrentRoom.ClearExpectedUsers();
            }
        
            
        }

        scenemanager.startIcon.SetActive(false);
        scenemanager.waitIcon.SetActive(false);

        PhotonNetwork.LeaveRoom();



    }

    
    public void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            PhotonNetwork.LeaveLobby();
            scenemanager.roomPanel.SetActive(false);
            scenemanager.lobyPanel.SetActive(false);
            scenemanager.mainPanel.SetActive(true);
        }
         
        
    }

    public void ErrorOK()
    {
        
        Application.LoadLevel(0);
        

        
    }
    public override void OnLeftRoom()
    {        
        scenemanager.roomPanel.SetActive(false);
        scenemanager.error.SetActive(false);
        scenemanager.lobyPanel.SetActive(true);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {

        Debug.Log("Error ! Disconnected");

        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int b = 0; b < roomList.Count; b++)
        {
            if (roomList[b].PlayerCount == 0)
            {
                roomList[b].RemovedFromList = true;                                
            }
        }

        
        for (int i = 0; i < roomList.Count; i++)
        {
            
            if (roomList[i].IsVisible)
            {
                if (scenemanager.targetRoom.transform.childCount <= roomList.Count - 1)
                {
                    if (roomList[i].PlayerCount > 0)
                    {
                        GameObject newPref = Instantiate(scenemanager.roomPrefab.gameObject, Vector3.zero, Quaternion.identity, scenemanager.targetRoom.transform);
                        newPref.transform.GetChild(0).GetComponent<Text>().text = roomList[i].Name;
                        newPref.SetActive(true);
                    }
                }
                else
                {
                    if (scenemanager.targetRoom.transform.childCount>0)
                    {
                        for (int b = 0; b < scenemanager.targetRoom.transform.childCount; b++)
                        {
                            Destroy(scenemanager.targetRoom.transform.GetChild(b));
                        }
                    }
                    
                }
            }
                              
        }
    }        
    public void RoomStartGame()
    {        
        SendFunctionToOtherPlayers();         
    }
    private void SendFunctionToOtherPlayers()
    {        
        photonView.RPC("FunctionForOtherPlayers", RpcTarget.All);                
    }


    [PunRPC]
    private void RoomPlayControll()
    {
        roomPlaying = true;
    }    

    int counter = 0;

    [PunRPC]
    private  void FunctionForOtherPlayers()
    {

        PhotonNetwork.CurrentRoom.IsVisible = false;
        //PhotonNetwork.CurrentRoom.IsOpen = false;
        scenemanager.level.gameObject.SetActive(true);
        scenemanager.lobyPanel.SetActive(false);
        scenemanager.roomPanel.SetActive(false);
        scenemanager.ingamePanel.SetActive(true);
        

        Vector3 point = GetPoints();        
        GameObject newPlayer = PhotonNetwork.Instantiate("Player_", point, Quaternion.identity);
         
         
        


         newPlayer.SetActive(true);
        photonView.RPC("AddCount", RpcTarget.All);

    }


    Vector3 GetPoints()
    {   Vector3 spawnpoints = scenemanager.startPoints[0].position;
        photonView.RPC("RemovePoint", RpcTarget.Others);
        return spawnpoints;
       
    }



    [PunRPC]
    void RemovePoint()
    {
        scenemanager.startPoints.RemoveAt(0);
    }


    [PunRPC]
    void AddCount()
    {
        counter++;
        if (counter >= PhotonNetwork.PlayerList.Length)
        {
            scenemanager.InGame();
        }
    }

    public void JoinRoom(Text room_name)
    {
        
            PhotonNetwork.JoinRoom(room_name.text);

            scenemanager.lobyPanel.SetActive(false);
            scenemanager.roomPanel.SetActive(true);
        
        
    }
    public override void OnLeftLobby()
    {
        Debug.Log("Lobiden çýkýldý ! ");
        PhotonNetwork.JoinLobby();

    }

 
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdatePlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int playercount = PhotonNetwork.CurrentRoom.PlayerCount;
           
        }
    }

 
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("ERROR !  Odaya Girilemedi ! ");
        scenemanager.error.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("ERROR ! Herhangi bir odaya girilemedi !");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("ERROR ! Oda kurulamadý !");
        Invoke(nameof(CreateRoom), 1f);
    }

}

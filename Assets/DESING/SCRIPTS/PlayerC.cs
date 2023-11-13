using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerC : MonoBehaviourPunCallbacks
{

    Scenemanager scenemanager;

    public NavMeshAgent nav;
    public GameObject arrow;
    public Vector3 target;
    public GameObject greenArea;
    public Color mycolor;
    bool move = false;
    public bool once;

    public int score = 0;


     

    
    private void Awake()
    {
        
        PhotonNetwork.AddCallbackTarget(this);
        scenemanager = GameObject.FindAnyObjectByType<Scenemanager>();
        if (photonView.IsMine)
        {
            photonView.RPC("SetName", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
            arrow.SetActive(true);
          
            // DelayColoring();




        }

    }
 

    public void PlayerStart()
    {
        nav.transform.parent = null;
        once = true;
    }

 
    [PunRPC]
    void SetName(string name)
    {
        transform.name = name;
    }
 
 
    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && once)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray,out hit ,100f))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        greenArea.SetActive(true);
                        move = true;
                        
                    }
                    if (move && hit.transform.gameObject.layer==3)
                    {
                        greenArea.SetActive(false);
                        scenemanager.pointeffect.transform.position = hit.point;
                        scenemanager.pointeffect.SetActive(true);
                        Invoke(nameof(DisableEffects), 0.5f);
                        target = hit.point;
                        move = false;
                    }
               
                    
                }
            }


            if (target!!=Vector3.zero)
            {
                nav.SetDestination(target);
            }

           
                transform.position = Vector3.MoveTowards(transform.position, nav.transform.position, 10f * Time.deltaTime);
                transform.LookAt(nav.nextPosition);
            
    
        }
    }

    void DisableEffects()
    {
        if (scenemanager.pointeffect.activeSelf)
        {
            scenemanager.pointeffect.SetActive(false);
        }
        
    }

    
   

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Lokal oyuncu pozisyon ve rotasyon bilgilerini diðer oyunculara gönderir
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Diðer oyuncularýn pozisyon ve rotasyon bilgilerini alýr
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }




    [PunRPC]
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.layer == 6)
            {

                Destroy(other.gameObject);
                score += 10;
                photonView.RPC("UpdateScore", RpcTarget.AllBuffered, score);

            }
        }   
        
    }

    [PunRPC]
    void UpdateScore(int sc2)
    {
        score = sc2;
        GameObject.FindObjectOfType<Scenemanager>().PlayerUpdate();
        
    }



}

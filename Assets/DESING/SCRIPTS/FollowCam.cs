using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviourPunCallbacks
{

    public Transform target;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target!=null)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, 35f * Time.deltaTime);
            //transform.position = target.transform.position + offset;
        }
        
    }
}

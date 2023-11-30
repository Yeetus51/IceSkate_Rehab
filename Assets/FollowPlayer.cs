using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [Range(0,1)]
    [SerializeField] private float followSpeed; 
    [SerializeField] private float duckSpeed; 

    [SerializeField] private float duckHeight; 
    [SerializeField] private float unduckHeight; 

    [SerializeField] private float duckRotation; 
    [SerializeField] private float unduckRotation; 

    private float desCamHeight = 0; 
    private float desCamRotaiton = 0; 

    void Start(){
        desCamHeight = unduckHeight;
        desCamRotaiton = unduckRotation;
    }
    private void FixedUpdate(){

        if(player){
            float xPosition = Mathf.Lerp(transform.position.x,player.position.x,followSpeed);
            float yPosition = Mathf.Lerp(transform.position.y,desCamHeight,duckSpeed);
            transform.position = new Vector3(xPosition, yPosition, transform.position.z); 

            float xRotation = Mathf.Lerp(transform.eulerAngles.x, desCamRotaiton, duckSpeed); 
            transform.eulerAngles = new Vector3(xRotation,0,0);
        }
    }

    public void CameraDuck(){
        desCamHeight = duckHeight; 
        desCamRotaiton = duckRotation; 
    }

    public void CameraUnDuck(){
        desCamHeight = unduckHeight; 
        desCamRotaiton = unduckRotation; 
    }
}



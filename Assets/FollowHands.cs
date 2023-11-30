using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FollowHands : MonoBehaviour
{

    [SerializeField] Transform rightHand; 
    [SerializeField] Transform leftHand;


    [SerializeField] Transform rightCollider;  
    [SerializeField] Transform leftCollider;  



    void FixedUpdate(){
        rightCollider.position = rightHand.position; 
        leftCollider.position = leftHand.position; 
    }




    void OnTriggerEnter(Collider other){

        switch(other.tag){

            case "CollectableCoco":
                
                break;
            case "CollectableHotdog":
                
                break;
            case "CollectableSoup":
                
                break;
        }


    }

}

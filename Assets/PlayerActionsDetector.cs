using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rfilkov.components;

public class PlayerActionsDetector : MonoBehaviour
{

    [SerializeField] Transform pelvis; 
    [SerializeField] Transform rightFoot;
    [SerializeField] Transform leftFoot;

    [SerializeField] Transform rightKnee; 
    [SerializeField] Transform leftKnee; 

    [SerializeField] AvatarController avatarController;
    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] float legRaiseThreshhold = 0.2f; 
    [SerializeField] float jumpThreshhold = 0.2f; 
    public bool jumped; 
    public bool rightLegUp; 
    public bool leftLegUp;

    float rightFootAv = 0;
    float leftFootAv = 0;

    float avarageYPos = 1; 

    float previousPlayerYPosition;

    float avarageVelocity; 


    void FixedUpdate(){

        float playerYPosition = avatarController.GetPlayerYPos(); 

        avarageVelocity = avarageVelocity * 0.8f + Mathf.Abs(playerYPosition - previousPlayerYPosition) * 0.2f;  

        previousPlayerYPosition = playerYPosition; 

        if(playerYPosition > 0){
            // if(playerYPosition > avarageYPos) avarageYPos = playerYPosition * 0.01f + avarageYPos * 0.99f; 
            // else avarageYPos = playerYPosition * 0.005f + avarageYPos * 0.995f; 
            float ratio = 1/ (avarageVelocity*100 + 0.99f) *10; 
            ratio = Mathf.Abs(ratio-1);
            float scaledRatio = ratio / 0.8f; // Scaling the ratio to a 0-1 range
            float playerYWeight = 0.001f + 0.009f * scaledRatio; // Linearly interpolate between 0.001 and 0.01
            float avarageYWeight = 0.999f - 0.009f * scaledRatio; // Linearly interpolate between 0.999 and 0.99

            avarageYPos = playerYPosition * playerYWeight + avarageYPos * avarageYWeight;

//            Debug.Log(avarageYPos); 
        }


        if(playerYPosition - avarageYPos > jumpThreshhold){
            playerMovement.DoJump();
            // avarageYPos -= (playerYPosition - avarageYPos) * 0.1f; 
        }


        rightFootAv = rightFootAv * 0.6f + rightFoot.position.y * 0.4f; 
        leftFootAv = leftFootAv * 0.6f + leftFoot.position.y * 0.4f; 

        if(rightFootAv - leftFootAv > legRaiseThreshhold){
            rightLegUp = true;
        }else rightLegUp = false; 

        if(leftFootAv - rightFootAv > legRaiseThreshhold){
            leftLegUp = true;
        }else leftLegUp = false; 


    } 


}

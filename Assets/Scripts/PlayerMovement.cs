using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rfilkov.components;
using Unity.VisualScripting;


public class PlayerMovement : MonoBehaviour
{
    public int sideLanes = 2;

    private int currentLane = 0;
    [SerializeField] AvatarController avatarController;

    [SerializeField] bool keyboardControlls = false;

    [SerializeField] bool disableExternalMovement = true; 
    [SerializeField] float keyboardControllsMovementSpeed;

    [Range(3f,7f)]
    [SerializeField] float distanceMultiplier = 1;


    bool snapping = true;
    bool interpolatedSnapping = true;

    bool jumping; 
    float jumpVelocity; 

    [SerializeField] float gravityStrength;
    [SerializeField] float jumpHight;

    [SerializeField] float movementSpeed; 

    [SerializeField] GameUiManager gameUiManager; 

    [SerializeField] Scroller scroller; 
    [SerializeField] FollowPlayer followPlayer; 
    [SerializeField] PlayerActionsDetector playerActionsDetector; 

    [SerializeField] GameObject character; 

    private int[] completedTutorials = new int[6]; 


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(jumping) Jumping(); 



        if(disableExternalMovement) return; 

        if (Input.GetKeyDown(KeyCode.S)) snapping = !snapping; 
        if (Input.GetKeyDown(KeyCode.D)) interpolatedSnapping = !interpolatedSnapping; 

        if(!keyboardControlls){
            float xPosition = avatarController.GetPlayerXPos() * distanceMultiplier;

            if (snapping && interpolatedSnapping)
            {
                float x = xPosition;
                float newXPostion = x - Mathf.Sin(x * Mathf.PI * 2) * 0.15f;


                transform.position = new Vector3(newXPostion, transform.position.y, transform.position.z);
            }
            else if(snapping)
            {
              transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
            }
            else{
                float x = xPosition / 2;
                float newXPostion = Mathf.Round(x);
                newXPostion *= 2;

                transform.position = new Vector3(newXPostion, transform.position.y, transform.position.z);
            }
        }
        else{

            if (Input.GetKeyDown(KeyCode.A) && currentLane > (sideLanes * -1))
            {
                currentLane--;
            }
            if (Input.GetKeyDown(KeyCode.D) && currentLane < sideLanes)
            {
                currentLane++;
            }

            if(Input.GetKey(KeyCode.Space) && !jumping){
                DoJump();
            }
        }
    }

    public void DoJump(){
        if(!jumping){
            jumping = true; 
            jumpVelocity = jumpHight; 
        }
    }

    void Jumping(){

        transform.position += Vector3.up * jumpVelocity * Mathf.Clamp(scroller.speed,0,0.15f); 

        if(transform.position.y <= 0){
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); 
            jumping = false;
        }

        jumpVelocity -= gravityStrength * Mathf.Clamp(scroller.speed,0,0.15f); 

    }


    private void OnTriggerEnter(Collider other){

        switch (other.tag)
        {
            case "CameraDuck":
                followPlayer.CameraDuck();
                break;

            case "RightLegUp":
                if(!playerActionsDetector.rightLegUp){
                    gameUiManager.InvokeVigniette();
                }
                break;
            
            case "LeftLegUp":
                if(!playerActionsDetector.leftLegUp){
                    gameUiManager.InvokeVigniette();
                }
                break;

            case "TutorialMoveLeft":
                if(GetTutorialCount(Tutorialtype.MoveLeft) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.MoveLeft);
                    IncrementTutorialCount(Tutorialtype.MoveLeft); 
                }
                break;

            case "TutorialMoveRight":
                if(GetTutorialCount(Tutorialtype.MoveRight) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.MoveRight);
                    IncrementTutorialCount(Tutorialtype.MoveRight); 
                }
                break;

            case "TutorialJump":
                if(GetTutorialCount(Tutorialtype.Jump) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.Jump);
                    IncrementTutorialCount(Tutorialtype.Jump); 
                }
                break;

            case "TutorialRightLegUp":
                if(GetTutorialCount(Tutorialtype.RightLegUp) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.RightLegUp);
                    IncrementTutorialCount(Tutorialtype.RightLegUp); 
                }
                break;

            case "TutorialLeftLegUp":
                if(GetTutorialCount(Tutorialtype.LeftLegUp) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.LeftLegUp);
                    IncrementTutorialCount(Tutorialtype.LeftLegUp); 
                }
                break;

            case "TutorialCrouch":
                if(GetTutorialCount(Tutorialtype.Crouch) < 2){
                    gameUiManager.InvokeTutorial(Tutorialtype.Crouch);
                    IncrementTutorialCount(Tutorialtype.Crouch); 
                }
                break;

            case "Jump":
                gameUiManager.InvokeVigniette();
                break;
            case "Wall":
                gameUiManager.InvokeVigniette();
                break;
        }        
    }



    void IncrementTutorialCount(Tutorialtype tutorialType) => completedTutorials[(int)tutorialType]++;
    int GetTutorialCount(Tutorialtype tutorialType) => completedTutorials[(int)tutorialType];

    private void OnTriggerExit(Collider other){

        if(other.tag == "CameraDuck"){
            followPlayer.CameraUnDuck();
            return; 
        }

    }

    // private void FixedUpdate(){
    //     if(jumping) Jumping(); 

    //     UpdateLane();
    // }

    private void UpdateLane() => character.transform.position = new Vector3(Mathf.Lerp(character.transform.position.x,currentLane * 2,keyboardControllsMovementSpeed), character.transform.position.y, character.transform.position.z);
}

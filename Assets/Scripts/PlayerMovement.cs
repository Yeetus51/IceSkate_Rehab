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
    [SerializeField] float keyboardControllsMovementSpeed;

    [Range(3f,7f)]
    [SerializeField] float distanceMultiplier = 1;


    bool toggle;

    bool jumping; 
    float jumpVelocity; 

    [SerializeField] float gravityStrength;
    [SerializeField] float jumpHight;

    [SerializeField] float movementSpeed; 

    [SerializeField] GameUiManager gameUiManager; 

    [SerializeField] Scroller scroller; 
    [SerializeField] FollowPlayer followPlayer; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) toggle = !toggle; 
        // if (keyboardControlls)
        // {
   

        //     return; 
        // }

        if(!keyboardControlls){
            transform.position = new Vector3(avatarController.GetPlayerXPos() * distanceMultiplier, transform.position.y, transform.position.z);
            if (toggle)
            {
                float x = transform.position.x;
                float newXPostion = x - Mathf.Sin(x * Mathf.PI * 2) * 0.15f;


                transform.position = new Vector3(newXPostion, transform.position.y, transform.position.z);
            }
            else
            {
                float x = transform.position.x / 2;
                float newXPostion = Mathf.Round(x);
                newXPostion *= 2;

                transform.position = new Vector3(newXPostion, transform.position.y, transform.position.z);
            }
        }
        else{
            // if(Input.GetKey(KeyCode.D)){
            //     transform.position += Vector3.right * movementSpeed; 
            // }
            // if(Input.GetKey(KeyCode.A)){
            //     transform.position -= Vector3.right * movementSpeed; 
            // }
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

    void DoJump(){
        jumping = true; 
        jumpVelocity = jumpHight; 

    }

    void Jumping(){

        transform.position += Vector3.up * jumpVelocity * scroller.speed; 

        if(transform.position.y <= 0){
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); 
            jumping = false;
        }

        jumpVelocity -= gravityStrength * scroller.speed; 

    }


    private void OnTriggerEnter(Collider other){



        if(other.tag == "CameraDuck"){
            followPlayer.CameraDuck();
            return; 
        }
        gameUiManager.InvokeVigniette();
    }

    private void OnTriggerExit(Collider other){

        if(other.tag == "CameraDuck"){
            followPlayer.CameraUnDuck();
            return; 
        }

    }

    private void FixedUpdate(){
        if(jumping) Jumping(); 

        UpdateLane();
    }

    private void UpdateLane() => this.transform.position = new Vector3(Mathf.Lerp(transform.position.x,currentLane * 2,keyboardControllsMovementSpeed), this.transform.position.y, this.transform.position.z);
}

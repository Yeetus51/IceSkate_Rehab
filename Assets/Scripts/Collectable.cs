using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    [SerializeField] bool reset; 
    [SerializeField] bool initiateFlash;  
    [SerializeField] private GameObject godrays; 
    [SerializeField] private GameObject flash;  

    [SerializeField] private float turnSpeed = 0.7f; 


    [SerializeField] float maxFlashSize; 
    [SerializeField] float initialSize; 

    bool flashing; 
    float desSize; 

    [SerializeField] float flashDuration;
    float timer; 
    


    public void InitiateFlash(){
        flash.SetActive(true);
        flashing = true; 
        Reset();
    }

    public void Reset(){
        desSize = maxFlashSize; 
        flash.transform.localScale = Vector3.one * initialSize; 
        transform.localScale = Vector3.one; 
    }

    void FixedUpdate(){

        godrays.transform.Rotate(Vector3.up,turnSpeed); 

        if(initiateFlash){
            InitiateFlash();
            initiateFlash = false; 
        }

        if(reset){
            Reset();
            reset = false;
        }


        if(flashing){
            float newValue = Mathf.Lerp(flash.transform.localScale.x,desSize,timer/flashDuration); 

            if(desSize != 0)flash.transform.localScale = Vector3.one * newValue;
            else transform.localScale = Vector3.one * newValue;

            timer += Time.deltaTime; 
            if(timer > flashDuration && desSize != maxFlashSize){
                flashing = false;
                timer = 0;
                gameObject.SetActive(false);
                return; 
            }

            if(timer > flashDuration){
                desSize = 0; 
                timer = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other){
        if(other.tag == "Hands"){
            InitiateFlash(); 
        } 
    }
}

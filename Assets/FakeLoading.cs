using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeLoading : MonoBehaviour
{

    [SerializeField] AudioSource audioSource; 
    int timer; 

    void FixedUpdate(){
        timer++; 

        if(timer > 6*50){
            gameObject.SetActive(false);
            audioSource.Play();
        }
    }
}

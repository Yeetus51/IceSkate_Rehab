using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 

public class GameUiManager : MonoBehaviour
{

    [SerializeField] Image vigniette; 
    [SerializeField] float vignietteSpeed; 

    private float timer = 0; 


    public void InvokeVigniette(){
        timer = 0; 


    }

    private void FixedUpdate(){
        if(timer < 1){
            timer += vignietteSpeed; 
            float x = -Mathf.Pow(timer,4) + 1; 

            vigniette.color = new Color(vigniette.color.r,vigniette.color.g,vigniette.color.b,x); 
        }
    }
}

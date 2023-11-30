using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class GameUiManager : MonoBehaviour
{

    [SerializeField] Image vigniette; 
    [SerializeField] float vignietteSpeed; 

    [SerializeField] GameObject tutorialContainer; 
    [SerializeField] TMP_Text instructions; 
    [SerializeField] private float tutorialViewDuration = 1; 

    [Space(30f)]

    [SerializeField] Image illsitaition; 

    [SerializeField] Sprite jump; 
    [SerializeField] Sprite moveRight; 
    [SerializeField] Sprite moveLeft; 
    [SerializeField] Sprite crouch; 
    [SerializeField] Sprite rightLeftUp; 
    [SerializeField] Sprite leftLegUp; 

    private float timer = 0; 
    private float tutorioaltimer = 0; 


    public void InvokeVigniette(){
        timer = 0; 
    }

    public void InvokeTutorial(Tutorialtype tutorialtype){

        tutorialContainer.SetActive(true); 
        tutorioaltimer = tutorialViewDuration; 
        switch (tutorialtype)
        {
            case Tutorialtype.MoveRight: 
                instructions.text = "Move to the Right!"; 
                illsitaition.sprite = moveRight; 
                break;

            case Tutorialtype.MoveLeft: 
                instructions.text = "Move to the Left!"; 
                illsitaition.sprite = moveLeft; 
                break;

            case Tutorialtype.Jump: 
                instructions.text = "Do a Jump!"; 
                illsitaition.sprite = jump; 
                break;

            case Tutorialtype.RightLegUp: 
                instructions.text = "Raise your Right Leg!"; 
                illsitaition.sprite = rightLeftUp; 
                break;

            case Tutorialtype.LeftLegUp: 
                instructions.text = "Raise your Left Leg!"; 
                illsitaition.sprite = leftLegUp; 
                break;

            case Tutorialtype.Crouch: 
                instructions.text = "Duck!"; 
                illsitaition.sprite = crouch; 
                break;
        }
    }

    private void FixedUpdate(){
        if(timer < 1){
            timer += vignietteSpeed; 
            float x = -Mathf.Pow(timer,4) + 1; 

            vigniette.color = new Color(vigniette.color.r,vigniette.color.g,vigniette.color.b,x); 
        }
        if(tutorioaltimer > 0){
            tutorioaltimer -= Time.deltaTime; 

            if(tutorioaltimer < 0) {
                tutorioaltimer = 0; 
                tutorialContainer.SetActive(false); 
            }
        }
    }

}

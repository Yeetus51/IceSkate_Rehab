using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System;
using UnityEngine.SceneManagement; 

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
    [SerializeField] AudioSource source;

    private float timer = 0; 
    private float tutorioaltimer = 0;

    [SerializeField] TextMeshProUGUI collectableHotChoc;
    [SerializeField] TextMeshProUGUI hotChocScore;
    int collectedHotChocs = 0;
    [SerializeField] TextMeshProUGUI collectableHotDog;
    [SerializeField] TextMeshProUGUI hotDogScore;
    int collectedHotDog = 0;
    [SerializeField] TextMeshProUGUI collectableSoup;
    [SerializeField] TextMeshProUGUI soupScore;
    int collectedSoup = 0;

    int playerHitCount;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject endGameUI;


    [SerializeField] Scroller scroller; 

    public void InvokeVigniette(){
        playerHitCount++;
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
    public void CollectableScore(string collectable) {
        
        switch (collectable) {
            case "CollectableCoco":
                collectedHotChocs++;
                source.Play();
                collectableHotChoc.text = collectedHotChocs.ToString();
                break;
            case "CollectableHotdog":
                collectedHotDog++;
                source.Play();
                collectableHotDog.text = collectedHotDog.ToString();
                break;
            case "CollectableSoup":
                collectedSoup++;
                source.Play();
                collectableSoup.text = collectedSoup.ToString();
                break;
        }
    }
    public void InvokeEndScreen(int totalCoco, int totalHotDog, int totalSoup, int totalSpawnedObstacles) {
        endGameUI.SetActive(true);
        hotChocScore.text = collectedHotChocs + "/" + totalCoco.ToString();
        hotDogScore.text = collectedHotDog + "/" + totalHotDog.ToString();
        soupScore.text = collectedSoup + "/" + totalSoup.ToString();


        float score = 0;
        if(totalSpawnedObstacles != 0) {
            score = playerHitCount / totalSpawnedObstacles;
        }
        score = (1 - score) * 100; 
        score = Mathf.Clamp(score,0,100); 

        scoreText.text = "Score: " + score.ToString() + "%";

        GameEnded(); 
    }

    void GameEnded(){

        scroller.GameEnded();

        playerHitCount = 0;
        collectedSoup = 0;
        collectedHotDog = 0; 
        collectedHotChocs = 0;


    }


    public void Restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }


}

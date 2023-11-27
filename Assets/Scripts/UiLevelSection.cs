using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class UiLevelSection : MonoBehaviour
{
    [SerializeField] private Button button; 
    [SerializeField] private Image container;  
    [SerializeField] private TMP_Text title; 
    [SerializeField] private TMP_Text duration; 

    [SerializeField] private Image jump; 
    [SerializeField] private Image crouch; 
    [SerializeField] private Image rightLeg; 
    [SerializeField] private Image LeftLeg; 

    [HideInInspector] public LevelSection levelSection; 

    public void SetSectionText(string text){
        title.text = text;    
    }
    public string GetTitle(){
        return title.text; 
    }

    public void SetTimeText(int minutes, int seconds){
        duration.text = minutes.ToString() + "m " + seconds.ToString() +  "s";  
    }

    public void SetIconStates(bool pJump, bool pCrouch, bool pRightLeg, bool pLeftLeg){
        SetImageState(jump,pJump);
        SetImageState(crouch,pCrouch);
        SetImageState(rightLeg,pRightLeg);
        SetImageState(LeftLeg,pLeftLeg);
    }
    private void SetImageState(Image icon, bool state){
        if(icon) icon.color = new Color(1,1,1,state?1:0.3f);
    }
    public void SetContainerState(bool state) => SetImageState(container,state); 

    public void SetContainerPlaying(bool breakTime = false){
        container.color = new Color(breakTime?0.7f:0,breakTime?0.7f:1,0,0.7f);
    }
    public void SetContainerPlayingState(bool state){
        container.color = new Color(state?0.3f:0,1,state?0.3f:0,state?1:0.7f);
    }

    public void SetButtonState(bool state) => button.interactable = state; 

}

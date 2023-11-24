using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UiSettingsManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] TMP_InputField durationMinutes; 
    [SerializeField] TMP_InputField durationSeconds; 
    [SerializeField] Toggle tutorialMode; 
    [SerializeField] Toggle singleLaneMode; 
    [SerializeField] Toggle jump; 
    [SerializeField] Toggle crouch; 
    [SerializeField] Toggle rightLegUp; 
    [SerializeField] Toggle leftLegUp; 
    [SerializeField] Slider maxLaneChange; 
    [SerializeField] Slider laneChangeGap; 
    [SerializeField] Slider laneChangeRate; 
    [SerializeField] Slider bridgeSpawnRate; 
    [SerializeField] Slider obstacleSpawnRate; 
    [SerializeField] Slider speed;


    [Header("Prefabs")]
    [SerializeField] GameObject sectionPrefab; 
    [SerializeField] GameObject addButtonPrefab;

    [Header("Containers")]
    [SerializeField] GameObject sectionsContainer;  
    [SerializeField] GridLayoutGroup sectionsGridLayout; 


    List<LevelSection> levelSections = new List<LevelSection>(); 



    void Start(){

        AddSection(0);
        GameObject newAddButton = Instantiate(addButtonPrefab,sectionsContainer.transform);
        Button AddButton = newAddButton.GetComponent<Button>(); 
        AddButton.onClick.AddListener(() => AddSection(levelSections.Count)); 

        UpdateTimeFrame(); 
    }

    private void AddSection(int index){

        GameObject newSection = Instantiate(sectionPrefab,sectionsContainer.transform);
        Button sectionButton = newSection.GetComponent<Button>(); 
        SetIndex(newSection,sectionsContainer.transform.childCount -2); 
        sectionButton.onClick.AddListener(() => InvokeSectionSettings(levelSections.Count)); 
        Debug.Log("Add section  " + index);

        LevelSection test = new LevelSection(); 
        levelSections.Add(test); 

        UpdateTimeFrame(); 
    } 

    void UpdateTimeFrame(){
        sectionsGridLayout.cellSize = new Vector2((622 - (8*levelSections.Count)) / (levelSections.Count +1), sectionsGridLayout.cellSize.y);
    }

    private void InvokeSectionSettings(int index){


    }


    public void SetIndex(GameObject gameobject, int index)
    {
        // Check if the GameObject is not null and index is valid
        if (gameobject != null && index >= 0)
        {
            // Set the sibling index of the GameObject
            gameobject.transform.SetSiblingIndex(index);
        }
    }




}

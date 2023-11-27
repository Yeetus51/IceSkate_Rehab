using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System;
using System.Linq;

public class UiSettingsManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] TMP_InputField durationMinutes; 
    [SerializeField] TMP_InputField durationSeconds; 
    [SerializeField] TMP_InputField breakMinutes; 
    [SerializeField] TMP_InputField breakSeconds; 
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
    [SerializeField] Button deleteButton; 
    [SerializeField] TMP_Text totalDurationText; 


    [Header("Prefabs")]
    [SerializeField] GameObject sectionPrefab; 
    [SerializeField] GameObject addButtonPrefab;

    [Header("Containers")]
    [SerializeField] GameObject sectionsContainer;  
    [SerializeField] GridLayoutGroup sectionsGridLayout; 

    [Header("Dependencies")]
    [SerializeField] Scroller scroller; 
    [SerializeField] LevelSettingsPresets presets; 
    [SerializeField] LevelManager levelManager; 

    [Header("Settings")]
    [SerializeField] private int maxSectionAmount; 
    [SerializeField] private float tileLengthFactor = 2; 


    private LevelSection activeLevelSection;

    private Button addButton; 


    Dictionary<LevelSection, UiLevelSection> uiLevelSections = new Dictionary<LevelSection, UiLevelSection>();



    void Start(){

        AddSection(0);
        if(uiLevelSections.Count <= 1 && deleteButton.interactable) deleteButton.interactable = false;

        InstantiateAddButton(); 

        UpdateTimeFrame(); 

        durationMinutes.onEndEdit.AddListener(delegate{SetDurationValues();PresetChanged(true);});
        durationSeconds.onEndEdit.AddListener(delegate{SetDurationValues();PresetChanged(true);});

        breakMinutes.onEndEdit.AddListener(delegate{SetBreakValues();PresetChanged(true);});
        breakSeconds.onEndEdit.AddListener(delegate{SetBreakValues();PresetChanged(true);});

        tutorialMode.onValueChanged.AddListener(delegate{activeLevelSection.tutorialMode = tutorialMode.isOn;PresetChanged();}); 
        singleLaneMode.onValueChanged.AddListener(delegate{activeLevelSection.singleLaneMode = singleLaneMode.isOn;PresetChanged();}); 
        jump.onValueChanged.AddListener(delegate{activeLevelSection.obstacleOptions.jump = jump.isOn;PresetChanged();}); 
        crouch.onValueChanged.AddListener(delegate{activeLevelSection.obstacleOptions.crouch = crouch.isOn;PresetChanged();}); 
        rightLegUp.onValueChanged.AddListener(delegate{activeLevelSection.obstacleOptions.rightLeg = rightLegUp.isOn;PresetChanged();}); 
        leftLegUp.onValueChanged.AddListener(delegate{activeLevelSection.obstacleOptions.leftLeg = leftLegUp.isOn;PresetChanged();}); 


        maxLaneChange.onValueChanged.AddListener(delegate{maxLaneChange.value = ValidateAndSetSliderValue(ref activeLevelSection.maxLaneChange,maxLaneChange.value,1,3,1);PresetChanged();});
        laneChangeGap.onValueChanged.AddListener(delegate{laneChangeGap.value = ValidateAndSetSliderValue(ref activeLevelSection.laneChangeGap,laneChangeGap.value,0,3,1);PresetChanged();});
        laneChangeRate.onValueChanged.AddListener(delegate{laneChangeRate.value = ValidateAndSetSliderValue(ref activeLevelSection.laneChangeFrequency,laneChangeRate.value,0.5f,2,0.1f);PresetChanged();});
        bridgeSpawnRate.onValueChanged.AddListener(delegate{bridgeSpawnRate.value = ValidateAndSetSliderValue(ref activeLevelSection.bridgeSpawnRate,bridgeSpawnRate.value,0,10,1);PresetChanged();});
        obstacleSpawnRate.onValueChanged.AddListener(delegate{obstacleSpawnRate.value = ValidateAndSetSliderValue(ref activeLevelSection.obstacleSpawnRate,obstacleSpawnRate.value,0,10,1);PresetChanged();});
        speed.onValueChanged.AddListener(delegate{speed.value = ValidateAndSetSliderValue(ref activeLevelSection.speed,speed.value,0.05f,0.2f,0.025f);PresetChanged();});
        InvokeTutorialPreset(); 
    }
    private void InstantiateAddButton(){
        GameObject newAddButton = Instantiate(addButtonPrefab,sectionsContainer.transform);
        addButton = newAddButton.GetComponent<Button>(); 
        addButton.onClick.AddListener(() => AddSection(uiLevelSections.Count)); 
    }

    float ValidateAndSetSliderValue(ref int setTo, float value,int min, int max, float increment = 0){
        
        float mappedValue = value * (max - min) + min;
        if(increment != 0) mappedValue = RoundToNearest(mappedValue,increment); 

        setTo = (int)mappedValue; 

        return (mappedValue - min)/(max-min); 
    }

    float ValidateAndSetSliderValue(ref float setTo, float value,float min, float max, float increment = 0){
        float mappedValue = value * (max - min) + min;
        if(increment != 0) mappedValue = RoundToNearest(mappedValue,increment); 

        setTo = mappedValue; 

        return (mappedValue - min)/(max-min); 
    }

    int ValidateNumaricalInput(string input){
        int number = 0;
        if(input.Length <= 0) return number;
        input.Trim();
        if(input[input.Length - 1] == 'M' || input[input.Length - 1] == 'S' || 
        input[input.Length - 1] == 'm' || input[input.Length - 1] == 's') {
            input = input.Substring(0, input.Length - 1);
        }
        input.Trim();
        int.TryParse(input,out number); 
        return number;
    }
    void SetDurationValues(){
        int minutes = ValidateNumaricalInput(durationMinutes.text); 
        int timeInSeconds = minutes * 60; 

        int seconds = ValidateNumaricalInput(durationSeconds.text); 
        timeInSeconds += seconds; 


        activeLevelSection.distance = (int)(timeInSeconds * activeLevelSection.speed * 50 /tileLengthFactor); 

        FactorTimeText(durationMinutes, durationSeconds, timeInSeconds);
    }

    void SetBreakValues(){
        int minutes = ValidateNumaricalInput(breakMinutes.text); 
        int timeInSeconds = minutes * 60; 

        int seconds = ValidateNumaricalInput(breakSeconds.text); 
        timeInSeconds += seconds; 

        activeLevelSection.breakDistance = (int)(timeInSeconds * activeLevelSection.speed * 50 /tileLengthFactor); 

        FactorTimeText(breakMinutes, breakSeconds, timeInSeconds);
    }
    int GetDistanceInSeconds(LevelSection section,int distance){
        int seconds = 0; 
        if(distance != 0){
            seconds = Mathf.RoundToInt(distance * tileLengthFactor / (section.speed *50));
        }
        return seconds; 
    }
    void FactorTimeText(TMP_InputField minuets,TMP_InputField seconds, int timeInSeconds){
        minuets.text = (FloorToNearest(timeInSeconds, 60)/60).ToString() + "m"; 
        seconds.text = (timeInSeconds % 60).ToString() + "s"; 

        if(timeInSeconds == 0){
            minuets.text = "";
            seconds.text = "";
        }
    }

    int FloorToNearest(int value, int to){
        int mult = (int)Mathf.Floor(value / to); 
        return mult * to; 
    }
    float RoundToNearest(float value, float to){
        float mult = Mathf.Round(value / to); 
        return mult * to; 
    }

    private void AddSection(int index){

        GameObject newSection = Instantiate(sectionPrefab,sectionsContainer.transform);
        Button sectionButton = newSection.GetComponent<Button>(); 
        SetIndex(newSection,sectionsContainer.transform.childCount -2); 

        LevelSection newLevelSection = new LevelSection(); 
        sectionButton.onClick.AddListener(() => InvokeSectionSettings(newLevelSection));
        UiLevelSection uiLevelSection = newSection.GetComponent<UiLevelSection>();
        uiLevelSection.levelSection = newLevelSection; 

        uiLevelSections.Add(newLevelSection, uiLevelSection); 

        InvokeSectionSettings(newLevelSection); 

        if(uiLevelSections.Count > 1 && !deleteButton.interactable) deleteButton.interactable = true; 
        CheckSectionLimit();
        UpdateTimeFrame(); 

    } 
    private void CheckSectionLimit(){
        if(uiLevelSections.Count >= maxSectionAmount && addButton != null){
            Destroy(addButton.gameObject); 
            addButton = null; 
        }
    }

    void UpdateTimeFrame(){
        int count = uiLevelSections.Count - (addButton?0:1);
        sectionsGridLayout.cellSize = new Vector2((622 - (8*count)) / (count +1), sectionsGridLayout.cellSize.y);
    }

    private void InvokeSectionSettings(LevelSection level){

        if(activeLevelSection != null && uiLevelSections.ContainsKey(activeLevelSection)) SetLevelSectionActive(uiLevelSections[activeLevelSection], false); 
        activeLevelSection = level; 
        SetLevelSectionActive(uiLevelSections[activeLevelSection], true); 
        string sectionTitle = uiLevelSections[activeLevelSection].GetTitle();

        UpdateSectionToggles();

        int breakInSeconds = GetDistanceInSeconds(activeLevelSection,activeLevelSection.breakDistance); 
        FactorTimeText(breakMinutes, breakSeconds, breakInSeconds);
        
        int durationInSeconds = GetDistanceInSeconds(activeLevelSection,activeLevelSection.distance); 
        FactorTimeText(durationMinutes, durationSeconds, durationInSeconds);

        UpdateSectionTime(); 

        tutorialMode.isOn = level.tutorialMode; 
        singleLaneMode.isOn = level.singleLaneMode; 
        jump.isOn = level.obstacleOptions.jump; 
        crouch.isOn = level.obstacleOptions.crouch; 
        rightLegUp.isOn = level.obstacleOptions.rightLeg; 
        leftLegUp.isOn = level.obstacleOptions.leftLeg; 



        SetSliderValue(maxLaneChange, activeLevelSection.maxLaneChange,1,3); 
        SetSliderValue(laneChangeGap, activeLevelSection.laneChangeGap,0,3); 
        SetSliderValue(laneChangeRate, activeLevelSection.laneChangeFrequency,0.5f,2); 
        SetSliderValue(bridgeSpawnRate, activeLevelSection.bridgeSpawnRate,0,10); 
        SetSliderValue(obstacleSpawnRate, activeLevelSection.obstacleSpawnRate,0,10); 
        SetSliderValue(speed, activeLevelSection.speed,0.05f,0.2f); 

        uiLevelSections[activeLevelSection].SetSectionText(sectionTitle); 
    }
    void SetSliderValue(Slider slider, float value, float min, float max){
        value = Mathf.Clamp(value, min, max); 
        slider.value = (value - min)/(max-min); 
    }

    void SetLevelSectionActive(UiLevelSection sectionUi, bool state){
        if(sectionUi) sectionUi.SetContainerState(state); 
    }

    public void SetIndex(GameObject gameobject, int index)
    {
        if (gameobject != null && index >= 0)
        {
            gameobject.transform.SetSiblingIndex(index);
        }
    }
    public void DeleteActiveExercise(){
        UiLevelSection uiElement;
        uiLevelSections.Remove(activeLevelSection, out uiElement); 
        Destroy(uiElement.gameObject);

        InvokeSectionSettings(uiLevelSections.First().Key); 
        if(uiLevelSections.Count <= 1 && deleteButton.interactable) deleteButton.interactable = false;
        if(addButton == null){
            InstantiateAddButton(); 
        }
        UpdateTimeFrame(); 
    }
    public void StartLevels(){
        List<LevelSection> sections = new List<LevelSection>();
        int childCount = sectionsContainer.transform.childCount; 
        for(int i = 0; i < childCount; i++){
            UiLevelSection uiSection =  sectionsContainer.transform.GetChild(i).GetComponent<UiLevelSection>(); 
            if(uiSection != null){
                sections.Add(uiSection.levelSection); 
            }
        }

        levelManager.SetSectionsData(sections);
    }

    public void InvokeTutorialPreset() {
        SetValuesFromPreset(presets.tutorial);
        uiLevelSections[activeLevelSection].SetSectionText("Tutorial"); 
    } 
    public void InvokeEasyPreset(){
        SetValuesFromPreset(presets.easy);
        uiLevelSections[activeLevelSection].SetSectionText("Easy"); 
    }
    public void InvokeMediumPreset() {
        SetValuesFromPreset(presets.medium);
        uiLevelSections[activeLevelSection].SetSectionText("Medium"); 
    }
    public void InvokeHardPreset(){
        SetValuesFromPreset(presets.hard);
        uiLevelSections[activeLevelSection].SetSectionText("Hard"); 
    } 

    private void PresetChanged(bool timeOnly = false){
        if(!timeOnly)uiLevelSections[activeLevelSection].SetSectionText("Custom"); 

        UpdateSectionToggles();
        UpdateSectionTime(); 
    }
    private void UpdateSectionToggles(){
        uiLevelSections[activeLevelSection].SetIconStates(  activeLevelSection.obstacleOptions.jump,
                                                            activeLevelSection.obstacleOptions.crouch,
                                                            activeLevelSection.obstacleOptions.rightLeg,
                                                            activeLevelSection.obstacleOptions.leftLeg);
    }
    private void UpdateSectionTime(){
        SetDurationValues();
        SetBreakValues();

        int totalTimeInSeconds = GetTotalSectionDurationInSeconds(activeLevelSection); 
        int minuets = FloorToNearest(totalTimeInSeconds,60)/60; 
        int seconds = totalTimeInSeconds % 60; 

        uiLevelSections[activeLevelSection].SetTimeText(minuets,seconds); 

        UpdateTotalTime(); 
    }
    private void UpdateTotalTime(){
        int totalDurationInSeconds = 0; 
        foreach(LevelSection section in uiLevelSections.Keys){
            totalDurationInSeconds += GetTotalSectionDurationInSeconds(section); 
        }
        int hours = FloorToNearest(totalDurationInSeconds, 3600)/3600;
        int minuets = (FloorToNearest(totalDurationInSeconds, 60)/60)%60;
        int seconds = totalDurationInSeconds % 60; 
        string format = "Total Duration: "  + (hours>0?hours.ToString() + "h ": "") + minuets.ToString() + "m " + seconds.ToString() + "s";

        totalDurationText.text = format; 

    }
    private int GetTotalSectionDurationInSeconds(LevelSection section){
        int total = 0; 
        total += GetDistanceInSeconds(section,section.distance); 
        total += GetDistanceInSeconds(section, section.breakDistance); 
        return total; 
    }

    private void SetValuesFromPreset(LevelSectionPreset preset){


        durationMinutes.text = preset.durationMinutes.ToString(); 
        durationSeconds.text = preset.durationSeconds.ToString(); 

        breakMinutes.text = preset.breakMinutes.ToString();
        breakSeconds.text = preset.breakSeconds.ToString();

        tutorialMode.isOn = preset.tutorialMode; 
        singleLaneMode.isOn = preset.singleLaneMode; 
        jump.isOn = preset.jump; 
        crouch.isOn = preset.crouch; 
        rightLegUp.isOn = preset.rightLegUp; 
        leftLegUp.isOn = preset.leftLegUp; 

        maxLaneChange.value = (int)preset.maxLaneChange; 
        laneChangeGap.value = (int)preset.laneChangeGap; 
        laneChangeRate.value = preset.laneChangeRate; 
        bridgeSpawnRate.value = (int)preset.bridgeSpawnRate; 
        obstacleSpawnRate.value = (int)preset.obstacleSpawnRate; 
        speed.value = preset.speed; 
        InvokeSectionSettings(activeLevelSection);
    }
}

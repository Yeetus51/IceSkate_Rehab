using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Scroller scroller; 
    public List<LevelSection> levelSections; 


    void Start(){
        scroller.SetLevels(levelSections); 
    }

    public void SetSectionsData(List<LevelSection> sections){
        levelSections.Clear(); 
        foreach(LevelSection section in sections){
            levelSections.Add(section); 
            LevelSection breakSection = new LevelSection();
            breakSection.distance = section.breakDistance; 
            breakSection.breakTime = true; 
            breakSection.speed = section.speed; 
            levelSections.Add(breakSection); 
        }
        scroller.SetLevels(levelSections); 
    }


}

[Serializable]
public class LevelSection{
    [SerializeField] public int distance = 0; 
    [SerializeField] public int breakDistance = 0; 
    [SerializeField] public bool tutorialMode = false; 
    [SerializeField] public bool singleLaneMode = false;
    [SerializeField] public bool breakTime = false;

    [Range(1, 3)]
    [SerializeField] public int maxLaneChange = 1;

    [Range(0, 3)]
    [SerializeField] public int laneChangeGap = 1;

    [Range(0.5f, 3)]
    [SerializeField] public float laneChangeFrequency = 1;

    [Range(0, 10)]
    [SerializeField] public int bridgeSpawnRate = 2;

    [Range(0, 10)]
    [SerializeField] public int obstacleSpawnRate = 2;

    [SerializeField] public ObstacleOptions obstacleOptions; 

    [Space(30f)]
    [Range(0.01f,5)]
    [SerializeField] public float speed = 0.1f;


    public LevelSection(int pDuration = 0, bool pTutorialMode = false, bool pSingleLaneMode = false, int pMaxLangeChange = 1,
                        int pLaneChangeGap = 1, float pLaneChangeFrequency = 1, int pBridgeSpawnRate = 2, int pObstacleSpawnRate = 2,
                        ObstacleOptions pObstacleOptions = null, float pSpeed = 0.1f){
            distance = pDuration;
            tutorialMode = pTutorialMode; 
            singleLaneMode = pSingleLaneMode; 
            maxLaneChange = pMaxLangeChange; 
            laneChangeGap = pLaneChangeGap; 
            laneChangeFrequency = pLaneChangeFrequency; 
            bridgeSpawnRate = pBridgeSpawnRate; 
            obstacleSpawnRate = pObstacleSpawnRate; 
            speed = pSpeed; 

            if(pObstacleOptions == null){
                obstacleOptions = new ObstacleOptions(false,false,false,false);
            }
            else obstacleOptions = pObstacleOptions; 
        }


    }
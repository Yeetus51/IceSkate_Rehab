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
}

[Serializable]
public class LevelSection{
    [SerializeField] public int duration = 10; 
    [SerializeField] public bool tutorialMode = false; 
    [SerializeField] public bool singleLaneMode = false;

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
}
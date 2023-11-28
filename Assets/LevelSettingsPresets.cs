using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSettingsPresets : MonoBehaviour
{

    [SerializeField] public LevelSectionPreset tutorial; 
    [SerializeField] public LevelSectionPreset easy; 
    [SerializeField] public LevelSectionPreset medium; 
    [SerializeField] public LevelSectionPreset hard; 

}

[Serializable]
public class LevelSectionPreset{
    [SerializeField] public float durationMinutes; 
    [SerializeField] public float durationSeconds; 
    [SerializeField] public float breakMinutes; 
    [SerializeField] public float breakSeconds; 
    [SerializeField] public bool tutorialMode; 
    [SerializeField] public bool singleLaneMode; 
    [SerializeField] public bool crouch; 
    [SerializeField] public bool jump; 
    [SerializeField] public bool leftLegUp; 
    [SerializeField] public bool rightLegUp; 
    [SerializeField] public float laneChangeGap; 
    [SerializeField] public float maxLaneChange; 
    [SerializeField] public float bridgeSpawnRate; 
    [SerializeField] public float laneChangeRate; 
    [SerializeField] public float speed;
    [SerializeField] public float obstacleSpawnRate; 
}

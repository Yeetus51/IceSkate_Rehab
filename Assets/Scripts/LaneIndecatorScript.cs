using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.rfilkov.components;


public class LaneIndecatorScript : MonoBehaviour
{
    [SerializeField] AvatarController avatarController;
    [SerializeField] Slider sliderFloat;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        sliderFloat.value = avatarController.GetPlayerXPos() / 2 + 0.5f ;
    }
}

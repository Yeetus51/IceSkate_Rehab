using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.rfilkov.components;


public class PlayerMovement : MonoBehaviour
{
    public int sideLanes = 2;

    private int currentLane = 0;
    [SerializeField] AvatarController avatarController;

    [SerializeField] bool keyboardControlls = false;

    [Range(3f,7f)]
    [SerializeField] float distanceMultiplier = 1; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboardControlls)
        {
            if (Input.GetKeyDown(KeyCode.A) && currentLane > (sideLanes * -1))
            {
                currentLane--;
                UpdateLane();
            }
            if (Input.GetKeyDown(KeyCode.D) && currentLane < sideLanes)
            {
                currentLane++;
                UpdateLane();
            }

            return; 
        }

        transform.position = new Vector3(avatarController.GetPlayerXPos() * distanceMultiplier, transform.position.y, transform.position.z);
        float x = transform.position.x;
        float newXPostion = x - Mathf.Sin(x * Mathf.PI * 2) * 0.15f; 
        transform.position = new Vector3(newXPostion, transform.position.y, transform.position.z);

    }

    private void UpdateLane() => this.transform.position = new Vector3(currentLane * 2, this.transform.position.y, this.transform.position.z);
}

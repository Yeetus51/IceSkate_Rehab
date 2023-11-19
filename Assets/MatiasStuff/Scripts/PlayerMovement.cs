using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int sideLanes = 2;

    private int currentLane = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) && currentLane > (sideLanes * -1)) {
            currentLane--;
            UpdateLane();
        }
        if (Input.GetKeyDown(KeyCode.D) && currentLane < sideLanes)
        {
            currentLane++;
            UpdateLane();
        }

    }

    private void UpdateLane() => this.transform.position = new Vector3(currentLane * 2, this.transform.position.y, this.transform.position.z);
}

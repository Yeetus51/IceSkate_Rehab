using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    private Scroller scroller; 

    public void SetScroller(Scroller pScroller)
    {
        scroller = pScroller;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (scroller)
        {
            this.transform.position -= Vector3.forward * scroller.speed;
        }
    }

}

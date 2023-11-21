using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{

    private Scroller scroller;
    private GameObject prefab;


    public void SetScroller(Scroller pScroller)
    {
        scroller = pScroller;
    }
    public void SetPrefab(GameObject pPrefab)
    {
        prefab = pPrefab; 
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

        if(transform.position.z < -scroller.despawnDistance)
        {
            scroller.ReturnObjectToPool(prefab, this.gameObject); 
        }
    }

}

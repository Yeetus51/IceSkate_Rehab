using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{

    private List<TranslateObject> _planes = new List<TranslateObject>();

    private void Awake()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            _planes.Add(this.transform.GetChild(i).GetComponent<TranslateObject>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(TranslateObject plane in _planes)
        {
            plane.OnObjectTranslated += RepositionPlane;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void RepositionPlane(TranslateObject pPlane)
    {
        //get last plane in list
        TranslateObject _lastPlane = _planes[_planes.Count - 1];
        
        //remove from list
        _planes.Remove(pPlane);

        //add as last plane in list
        _planes.Add(pPlane);

        Vector3 _newPos = new Vector3(0, 0, CalculateZPos(pPlane, _lastPlane));

        pPlane.Translate(pPlane.transform, _newPos);
    }

    private float CalculateZPos(TranslateObject pPlane, TranslateObject pLastPlane) => (pLastPlane.transform.lossyScale.z * 5 + pPlane.transform.lossyScale.z * 5) + pLastPlane.transform.localPosition.z;

    private void OnDestroy()
    {
        foreach (TranslateObject plane in _planes)
        {
            plane.OnObjectTranslated -= RepositionPlane;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TranslateObject : MonoBehaviour
{
    //[SerializeField] private Vector3 _translateToPoint;

    public event Action<TranslateObject> OnObjectTranslated;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (TriggeredTranslate())
        {
            //Translate(this.transform);
            OnObjectTranslated?.Invoke(this);
        }

    }

    protected abstract bool TriggeredTranslate();

    public void Translate(Transform pObjToTranlate, Vector3 pTranslatePoint) => pObjToTranlate.localPosition = pTranslatePoint;

}

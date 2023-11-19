using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePositionBased : TranslateObject
{
    [SerializeField] private Vector3 _positionTranslateTrigger;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override bool TriggeredTranslate() => CloseEnough(this.transform.localPosition, _positionTranslateTrigger, .5f);

    private bool CloseEnough(Vector3 pPosA, Vector3 pPosB, float pAllowedDistance = 0.1f) => (pPosA - pPosB).magnitude <= pAllowedDistance; 
}

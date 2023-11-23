using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _exerciseDuration;
    [SerializeField] private float _exerciseBreakTime;
    [SerializeField] private float _exerciseBreakDuration;

    public static event Action OnExerciseDone;
    public static event Action<bool> OnExeciseRest;

    IEnumerator ExerciseDuration()
    {
        StartCoroutine(ExerciseBreakStart());

        yield return new WaitForSeconds(_exerciseDuration);

        Debug.Log("End excercise");
        OnExerciseDone?.Invoke();
    }

    IEnumerator ExerciseBreakStart()
    {
        yield return new WaitForSeconds(_exerciseBreakTime);

        Debug.Log("Start break");
        StartCoroutine(ExerciseBreakStop());

        OnExeciseRest?.Invoke(true);
    }

    IEnumerator ExerciseBreakStop()
    {
        yield return new WaitForSeconds(_exerciseBreakDuration);

        Debug.Log("Stop break!");

        StartCoroutine(ExerciseBreakStart());
        OnExeciseRest?.Invoke(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExerciseDuration());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

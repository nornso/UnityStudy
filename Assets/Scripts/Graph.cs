using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab = default;

    [SerializeField, Range(10, 100)]
    int resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField]
    FunctionLibrary.FunctionName function = default;

    [SerializeField]
    TransitionMode transtionMode = TransitionMode.Cycle;

    public enum TransitionMode { Cycle, Rnadom };

    Transform[] points;

    private float duration;

    private bool transitioning;

    FunctionLibrary.FunctionName transitonFunciton;

    void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    void Update()
    {
        duration += Time.deltaTime;

        if (transitioning) 
        {
            if (duration >= transitionDuration) 
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;

            transitioning = true;
            transitonFunciton = function;
            PickNexFunctipn();
        }

        if(transitioning)
            UpdateFunctionTransition();
        else
            UpdateFunction();
    }

    private void PickNexFunctipn()
    {
        function = transtionMode == TransitionMode.Cycle ?
                   FunctionLibrary.GetNextFunctionName(function) :
                   FunctionLibrary.GetRandomNextFunctionNameOtherThan(function);
    }

    private void UpdateFunction()
    {
        float time = Time.time;
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float step = 2f / resolution;

        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }

            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }

    private void UpdateFunctionTransition()
    {
        FunctionLibrary.Function
            from = FunctionLibrary.GetFunction(transitonFunciton),
            to = FunctionLibrary.GetFunction(function);

        float progress = duration / transitionDuration;
        float time = Time.time;
        float step = 2f / resolution;

        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;
            }

            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }
}

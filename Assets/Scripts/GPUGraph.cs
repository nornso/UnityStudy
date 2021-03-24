using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField, Range(10, 200)]
    int resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField]
    FunctionLibrary.FunctionName function = default;

    [SerializeField]
    TransitionMode transtionMode = TransitionMode.Cycle;

    public enum TransitionMode { Cycle, Rnadom };

    private float duration;

    private bool transitioning;

    FunctionLibrary.FunctionName transitonFunciton;

    ComputeBuffer positionBuffer;

    private void OnEnable()
    {
        positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
        positionBuffer = null;
    }

    void Update()
    {
    }

    private void PickNexFunctipn()
    {
        function = transtionMode == TransitionMode.Cycle ?
                   FunctionLibrary.GetNextFunctionName(function) :
                   FunctionLibrary.GetRandomNextFunctionNameOtherThan(function);
    }
}

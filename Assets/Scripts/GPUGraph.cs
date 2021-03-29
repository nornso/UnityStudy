using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;

    [SerializeField, Range(10, maxResolution)]
    int resolution = 10;

    [SerializeField, Min(0f)]
    float functionDuration = 1f, transitionDuration = 1f;

    [SerializeField]
    FunctionLibrary.FunctionName function = default;

    [SerializeField]
    TransitionMode transtionMode = TransitionMode.Cycle;

    [SerializeField]
    ComputeShader computeShader = default;

    [SerializeField]
    Material material = default;

    [SerializeField]
    Mesh mesh = default;

    static readonly int positionsId = Shader.PropertyToID("_Positions"),
                        resolutionId = Shader.PropertyToID("_Resolution"),
                        scaleId = Shader.PropertyToID("_Scale"),
                        stepId = Shader.PropertyToID("_Step"),
                        timeId = Shader.PropertyToID("_Time"),
                        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    public enum TransitionMode { Cycle, Rnadom };

    private float duration;

    private bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionBuffer;

    private void OnEnable()
    {
        //3个浮点数,每个4字节
        positionBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionBuffer.Release();
        positionBuffer = null;
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
            transitionFunction = function;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    private void PickNextFunction()
    {
        function = transtionMode == TransitionMode.Cycle ?
                   FunctionLibrary.GetNextFunctionName(function) :
                   FunctionLibrary.GetRandomNextFunctionNameOtherThan(function);
    }

    void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if (transitioning)
        {
            computeShader.SetFloat(transitionProgressId, Mathf.SmoothStep(0f, 1f, duration / transitionDuration));
        }

        int kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * FunctionLibrary.FunctionCount;
        computeShader.SetBuffer(kernelIndex, positionsId, positionBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsId, positionBuffer);
        material.SetVector(scaleId, new Vector4(step, 1f / step));

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }
}

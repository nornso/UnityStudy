using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField, Range(10, 1000)]
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
                        timeId = Shader.PropertyToID("_Time");

    public enum TransitionMode { Cycle, Rnadom };

    private float duration;

    private bool transitioning;

    FunctionLibrary.FunctionName transitonFunciton;

    ComputeBuffer positionBuffer;

    private void OnEnable()
    {
        //3个浮点数,每个4字节
        positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
    }

    private void OnDisable()
    {
        positionBuffer.Release();
        positionBuffer = null;
    }

    void Update()
    {
        UpdateFunctionOnGpu();
    }

    private void PickNexFunctipn()
    {
        function = transtionMode == TransitionMode.Cycle ?
                   FunctionLibrary.GetNextFunctionName(function) :
                   FunctionLibrary.GetRandomNextFunctionNameOtherThan(function);
    }

    void UpdateFunctionOnGpu()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionBuffer);

        material.SetBuffer(positionsId, positionBuffer);
        material.SetVector(scaleId, new Vector4(step, 1f / step));
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionBuffer.count);
    }
}

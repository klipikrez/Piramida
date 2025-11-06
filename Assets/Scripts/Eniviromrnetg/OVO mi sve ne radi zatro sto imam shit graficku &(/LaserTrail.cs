using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LaserTrail : MonoBehaviour
{
    RenderTexture tex;

    Color[] pixels;
    public float dissapateSpeed = 5f;
    public Transform point;
    public float radious = 5f;
    public ComputeShader cShaderClear;
    public ComputeShader cShaderDim;
    public ComputeShader cShaderTrail;
    int kernelClear;
    int kernelLaserDim;
    int kernelLaserTrail;
    Vector2Int dispatchCountClear;
    Vector2Int dispatchCountDim;
    Vector2Int dispatchCountTrail;
    public int texResolution = 256;
    /*public ComputeShader Dim;
    public ComputeShader Line;
    public ComputeShader Reset;
    int kernelDim;
    int kernelReset;
    int kernelLine;*/

    // Start is called before the first frame update
    void Start()
    {
        kernelClear = cShaderClear.FindKernel("CSMain");
        kernelLaserDim = cShaderDim.FindKernel("CSMain");
        //kernelLaserDim = LaserTrailComputeShader.FindKernel("CSMainLaserDim");
        //kernelLaserTrail = LaserTrailComputeShader.FindKernel("CSMainLaserTrail");
        Debug.Log(kernelClear + " " + kernelLaserDim + " " + kernelLaserTrail);

        tex = new RenderTexture(texResolution, texResolution, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        uint threadX = 0;
        uint threadY = 0;
        uint threadZ = 0;

        cShaderClear.GetKernelThreadGroupSizes(kernelClear, out threadX, out threadY, out threadZ);
        dispatchCountClear = Vector2Int.one;
        dispatchCountClear.x = Mathf.CeilToInt(texResolution / threadX) + 1;
        dispatchCountClear.y = Mathf.CeilToInt(texResolution / threadY) + 1;
        Debug.Log(threadX + " " + threadY + " " + threadZ);

        cShaderDim.GetKernelThreadGroupSizes(kernelLaserDim, out threadX, out threadY, out threadZ);
        dispatchCountDim = Vector2Int.one;
        dispatchCountDim.x = Mathf.CeilToInt(texResolution / threadX) + 1;
        dispatchCountDim.y = Mathf.CeilToInt(texResolution / threadY) + 1;

        /*LaserTrailComputeShader.GetKernelThreadGroupSizes(kernelLaserDim, out threadX, out threadY, out threadZ);
        dispatchCountDim = Vector2Int.one;
        dispatchCountDim.x = Mathf.CeilToInt(texResolution / threadX) + 1;
        dispatchCountDim.y = Mathf.CeilToInt(texResolution / threadY) + 1;

        LaserTrailComputeShader.GetKernelThreadGroupSizes(kernelLaserTrail, out threadX, out threadY, out threadZ);
        dispatchCountTrail = Vector2Int.one;
        dispatchCountTrail.x = Mathf.CeilToInt(texResolution / threadX) + 1;
        dispatchCountTrail.y = Mathf.CeilToInt(texResolution / threadY) + 1;*/


        gameObject.GetComponent<Renderer>().material.SetTexture("_LaserTexture", tex);

        //cShader.SetInt("_texResolution", texResolution);
        cShaderClear.SetTexture(kernelClear, "Result", tex);
        cShaderDim.SetTexture(kernelLaserDim, "Result", tex);
        //cShader.SetTexture(kernelLaserDim, "Result", tex);
        //cShader.SetTexture(kernelLaserTrail, "Result", tex);
        cShaderClear.Dispatch(kernelClear, dispatchCountClear.x, dispatchCountClear.y, 1);
        //tex = new Texture2D(256, 256);

        /*
        pixels = new Color[tex.width * tex.height];
        ClearTexture();*/
    }

    // Update is called once per frame
    void Update()
    {
        cShaderClear.Dispatch(kernelClear, dispatchCountClear.x, dispatchCountClear.y, 1);
        //cShaderDim.Dispatch(kernelLaserDim, dispatchCountClear.x, dispatchCountClear.y, 1);
        //LaserTrailComputeShader.Dispatch(kernelLaserDim, dispatchCountDim.x, dispatchCountDim.y, 1);



        //LaserTrailComputeShader.Dispatch(kernelLaserTrail, dispatchCountTrail.x, dispatchCountTrail.y, 1);


        //DarkenTexture();
        //int kernelHandle = shader.FindKernel("CSMain");
        /*Dim.SetTexture(0, "Result", tex);
        Dim.SetTexture(0, "ImageInput", tex);
        Dim.SetFloat("deltaTime", Time.deltaTime);
        Dim.SetFloat("dissapateSpeed", dissapateSpeed);
        Dim.Dispatch(0, tex.width / 8, tex.height / 8, 1);*/
        //tex.SetPixels(pixels);
        //tex.Apply();

    }

    void ClearTexture()
    {

        for (int i = 0; i < pixels.Length; i++)
        {

            pixels[i] = new Color(1, 0, 0, 1);

        }
        // tex.(pixels);
        // tex.Apply();
    }

    void DarkenTexture()
    {
        /*for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r -= Time.deltaTime / dissapateSpeed;
            pixels[i].r = Mathf.Max(pixels[i].r, 0);
        }*/
        //tex.SetPixels(pixels);
        //tex.Apply();

    }

    public void AddTrailAtPoint(Vector2 uv)
    {


        //pixels[(int)(hit.textureCoord.x * tex.width) + (int)(hit.textureCoord.y * tex.height) * tex.height] = new Color(1, 0, 0, 0);
        DrawFilledCircle((int)(uv.x * tex.width), (int)(uv.y * tex.height), radious);

        /*for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {

                pixels[i] -= new Color(Time.deltaTime / dissapateSpeed, 0, 0, 0);
            }
        }*/

        //tex.SetPixels(pixels);
        //tex.Apply();
    }

    private void DrawFilledCircle(int x, int y, float radious)
    {
        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(i, j));

                pixels[j * tex.height + i].r += Mathf.Max((radious - distance) / radious, 0);
                pixels[j * tex.height + i].r = Mathf.Min(pixels[j * tex.height + i].r, 1);
            }
        }

    }
}

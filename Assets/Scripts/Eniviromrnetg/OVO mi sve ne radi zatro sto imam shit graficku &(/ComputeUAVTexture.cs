using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeUAVTexture : MonoBehaviour
{
	public ComputeShader shader;

	private int size = 128;
	private int _kernel;
	public Material _mat;
	public float value = 0;
	public float value2 = 0;
	RenderTexture tex;
	//RenderTexture Tex2;

	void Start()
	{
		_kernel = shader.FindKernel("CSMain");

		tex = new RenderTexture(size, size, 0);
		tex.enableRandomWrite = true;
		tex.Create();

		_mat.SetTexture("_MainTex", tex);

	}

	void Update()
	{
		tex.Create();
		_kernel = shader.FindKernel("CSMain");
		shader.SetTexture(_kernel, "Result", tex);
		//Tex2 = tex;
		shader.SetFloat("value", value);
		shader.SetFloat("value2", value2);
		//shader.SetTexture(_kernel, "Input", Tex2);
		shader.Dispatch(_kernel, Mathf.CeilToInt(size / 8f), Mathf.CeilToInt(size / 8f), 1);

	}

	Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
		// ReadPixels looks at the active RenderTexture.
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}
}

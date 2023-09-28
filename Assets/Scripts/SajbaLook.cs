using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using static Functions;

public class SajbaLook : MonoBehaviour
{
    Camera cam;
    public float multiply = 7f;
    public GameObject[] set;
    public Transform kapak;
    public Transform targetEye;
    public Vector3 addRotation;
    public float EyeLookAtMultyply = 52f;
    public float yeyZAdd = 5f;
    public Transform copyrotationToKapak;
    public Vector2 timeBetweenBlinks = new Vector2(0, 12);
    public float blinkTime = 0.2f;
    public float stayBlinked = 0.05f;
    public SkinnedMeshRenderer kapakRendereer;
    public MeshRenderer eyeRenderer;
    bool crazy = false;
    Coroutine blink;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        blink = StartCoroutine(c_Blink());
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StopCoroutine(blink);
            blink = StartCoroutine(c_Blink());
            eyeRenderer.material.SetFloat("_CARZYMODE", UnityEngine.Random.Range(0f, 10f) > 9f ? (1) : (0));
        }

        Vector3 mousePos = Input.mousePosition;

        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        point *= multiply;
        point.z = -3.63f + 3.5f;
        foreach (GameObject obj in set)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, point, DeltaTimeLerp(0.1f));
        }


        targetEye.LookAt(point * EyeLookAtMultyply + Vector3.forward * yeyZAdd, Vector3.back);

        kapak.rotation = copyrotationToKapak.rotation;
        kapak.rotation *= Quaternion.Euler(addRotation);
    }

    IEnumerator c_Blink()
    {
        while (true)
        {
            float timer = 0;

            while (timer < blinkTime)
            {
                SetKapakBlink((timer / blinkTime) * (timer / blinkTime) * (timer / blinkTime) * 100f);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            SetKapakBlink(100f);
            timer = blinkTime;
            yield return new WaitForSeconds(stayBlinked);
            while (Input.GetMouseButton(0))
            {
                yield return new WaitForEndOfFrame();
            }
            while (timer > 0)
            {
                SetKapakBlink((timer / blinkTime) * (timer / blinkTime) * (timer / blinkTime) * 100f);
                timer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();

            }
            SetKapakBlink(0f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(timeBetweenBlinks.x, timeBetweenBlinks.y));

        }
    }
    void SetKapakBlink(float value)
    {
        kapakRendereer.SetBlendShapeWeight(0, value);
    }

}

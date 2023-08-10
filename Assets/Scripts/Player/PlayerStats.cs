using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float health = 100f;
    public Collider hitbox;
    public float invincibilityTime = 1f;
    public bool canTakeDamage = true;
    public Slider slider;
    public float regen = 25f;
    public int diedTimes = 0;
    public Text text;
    Camera PlayerCamera;
    float timeScreenShake = 0;
    Coroutine screenShakeCorutine;
    Coroutine screenShakeSourceCorutine;
    private void Start()
    {
        if (PlayerCamera == null)
        {
            PlayerCamera = gameObject.GetComponentInChildren<Camera>();
        }
    }
    public void Damage(float damage)
    {
        if (canTakeDamage)
        {
            health -= damage;
            AudioManager.Instance.PlayAudioClip("uoh", 1f);
            StartCoroutine("c_InvincibilityFrames");
        }
    }

    private void Update()
    {
        float updatedHealth = health + regen * Time.deltaTime;
        if (updatedHealth > 100 || updatedHealth < 0)
        {
            if (updatedHealth < 0)
            {
                diedTimes++;
                text.text = diedTimes.ToString();
            }
            health = 100;
        }
        else
        {
            health = updatedHealth;
        }
        slider.value = health / 100f;
    }
    public void ContinuousDamage(float damage)
    {
        health -= damage * Time.deltaTime;
        AudioManager.Instance.PlayAudioClip("uoh", 0.4f, 256);
        //AudioManager.Instance.PlayAudioClip("uoh", 1f);
        //StartCoroutine("c_InvincibilityFrames");
    }

    public void Screenshake(float duration, float strenth, float speed)
    {
        if (screenShakeCorutine != null)
        {
            StopCoroutine(screenShakeCorutine);
            screenShakeCorutine = null;
        }
        timeScreenShake = 0;
        screenShakeCorutine = StartCoroutine(c_ShakeScreen(Random.Range(0f, 52f), duration, strenth, speed));
    }

    public IEnumerator c_ShakeScreen(float seed, float duration, float strenth, float speed)
    {
        do
        {
            transform.Rotate(new Vector3(0, (0.4665f - Mathf.PerlinNoise(seed, timeScreenShake * speed)) * (1f - timeScreenShake / duration) * strenth, 0));

            float camRotationX = PlayerCamera.transform.rotation.eulerAngles.x >= 270 ? PlayerCamera.transform.rotation.eulerAngles.x - 360f : PlayerCamera.transform.rotation.eulerAngles.x;

            camRotationX -= (0.4665f - Mathf.PerlinNoise(seed + 1f, timeScreenShake * speed)) * (1f - timeScreenShake / duration) * strenth;
            camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

            PlayerCamera.gameObject.transform.eulerAngles = new Vector3(
                camRotationX,
                PlayerCamera.gameObject.transform.eulerAngles.y,
                PlayerCamera.gameObject.transform.eulerAngles.z);

            timeScreenShake += Time.deltaTime;
            if (timeScreenShake > duration)
            {
                StopCoroutine(screenShakeCorutine);
                screenShakeCorutine = null;
            }
            yield return new WaitForEndOfFrame();
        } while (true);
    }

    public void ScreenshakeSource(float strenth, float speed, Transform source, float maxDistance)
    {
        if (screenShakeSourceCorutine != null)
        {
            StopCoroutine(screenShakeSourceCorutine);
            screenShakeSourceCorutine = null;
        }
        timeScreenShake = 0;
        screenShakeSourceCorutine = StartCoroutine(c_ShakeScreenSource(Random.Range(0f, 52f), strenth, speed, source, maxDistance));
    }

    public IEnumerator c_ShakeScreenSource(float seed, float strenth, float speed, Transform source, float maxDistance)
    {
        while (source != null)
        {

            float distance = Mathf.Min(Mathf.Max(maxDistance - Vector3.Distance(transform.position, source.position), 0), 1);
            transform.Rotate(new Vector3(0, (0.4665f - Mathf.PerlinNoise(seed, timeScreenShake * speed)) * strenth * distance, 0));

            float camRotationX = PlayerCamera.transform.rotation.eulerAngles.x >= 270 ? PlayerCamera.transform.rotation.eulerAngles.x - 360f : PlayerCamera.transform.rotation.eulerAngles.x;

            camRotationX -= (0.4665f - Mathf.PerlinNoise(seed + 1f, timeScreenShake * speed)) * strenth * distance;
            camRotationX = Mathf.Clamp(camRotationX, -90f, 90f);

            PlayerCamera.gameObject.transform.eulerAngles = new Vector3(
                camRotationX,
                PlayerCamera.gameObject.transform.eulerAngles.y,
                PlayerCamera.gameObject.transform.eulerAngles.z);

            timeScreenShake += Time.deltaTime;
            yield return new WaitForEndOfFrame();

        }
        screenShakeSourceCorutine = null;
    }

    public IEnumerator c_InvincibilityFrames()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(invincibilityTime);
        canTakeDamage = true;
    }


}

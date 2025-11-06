using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float health = 100f;
    public Collider hitbox;
    public float invincibilityTime = 1f;

    public Slider slider;
    public float regen = 25f;
    public int diedTimes = 0;
    public Text text;
    public AudioClip[] continuousDamageDounds;
    Camera PlayerCamera;
    float timeScreenShake = 0;
    Coroutine screenShakeCorutine;
    Coroutine screenShakeSourceCorutine;
    Dictionary<GameObject, float> timeSinceLastAttack = new Dictionary<GameObject, float>();
    System.Guid continuousDamageId;
    bool lost = false;
    private void Start()
    {
        if (PlayerCamera == null)
        {
            PlayerCamera = gameObject.GetComponentInChildren<Camera>();
        }
    }
    public bool Damage(float damage, GameObject sender)
    {
        bool canTakeDamage = false;
        if (timeSinceLastAttack.ContainsKey(sender))
        {
            if (timeSinceLastAttack[sender] < Time.time - invincibilityTime)
            {
                canTakeDamage = true;
            }
        }
        else
        {
            canTakeDamage = true;
            timeSinceLastAttack.Add(sender, Time.time);
        }

        if (canTakeDamage)
        {
            health -= damage;
            AudioManager.Instance.PlayAudioClip("uoh", 1f);
            //StartCoroutine("c_InvincibilityFrames");
            return true;
        }
        return false;
    }

    private void Update()
    {
        float updatedHealth = health + regen * Time.deltaTime;

        if (updatedHealth > 100)
        {
            health = 100;
        }
        else
        {
            if (updatedHealth < 0 && !lost)
            {
                lost = true;
                Loose();
            }
            health = updatedHealth;
        }
        /*if (updatedHealth > 100 || updatedHealth < 0)
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
        }*/
        slider.value = health / 100f;
    }

    private void Loose()
    {
        GameMenu.Instance.Lost();
    }

    public void ContinuousDamage(float damage)
    {
        health -= damage * Time.deltaTime;
        if (!AudioManager.Instance.PlayingAudio.ContainsKey(continuousDamageId))
            continuousDamageId = AudioManager.Instance.PlayAudioClip(continuousDamageDounds[Random.Range(0, continuousDamageDounds.Length)].name, 1.0f, 256);
        //AudioManager.Instance.PlayAudioClip("uoh", 1f);
        //StartCoroutine("c_InvincibilityFrames");
    }

    // Starts a regular screen shake that begins strong and fades out over time.
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

    // Starts an inverse screen shake that begins weak and grows stronger over time.
    public void ScreenshakeInverse(float duration, float strenth, float speed)
    {
        if (screenShakeCorutine != null)
        {
            StopCoroutine(screenShakeCorutine);
            screenShakeCorutine = null;
        }
        timeScreenShake = 0;
        screenShakeCorutine = StartCoroutine(c_ShakeScreenInverse(Random.Range(0f, 52f), duration, strenth, speed));
    }

    public IEnumerator c_ShakeScreenInverse(float seed, float duration, float strenth, float speed)
    {
        do
        {
            transform.Rotate(new Vector3(0, (0.4665f - Mathf.PerlinNoise(seed, timeScreenShake * speed)) * (timeScreenShake / duration) * strenth, 0));

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

    // Starts a distance-based screen shake originating from a specific Transform (source).
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
            transform.Rotate(60f * Time.deltaTime * new Vector3(0, (0.4665f - Mathf.PerlinNoise(seed, timeScreenShake * speed)) * strenth * distance, 0));

            float camRotationX = PlayerCamera.transform.rotation.eulerAngles.x >= 270 ? PlayerCamera.transform.rotation.eulerAngles.x - 360f : PlayerCamera.transform.rotation.eulerAngles.x;

            camRotationX -= 60f * Time.deltaTime * (0.4665f - Mathf.PerlinNoise(seed + 1f, timeScreenShake * speed)) * strenth * distance;
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
    /*
        public IEnumerator c_InvincibilityFrames()
        {
            canTakeDamage = false;
            yield return new WaitForSeconds(invincibilityTime);
            canTakeDamage = true;
        }*/


}

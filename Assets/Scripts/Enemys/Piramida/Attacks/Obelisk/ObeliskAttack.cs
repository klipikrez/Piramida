using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ObeliskAttack : MonoBehaviour
{
    public float attackDelay = 1f;
    public float attackTime = 1f;
    public float chillAroundTime = 5f;
    public float returnTime = 1f;
    public float height = 20f;
    public Transform obj;
    public Material heptagramMaterial;
    float timer = 0f;
    public AnimationCurve kurvaZaObelisk;
    public AnimationCurve kurvaZaObeliskReturn;
    public Animator laserHeptagramAnimation;
    public VisualEffect heptagramParticles;
    public VisualEffect obeliskShock;
    public GameObject attackCollider;
    private void Start()
    {
        heptagramMaterial.SetFloat("_Fade", 1 - (timer / chillAroundTime));
        laserHeptagramAnimation.enabled = true;
        timer = 0;
        obeliskShock.gameObject.SetActive(false);
        AudioManager.Instance.PlayAudioDDDClipStatic("ObeliskTesters", transform.position, 0.75f);
        obj.localPosition = new Vector3(obj.localPosition.x, -height, obj.localPosition.z);
        laserHeptagramAnimation.speed = 1 / attackDelay;
        attackCollider.SetActive(false);
        StartCoroutine(c_Attack());
    }
    public IEnumerator c_Attack()
    {

        yield return new WaitForSeconds(attackDelay);
        heptagramParticles.SendEvent("Stop");
        attackCollider.SetActive(true);
        AudioManager.Instance.PlayAudioClip("PiramidaObelisk");
        obeliskShock.gameObject.SetActive(true);
        obeliskShock.SendEvent("Start");
        AudioManager.Instance.StopAudio("ObeliskTesters");
        while (timer < attackTime)
        {
            obj.localPosition = new Vector3(0, -(1 - kurvaZaObelisk.Evaluate(timer / attackTime)) * height, 0);
            heptagramMaterial.SetFloat("_Fade", 1 - (timer / attackTime));

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        attackCollider.SetActive(false);

        //yield return new WaitForSeconds(chillAroundTime);
        laserHeptagramAnimation.enabled = false;
        timer = 0;
        while (timer < chillAroundTime)
        {



            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        timer = 0;
        while (timer < returnTime)
        {

            obj.localPosition = new Vector3(0, -(1 - kurvaZaObeliskReturn.Evaluate(timer / attackTime)) * height, 0);


            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        Destroy(gameObject);
    }

}

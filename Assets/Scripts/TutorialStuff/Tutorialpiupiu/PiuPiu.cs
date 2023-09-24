using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;
using Yarn.Unity.Example;

public class PiuPiu : BaseEnemy
{
    public VisualEffect feathers;
    public GameObject[] hide;
    public string StartDialogueNodeName = "";
    public string StartDialogueNodeName2 = "";
    bool killed = false;
    bool secondDialogue = false;
    private void Start()
    {
        feathers.SendEvent("Start");
    }
    public override void Damage(float damage)
    {
        feathers.SendEvent("Start");
        AudioManager.Instance.PlayAudioClip("PiuPiu", 0.3f);
        Bullet bullet = RopeTomahawk.Instance.T2.GetComponent<Bullet>();
        bullet.velocity = Vector3.down;
        bullet.transform.position = transform.position;
        foreach (GameObject obj in hide)
        {
            obj.SetActive(false);
        }
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        LMZSKPositionManager.positionIndex = 2;
        LMZSKPositionManager.Instance.ChangePosition();
        DialogueManager.Instance.StartDialogue(StartDialogueNodeName);
        LMZSKPositionManager.positionIndex = 3;
        killed = true;
    }

    private void Update()
    {

        if (!secondDialogue && killed && !DialogueManager.Instance.inDialogue && RopeTomahawk.Instance.hit && CheckIfBulletInside.bullet)
        {
            secondDialogue = true;
            LMZSKPositionManager.positionIndex = 2;
            LMZSKPositionManager.Instance.ChangePosition();
            DialogueManager.Instance.StartDialogue(StartDialogueNodeName2);
            CheckIfBulletInside.Instance.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }


}

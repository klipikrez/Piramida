using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity.Example;

public class PlayerArms : MonoBehaviour
{
    [System.NonSerialized]
    public PlayerInput input;
    [System.NonSerialized]
    public PlayerMovement movement;
    public List<BaseGun> guns;
    public int selectedGun = 0;
    [System.NonSerialized]
    public bool reloading = false;
    [System.NonSerialized]
    public Camera cam;
    [System.NonSerialized]
    public Coroutine fireCorutine;
    [System.NonSerialized]
    public Coroutine reloadCorutine;
    [System.NonSerialized]
    public bool shooting = false;
    public float[] ammoPerArm;
    public float reloadTimer;
    public bool shiftPressed = false;
    [System.NonSerialized]
    public bool inDialogue = false;

    void Start()
    {
        if (cam == null)
        {
            cam = gameObject.GetComponentInChildren<Camera>();
        }
        if (input == null)
        {
            input = new PlayerInput();
            input.Player.Enable();
        }
        if (movement == null)
        {
            movement = gameObject.GetComponent<PlayerMovement>();
        }

        ammoPerArm = new float[guns.Count];
        for (int i = 0; i < ammoPerArm.Length; i++)
        {
            ammoPerArm[i] = guns[i].maxAmmo;
        }

        SubscribeButtonPressFunctions();

    }

    void SubscribeButtonPressFunctions()
    {
        input.Player.Fire.performed += Shoot;
        input.Player.Fire.canceled += StopShoot;

        input.Player.Reload.performed += Reload;
        input.Player.Reload.canceled += StopReload;

        input.Player.Shift.performed += Shift;
        input.Player.Shift.canceled += StopShift;
    }
    public void UnsubscribeButtonPressFunctions()
    {
        input.Player.Fire.performed -= Shoot;
        input.Player.Fire.canceled -= StopShoot;

        input.Player.Reload.performed -= Reload;
        input.Player.Reload.canceled -= StopReload;

        input.Player.Shift.performed -= Shift;
        input.Player.Shift.canceled -= StopShift;
    }

    private void StopReload(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            guns[selectedGun].ReloadCancelled(this);
        }
    }

    private void Shift(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            shiftPressed = true;
            guns[selectedGun].Shift(this);
        }
    }

    private void StopShift(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            shiftPressed = false;
            guns[selectedGun].ShiftCancelled(this);
        }
    }

    private void Reload(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            if (ammoPerArm[selectedGun] != guns[selectedGun].maxAmmo)
            {
                reloading = true;
                guns[selectedGun].Reload(this);
                if (fireCorutine != null)
                    StopCoroutine(fireCorutine);


                reloadCorutine = StartCoroutine(c_Reload());

            }
        }
    }

    private void StopShoot(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            shooting = false;
        }
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (!GameMenu.Instance.paused && !inDialogue)
        {
            shooting = true;
            if (!reloading && ammoPerArm[selectedGun] != 0)
            {
                fireCorutine = StartCoroutine(c_Fire());
            }
        }
    }

    IEnumerator c_Reload()
    {
        while (reloading)
        {
            if (!GameMenu.Instance.paused && !inDialogue)
            {
                reloadTimer += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();

        }
        reloadCorutine = null;
    }
    IEnumerator c_Fire()
    {
        while (shooting && !reloading && ammoPerArm[selectedGun] != 0)
        {
            if (!GameMenu.Instance.paused && !inDialogue)
            {
                guns[selectedGun].Shoot(this);
            }
            yield return new WaitForSeconds(1 / guns[selectedGun].BPS);

        }
        fireCorutine = null;
    }
}

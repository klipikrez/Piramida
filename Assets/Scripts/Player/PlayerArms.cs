using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        input.Player.Fire.performed += Shoot;
        input.Player.Fire.canceled += StopShoot;

        input.Player.Reload.performed += Reload;
        input.Player.Reload.canceled += StopReload;

        input.Player.Shift.performed += Shift;
        input.Player.Shift.canceled += StopShift;

    }

    private void StopReload(InputAction.CallbackContext context)
    {
        guns[selectedGun].ReloadCancelled(this);
    }

    private void Shift(InputAction.CallbackContext context)
    {
        shiftPressed = true;
        guns[selectedGun].Shift(this);
    }

    private void StopShift(InputAction.CallbackContext context)
    {
        shiftPressed = false;
        guns[selectedGun].ShiftCancelled(this);
    }

    private void Reload(InputAction.CallbackContext context)
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

    private void StopShoot(InputAction.CallbackContext context)
    {
        shooting = false;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        shooting = true;
        if (!reloading && ammoPerArm[selectedGun] != 0)
        {
            fireCorutine = StartCoroutine(c_Fire());
        }
    }

    IEnumerator c_Reload()
    {
        while (reloading)
        {
            reloadTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        reloadCorutine = null;
    }
    IEnumerator c_Fire()
    {
        while (shooting && !reloading && ammoPerArm[selectedGun] != 0)
        {
            guns[selectedGun].Shoot(this);
            yield return new WaitForSeconds(1 / guns[selectedGun].BPS);
        }
        fireCorutine = null;
    }
}

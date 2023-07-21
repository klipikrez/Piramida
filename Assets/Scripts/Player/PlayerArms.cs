using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerArms : MonoBehaviour
{
    [System.NonSerialized]
    public PlayerInput input;

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

        ammoPerArm = new float[guns.Count];
        for (int i = 0; i < ammoPerArm.Length; i++)
        {
            ammoPerArm[i] = guns[i].maxAmmo;
        }

        input.Player.Fire.performed += Shoot;
        input.Player.Fire.canceled += StopShoot;

        input.Player.Reload.performed += Reload;

    }

    private void Reload(InputAction.CallbackContext context)
    {
        if (ammoPerArm[selectedGun] != guns[selectedGun].maxAmmo)
        {
            reloading = true;
            guns[selectedGun].Reload(this);
            if (fireCorutine != null)
                StopCoroutine(fireCorutine);


            reloadCorutine = StartCoroutine(Reload_c());

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
            fireCorutine = StartCoroutine(Fire_c());
        }
    }

    IEnumerator Reload_c()
    {
        while (reloading)
        {
            reloadTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        reloadCorutine = null;
    }
    IEnumerator Fire_c()
    {
        while (shooting && !reloading && ammoPerArm[selectedGun] != 0)
        {
            guns[selectedGun].Shoot(this);
            yield return new WaitForSeconds(1 / guns[selectedGun].BPS);
        }
        fireCorutine = null;
    }
}

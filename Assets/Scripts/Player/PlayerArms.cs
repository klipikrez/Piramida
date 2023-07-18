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

    private Coroutine fireCorutine;
    //[System.NonSerialized]
    public bool shooting = false;

    // Start is called before the first frame update
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

        input.Player.Fire.performed += Shoot;
        input.Player.Fire.canceled += StopShoot;

        input.Player.Reload.performed += Reload;

    }

    private void Reload(InputAction.CallbackContext context)
    {
        reloading = true;
        StopCoroutine(fireCorutine);
        fireCorutine = StartCoroutine(Fire());
    }

    private void StopShoot(InputAction.CallbackContext context)
    {
        shooting = false;
        //StopCoroutine(fireCorutine);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        shooting = true;
        if (fireCorutine == null && !reloading)
        {
            fireCorutine = StartCoroutine(Fire());
        }
    }


    IEnumerator Fire()
    {
        while (shooting)
        {

            guns[selectedGun].Shoot(this);
            yield return new WaitForSeconds(1 / guns[selectedGun].BPS);

        }
        fireCorutine = null;
    }

    IEnumerator Reload()
    {

        yield return new WaitUntil(() => guns[selectedGun].lockAndLoaded);
        reloading = false;

    }

    public void EndFire()
    {

    }

}

using System;
using UnityEngine;

public class PlayerExtensionData : MonoBehaviour
{
    public bool hasArms { get; private set; }
    public bool hasGun { get; private set; }

    private GameObject arms;
    private GameObject gun;

    private void Awake()
    {
        arms = transform.GetChild(1).gameObject;
        gun = transform.GetChild(2).gameObject;
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) UnlockArms();
        else if (Input.GetKeyDown(KeyCode.K)) UnlockGun();
    }
    #endif

    public void UnlockArms()
    {
        arms.SetActive(true);
        hasArms = true;
    }

    public void UnlockGun()
    {
        gun.SetActive(true);
        hasGun = true;
    }
}

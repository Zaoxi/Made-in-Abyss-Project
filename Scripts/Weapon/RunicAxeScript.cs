using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunicAxeScript : MonoBehaviour {
    private WeaponManager weaponManager;


	// Use this for initialization
	void Start () {
        weaponManager = WeaponManager.GetInstance();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enter");
            PlayerControl playerControl = weaponManager.GetPlayerControl();
            playerControl.AddAroundWeapon(gameObject);
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exit");
            PlayerControl playerControl = weaponManager.GetPlayerControl();
            playerControl.RemoveAroundWeapon(gameObject);
        }
    }
}

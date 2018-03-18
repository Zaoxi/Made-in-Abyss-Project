using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    private static WeaponManager instance = null;

    public GameObject[] weapons;
    public GameObject[] playerRighthand;
    public GameObject[] playerBack;

    private GameObject[] presentWeapons;

    const int PLAYER_RIGHT_HAND = 0;
    const int PLAYER_BACK1 = 1;
    const int PLAYER_BACK2 = 2;

    public static WeaponManager GetInstance()
    {
        if (instance == null)
            instance = GameObject.FindObjectOfType(typeof(WeaponManager)) as WeaponManager;
        return instance;
    }

    void Start()
    {
        presentWeapons = new GameObject[3];
    }

}


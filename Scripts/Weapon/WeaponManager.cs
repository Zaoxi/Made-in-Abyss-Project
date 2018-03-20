using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private static WeaponManager instance = null;
    private PlayerControl playerControl;

    // 모든 무기 저장
    public GameObject runicAxe;

    public static WeaponManager GetInstance()
    {
        if (instance == null) instance = GameObject.FindObjectOfType<WeaponManager>();
        return instance;
    }

    
    void Start()
    {
        playerControl = PlayerControl.GetInstance();

        //GameObject existRunicAxe = Instantiate(runicAxe);

    }

    public PlayerControl GetPlayerControl()
    {
        return playerControl;
    }
}

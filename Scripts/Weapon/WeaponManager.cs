using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

    static private WeaponManager instance = null;

    private List<GameObject> existWeapons;

    public static WeaponManager GetInstance()
    {
        if (instance == null) instance = GameObject.FindObjectOfType<WeaponManager>();
        return instance;
    }

    private void Start()
    {
        GameObject[] exist;
        exist = GameObject.FindGameObjectsWithTag("Weapons");

        for(int i=0; i<exist.Length; i++)
        {
            existWeapons.Add(exist[i]);
        }
    }

    public void ChangeWeaponTag(GameObject weapon)
    {
        for(int i = 0; i < existWeapons.Count; i++)
        {
            if(weapon == existWeapons[i])
            {
                weapon.tag = "equipped";
            }
        }
    }
}


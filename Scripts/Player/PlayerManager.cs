using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private static PlayerManager instance = null;

    // 리코, 레그, 나나치 중 선택해서 플레이 하기 위한 레퍼런스
    private RikoController rikoController = null;
    private RegController regController = null;
    private NanachiController nanachiController = null;

    public static PlayerManager GetInstance()
    {
        if (instance == null) instance = FindObjectOfType<PlayerManager>();
        return instance;
    }

    void Start()
    {
        // 플레이어 오브젝트 초기화
        rikoController = FindObjectOfType<RikoController>();
        regController = FindObjectOfType<RegController>();
        nanachiController = FindObjectOfType<NanachiController>();
    }


    // 리코 객체 반환
    public RikoController GetRiko()
    {
        return rikoController;
    }
    // 레그 객체 반환
    public RegController GetReg()
    {
        return regController;
    }
    // 나나치 객체 반환
    public NanachiController GetNanachi()
    {
        return nanachiController;
    }
}


public static class PlayerConstant
{
    public static float PLAYER_HEIGHT = 1.2f;
}
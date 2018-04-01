using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private static PlayerManager instance = null;

    // 리코, 레그, 나나치 중 선택해서 플레이 하기 위한 레퍼런스
    private RikoController rikoController = null;
    private RegController regController = null;
    private NanachiController nanachiController = null;

    private CameraControl playerCamera = null;

    // 리코, 레그, 나나치 중 플레이어가 조작하고있는 캐릭터를 선택하는 플래그
    const int RIKO_ACTIVATE = 0;
    const int REG_ACTIVATE = 1;
    const int NANACHI_ACTIVATE = 2;
    private int characterActivated = 0;

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
        playerCamera = CameraControl.GetInstance();
        playerCamera.ChangePlayerObject(rikoController.gameObject);
    }

    void Update()
    {
        CheckChangePlayer();
        if (characterActivated == RIKO_ACTIVATE)
        {
            rikoController.CheckUpdate();
        }
        else if (characterActivated == REG_ACTIVATE)
        {
            regController.CheckUpdate();
        }
        else if (characterActivated == NANACHI_ACTIVATE)
        {
            nanachiController.CheckUpdate();
        }
    }

    void FixedUpdate()
    {
        if (characterActivated == RIKO_ACTIVATE)
        {
            rikoController.CheckFixedUpdate();
        }
        else if (characterActivated == REG_ACTIVATE)
        {
            regController.CheckFixedUpdate();
        }
        else if (characterActivated == NANACHI_ACTIVATE)
        {
            nanachiController.CheckFixedUpdate();
        }
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

    void CheckChangePlayer()
    {
        if(rikoController != null && Input.GetKeyDown(KeyCode.Z))
        {
            playerCamera.ChangePlayerObject(rikoController.gameObject);
            characterActivated = RIKO_ACTIVATE;

        }
        else if(regController != null && Input.GetKeyDown(KeyCode.X))
        {
            playerCamera.ChangePlayerObject(regController.gameObject);
            characterActivated = REG_ACTIVATE;
        }
        else if(nanachiController != null && Input.GetKeyDown(KeyCode.C))
        {
            playerCamera.ChangePlayerObject(nanachiController.gameObject);
            characterActivated = NANACHI_ACTIVATE;
        }
    }
}


public static class PlayerConstant
{
    public static float PLAYER_HEIGHT = 1.2f;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private static PlayerControl instance = null;

    private CrosshairController crossHair;
    // 플레이어 메인 카메라 오브젝트
    private GameObject playerCamera;
    // 플레이어 애니메이터
    private Animator playerAnimator;
    private Rigidbody rb;
    // 플레이어 등 아이템 장착 위치
    private PlayerBack playerBack;
    // 플레이어 오른손 아이템 장착 위치
    private PlayerRightHand playerRightHand;


    private GameObject equipped;
    private List<GameObject> aroundWeapons;


    // 플레이어의 스피드
    public float speed;


    // 점프를 활성화화는 플래그
    private bool space = false;
    // 달리기를 활성화하는 플래그
    private bool run = false;
    // F(아이템 줍기)키를 활성화 하는 플래그
    private int f = 0;


    // 방금 프레임에서 움직인 거리
    private float previousMove = 0f;
    // 점프 후 조작 제어를 위한 시간
    private float startTime = 0f;

    // 플레이어 회전 프레임
    private const float FRAME_ROTATION = 20F;


    public static PlayerControl GetInstance()
    {
        if (instance == null) instance = GameObject.FindObjectOfType<PlayerControl>();
        return instance;
    }


    void Start()
    {
        
        crossHair = GameObject.FindObjectOfType<CrosshairController>();
        aroundWeapons = new List<GameObject>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerBack = GameObject.FindObjectOfType<PlayerBack>();
        playerRightHand = GameObject.FindObjectOfType<PlayerRightHand>();

    }

    void Update()
    {
        CheckGetWeapon();
    }

    void FixedUpdate()
    {
        float moveV = Input.GetAxis("Vertical") * speed;
        float moveH = Input.GetAxis("Horizontal") * speed;

        // 플레이어가 Shift키를 눌렀는지 검사
        CheckRun();


        if (!space)
        {
            // 플레이어의 움직임제어
            previousMove = PlayerMove(moveV, moveH);

            // 플레이어가 Space키를 눌렀는지 검사
            space = CheckSpace();
        } // Space를 누른 뒤 1초 경과 후 플레이어 조작 가능
        else if (Time.time - startTime > 0.8f)
        {
            space = false;
        }
    }

    public void AddAroundWeapon(GameObject weapon)
    {
        aroundWeapons.Add(weapon);
    }

    public void RemoveAroundWeapon(GameObject weapon)
    {
        aroundWeapons.Remove(weapon);
    }

    private void CheckGetWeapon()
    {
        if(aroundWeapons.Count > 0 && Input.GetKeyDown(KeyCode.G))
        {
            GameObject aimed = crossHair.GetAimedObject();

            if(aroundWeapons.Contains(aimed))
            {
                aimed.tag = "Equipped";
                aroundWeapons.Remove(aimed);
                aimed.transform.rotation = playerRightHand.transform.rotation;
                aimed.transform.position = playerRightHand.transform.position;
                aimed.transform.parent = playerRightHand.transform;
            }
        }
    }

    // 플레이어가 Space 버튼을 눌렀는지 검사하는 함수
    private bool CheckSpace()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnimator.SetTrigger("Jump");
            startTime = Time.time;
            return true;
        }
        return false;
    }

    // 플레이어가 LeftShift버튼을 눌렀는지 검사하는 함수
    private void CheckRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerAnimator.SetBool("Run", true);
            run = true;
        }
        else //if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerAnimator.SetBool("Run", false);
            run = false;
        }
    }


    // 플레이어가 이동하는 함수, 플레이어의 다음 이동할 거리 반환(Vector3)
    private float PlayerMove(float moveV, float moveH)
    {
        float move;

        float moveV_deltaTime = moveV * Time.deltaTime;
        float moveH_deltaTime = moveH * Time.deltaTime;

        // 애니메이터 패러미터 세팅
        playerAnimator.SetFloat("moveV", moveV);
        playerAnimator.SetFloat("moveH", moveH);


        // 플레이어가 움직이는 경우
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {

            // 카메라 각도 - 플레이어의 각도 = 플레이어가 회전해야 할 각도
            float angleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y;

            // angleTerm이 180도 보다 크다면, 3xx 도 -> x도
            if (angleTerm > 180f)
            {
                angleTerm -= 360f;
            } // angleTerm이 -180도 보다 작다면, x도 -> 3xx도
            else if (angleTerm < -180f)
            {
                angleTerm += 360f;
            }



            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
            {
                rb.transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                rb.transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                rb.transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > -moveH_deltaTime ? -moveV_deltaTime : moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                rb.transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > moveH_deltaTime ? -moveV_deltaTime : -moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.transform.Rotate(0f, (angleTerm + 90) / FRAME_ROTATION, 0f);
                move = moveH_deltaTime;
                //transform.Translate(0f, 0.0f, moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                rb.transform.Rotate(0f, (angleTerm - 90) / FRAME_ROTATION, 0f);
                move = -moveH_deltaTime;
                //transform.Translate(0f, 0.0f, -moveH_deltaTime);
            }
            else
            {
                rb.transform.Rotate(0f, angleTerm / FRAME_ROTATION, 0f);
                move = moveV_deltaTime;
                //transform.Translate(0f, 0.0f, moveV_deltaTime);
            }
        }
        else
        {
            move = (Mathf.Abs(moveV) < Mathf.Abs(moveH) ? Mathf.Abs(moveH_deltaTime) : moveV_deltaTime);
        }

        if (moveV >= 0f && run)
        {
            move *= 2f;
        }

        rb.transform.Translate(0f, 0f, move);
        //rb.MovePosition(this.transform.position + Vector3.forward*move);

        return move;
    }
}


public static class PlayerConstant
{
    public static float PLAYER_HEIGHT = 1.2f;
}

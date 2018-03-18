using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{

    private GameObject playerCamera;
    private Animator playerAnimator;

    public float speed;
    private bool space = false;
    private bool run = false;
    // 방금 프레임에서 움직인 거리
    private float previousMove = 0f;

    private float startTime = 0f;

    // 플레이어 회전 프레임
    private const float FRAME_ROTATION = 20F;


    // Use this for initialization
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveV = Input.GetAxis("Vertical") * speed;
        float moveH = Input.GetAxis("Horizontal") * speed;
        CheckRun();

        if (!space)
        {
            previousMove = PlayerMove(moveV, moveH);

            space = CheckSpace();
        } // 점프를 누른 뒤 1초 경과 후 플레이어 조작 가능
        else if(Time.time - startTime > 0.5f)
        {
            space = false;
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerAnimator.SetBool("Run", true);
            run = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
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
                transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > -moveH_deltaTime ? -moveV_deltaTime : moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
                //transform.Translate(0f, 0.0f, (moveV_deltaTime > moveH_deltaTime ? -moveV_deltaTime : -moveH_deltaTime));
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0f, (angleTerm + 90) / FRAME_ROTATION, 0f);
                move = moveH_deltaTime;
                //transform.Translate(0f, 0.0f, moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0f, (angleTerm - 90) / FRAME_ROTATION, 0f);
                move = -moveH_deltaTime;
                //transform.Translate(0f, 0.0f, -moveH_deltaTime);
            }
            else
            {
                transform.Rotate(0f, angleTerm / FRAME_ROTATION, 0f);
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

        transform.Translate(0f, 0f, move);

        return move;
    }
}


public static class PlayerConstant
{
    public static float PLAYER_HEIGHT = 1.2f;
}

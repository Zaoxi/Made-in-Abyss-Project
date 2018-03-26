using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RikoController : MonoBehaviour
{
    private CrosshairController crossHair;
    // 플레이어 메인 카메라 오브젝트
    private CameraControl playerCamera;
    // 플레이어 애니메이터
    private Animator playerAnimator;
    // 플레이어 등 아이템 장착 위치
    private PlayerBack playerBack;
    private PlayerBack2 playerBack2;
    // 플레이어 오른손 아이템 장착 위치
    private PlayerRightHand playerRightHand;

    private Rigidbody rb;

    // 플레이어 주변에 존재하는 무기
    private List<GameObject> aroundWeapons;

    // 플레이어가 현재 가지고 있는 무기
    private GameObject[] playerWeapons;
    private int curWeapon = 0;

    // 플레이어 전투태세 전환 플래그
    private bool combat = false;
    private bool swapping = false;

    // 플레이어의 스피드
    public float speed;

    // 점프를 활성화화는 플래그
    private bool space = false;
    // 달리기를 활성화하는 플래그
    private bool run = false;
    // 아이템을 줍는중인지 확인하는 플래그
    private bool pickUp = false;

    // 방금 프레임에서 움직인 거리
    private float previousMove = 0f;
    // 점프 후 조작 제어를 위한 시간
    private float startTime = 0f;

    // 플레이어 회전 프레임
    private const float FRAME_ROTATION = 20F;
    // 플레이어 구르기 애니메이션 대기시간
    private const float WAIT_ROLLING = 1.3f;
    private const float WAIT_PICKUP = 0.7f;
    private const float WAIT_COMBAT = 0.8f;
    private const float WAIT_SWAP = 0.8f;
    private const float SWAP_COMPLETE = 1f;

    void Start()
    {
        // 현재 가지고 있는 무기 초기화
        playerWeapons = new GameObject[2];
        playerWeapons[0] = null;
        playerWeapons[1] = null;
        // 크로스헤어 객체 획득
        crossHair = CrosshairController.GetInstance();
        // 주변에 존재하는 무기를 담을 리스트
        aroundWeapons = new List<GameObject>();
        // 메인 카메라 획득
        playerCamera = CameraControl.GetInstance();
        // 애니메이터 컴포넌트 획득
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        // 리코의 등과 오른손에 아이템을 장착할 위치 획득
        playerBack = GameObject.FindObjectOfType<PlayerBack>();
        playerBack2 = GameObject.FindObjectOfType<PlayerBack2>();
        playerRightHand = GameObject.FindObjectOfType<PlayerRightHand>();


    }

    void Update()
    {
        // 줍기상태, 점프상태, 달리기 상태, 무기 전환 상태가 아닐 때
        if (!pickUp && !space && !run && !swapping)
        {
            CheckGetWeapon();
            CheckSwapWeapon();
            CheckSwitchToIdle();
        }
    }

    void FixedUpdate()
    {
        float moveV = Input.GetAxis("Vertical") * speed;
        float moveH = Input.GetAxis("Horizontal") * speed;

        // 플레이어가 Shift키를 눌렀는지 검사
        CheckRun();

        // 줍기상태, 점프상태, 무기 전환 상태가 아닐 때
        if (!space && !pickUp && !swapping)
        {
            // 플레이어의 움직임제어
            previousMove = PlayerMove(moveV, moveH);

            // 플레이어가 Space키를 눌렀는지 검사
            space = CheckSpace();
        } // Space를 누른 뒤 1초 경과 후 플레이어 조작 가능
        else if (Time.time - startTime > WAIT_ROLLING)
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

    // 비무장 상태로 전환하는 V키를 검사하는 함수
    private void CheckSwitchToIdle()
    {
        // 현재 선택한 무기의 다음무기
        int nextWeapon = (curWeapon + 1) % 2;

        // V키가 눌렸고, 무장상태라면
        if (Input.GetKeyDown(KeyCode.V) && combat)
        {
            // 플레이어의 다음 무기가 없는 경우, 허리에 무기를 보관한다.
            if (playerWeapons[nextWeapon] == null)
            {
                combat = false;
                swapping = true;
                playerAnimator.SetTrigger("Swap");
                playerAnimator.SetBool("Weapon", false);
                StartCoroutine(SwapWeaponPosition(curWeapon, nextWeapon));
            } // 플레이어의 다음 무기가 있는 경우, 어깨에 무기를 보관한다.
            else
            {
                combat = false;
                swapping = true;
                playerAnimator.SetBool("Weapon", false);

                StartCoroutine(SwitchToCombat());
            }
        }
    }

    // 손에 무기를 고정시키거나 어꺠에 고정시키는 함수
    private IEnumerator SwitchToCombat()
    {
        yield return new WaitForSeconds(WAIT_COMBAT);

        // 현재 전투상태라면
        if (combat == true)
        {
            playerWeapons[curWeapon].transform.position = playerRightHand.transform.position;
            playerWeapons[curWeapon].transform.rotation = playerRightHand.transform.rotation;
            playerWeapons[curWeapon].transform.parent = playerRightHand.transform;
        }// 현재 비 전투 상태라면
        else
        {
            playerWeapons[curWeapon].transform.position = playerBack2.transform.position;
            playerWeapons[curWeapon].transform.rotation = playerBack2.transform.rotation;
            playerWeapons[curWeapon].transform.parent = playerBack2.transform;
        }

        yield return new WaitForSeconds(SWAP_COMPLETE);
        swapping = false;
    }


    // 주변에 무기가 있고, 무기를 조준 후 G키를 눌렀을때 아이템을 장착하는 메소드
    private void CheckGetWeapon()
    {
        if (aroundWeapons.Count > 0 && Input.GetKeyDown(KeyCode.G))
        {
            GameObject aimed = crossHair.GetAimedObject();

            if (aroundWeapons.Contains(aimed))
            {
                // 아이템을 줍는 애니메이션 실행
                playerAnimator.SetTrigger("Pickup");
                playerAnimator.SetBool("Weapon", true);
                // 아이템을 줍는 동작동안 조작불가능 플래그
                pickUp = true;
                combat = true;
                aimed.tag = "Equipped";
                aroundWeapons.Remove(aimed);
                StartCoroutine(ChangeWeapon(aimed));
            }
        }
    }

    // 1번키, 2번키를 통해 무기를 스왑하는 메소드
    private void CheckSwapWeapon()
    {
        bool alpha1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool alpha2 = Input.GetKeyDown(KeyCode.Alpha2);
        int nextWeapon = (curWeapon + 1) % 2;

        // 플레이어가 비무장상태이고, 두번째 무기가 있을때 1, 2번 키를 누르면 무장상태로 전환
        if ((alpha1 || alpha2) && !combat && playerWeapons[nextWeapon] != null)
        {
            combat = true;
            swapping = true;
            playerAnimator.SetBool("Weapon", true);

            StartCoroutine(SwitchToCombat());
        } // 플레이어가 1번을 눌렀을때, 현재 무기가 2번 무기인 경우
        else if (alpha1 && curWeapon == 1) // ** 비무장상태에서는 1, 2번 둘중 아무거나 눌러도 무장상태로 변하는 수정작업 필요**
        {
            // 등에 고정된 무기를 바꾸는 애니메이션 실행
            swapping = true;
            playerAnimator.SetTrigger("Swap");
            // 플레이어의 1번 무기가 없는 경우
            if (playerWeapons[0] == null)
            {
                // 플레이어가 무장 상태라면, 비무장 상태로 전환하면서 등에 2번 무기를 고정시킨다.
                if (combat)
                {
                    combat = false;
                    playerAnimator.SetBool("Weapon", false);
                    StartCoroutine(SwapWeaponPosition(1, 0));
                }
                else // 플레이어가 비무장 상태라면 무장상태로 전환하고, 등에 위치한 2번 무기를 오른손에 고정시킨다.
                {
                    combat = true;
                    playerAnimator.SetBool("Weapon", true);
                    StartCoroutine(SwapWeaponPosition(0, 1));
                }
            }
            else // 플레이어가 1, 2번 무기를 모두 가지고 있다면, 1번 무기로 교체한다.
            {
                curWeapon = 0;

                StartCoroutine(SwapWeaponPosition(1, 0));
            }
        } // 2번을 눌렀을때, 플레이어의 무기가 1번 무기인 경우
        else if (alpha2 && curWeapon == 0)
        {
            swapping = true;
            playerAnimator.SetTrigger("Swap");
            // 플레이어의 2번 무기가 없는 경우
            if (playerWeapons[1] == null)
            {
                // 플레이어가 무장상태라면, 비무장 상태로 전환하고, 1번 무기를 등에 고정한다.
                if (combat)
                {
                    combat = false;
                    playerAnimator.SetBool("Weapon", false);
                    StartCoroutine(SwapWeaponPosition(0, 1));
                }
                else // 플레이어가 비무장 상태라면, 무장상태로 전환하고, 1번 무기를 오른손에 고정한다.
                {
                    combat = true;
                    playerAnimator.SetBool("Weapon", true);
                    StartCoroutine(SwapWeaponPosition(1, 0));
                }
            }
            else // 플레이어가 1, 2번 무기를 모두 갖고 있다면, 2번 무기로 교체한다.
            {
                curWeapon = 1;

                StartCoroutine(SwapWeaponPosition(0, 1));
            }
        }

    }

    // 1번 무기, 2번 무기 중 from번을 to번으로 무기의 위치를 교체하는 메소드
    private IEnumerator SwapWeaponPosition(int from, int to)
    {
        // 무기가 스왑되는 순간까지 기다린다.
        yield return new WaitForSeconds(WAIT_SWAP);

        if (playerWeapons[from] != null)
        {
            playerWeapons[from].transform.position = playerBack.transform.position;
            playerWeapons[from].transform.rotation = playerBack.transform.rotation;
            playerWeapons[from].transform.parent = playerBack.transform;
        }

        if (playerWeapons[to] != null)
        {
            playerWeapons[to].transform.position = playerRightHand.transform.position;
            playerWeapons[to].transform.rotation = playerRightHand.transform.rotation;
            playerWeapons[to].transform.parent = playerRightHand.transform;
        }
        // 스왑 애니메이션이 끝날때 까지 기다린다.
        yield return new WaitForSeconds(SWAP_COMPLETE);

        swapping = false;
    }

    // 무기를 교체하는 메소드
    private IEnumerator ChangeWeapon(GameObject weapon)
    {
        // 애니메이션이 완료되는 시간까지 대기
        yield return new WaitForSeconds(WAIT_PICKUP);

        int nextWeapon = (curWeapon + 1) % 2;


        // 플레이어가 현재 무기를 갖고있고,
        if (playerWeapons[curWeapon] != null)
        {   // 플레이어가 다음 무기가 없고, 비무장 상태인 경우 (플레이어가 무장인 상태에서 한개의 무기만 가지는 경우를 고려)
            if (System.NullReferenceException.ReferenceEquals(playerWeapons[nextWeapon], null) && !combat)
            {
                curWeapon = nextWeapon;
                playerWeapons[nextWeapon] = weapon;
            }// 플레이어가 다음 무기도 가지고 있는 경우
            else
            {
                playerWeapons[curWeapon].transform.parent = null;
                playerWeapons[curWeapon].transform.position = transform.position;
                playerWeapons[curWeapon].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 90f));
                playerWeapons[curWeapon] = weapon;
            }
        }
        else // 플레이어의 무기가 하나도 없는 경우
        {
            playerWeapons[curWeapon] = weapon;
        }
        // 플레이어의 손에 무기를 장착한다
        weapon.transform.rotation = playerRightHand.transform.rotation;
        weapon.transform.position = playerRightHand.transform.position;
        weapon.transform.parent = playerRightHand.transform;
        // 아이템 줍기 애니메이션이 끝났음을 가리키는 플래그
        pickUp = false;

        // 플레이어가 무장상태로 전환
        combat = true;
        playerAnimator.SetBool("Weapon", true);
    }

    // 플레이어가 Space 버튼을 눌렀는지 검사하는 메소드
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


    // 플레이어가 LeftShift버튼을 눌렀는지 검사하는 메소드
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


    // 플레이어가 이동하는 메소드, 플레이어의 다음 이동할 거리 반환(Vector3)
    private float PlayerMove(float moveV, float moveH)
    {
        float move;

        float moveV_deltaTime = moveV * Time.deltaTime;
        float moveH_deltaTime = moveH * Time.deltaTime;

        

        // 플레이어가 움직이는 경우
        // 카메라 각도 - 플레이어의 각도 = 플레이어가 회전해야 할 각도
        float angleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y;
        float backAngleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y + 180;

        // angleTerm이 180도 보다 크다면, 3xx 도 -> x도
        if (angleTerm > 180f)
        {
            angleTerm -= 360f;
        } // angleTerm이 -180도 보다 작다면, x도 -> 3xx도
        else if (angleTerm < -180f)
        {
            angleTerm += 360f;
        }

        // backAngleTerm 180도 보다 크다면, 3xx 도 -> x도
        if (backAngleTerm > 180f)
        {
            backAngleTerm -= 360f;
        } // angleTerm이 -180도 보다 작다면, x도 -> 3xx도
        else if (backAngleTerm < -180f)
        {
            backAngleTerm += 360f;
        }

        // 전후좌우 이동 처리
        if (moveH < -0.1f)
        {
            playerAnimator.SetBool("move", true);
            if (moveV > 0.1f)// W, A를 누른경우
            {
                rb.transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
            }
            else if (moveV < -0.1f)// S, A를 누른경우
            {
                rb.transform.Rotate(0f, (backAngleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < moveH_deltaTime ? -moveV_deltaTime : -moveH_deltaTime);
            }
            else // A를 누른경우
            {
                rb.transform.Rotate(0f, (angleTerm - 90) / FRAME_ROTATION, 0f);
                move = -moveH_deltaTime;
            }
        }
        else if (moveH > 0.1f)
        {
            playerAnimator.SetBool("move", true);
            if (moveV > 0.1f)// W, D를 누른경우
            {
                rb.transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
            }
            else if (moveV < -0.1f)// S, D를 누른경우
            {
                rb.transform.Rotate(0f, (backAngleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < -moveH_deltaTime ? -moveV_deltaTime : moveH_deltaTime);
            }
            else // D를 누른경우
            {
                rb.transform.Rotate(0f, (angleTerm + 90) / FRAME_ROTATION, 0f);
                move = moveH_deltaTime;
            }
        }
        else if (moveV > 0.1f) // W를 누른 경우
        {
            playerAnimator.SetBool("move", true);
            rb.transform.Rotate(0f, angleTerm / FRAME_ROTATION, 0f);
            move = moveV_deltaTime;
        }
        else if (moveV < -0.1f) // S를 누른 경우
        {
            playerAnimator.SetBool("move", true);
            rb.transform.Rotate(0f, backAngleTerm / FRAME_ROTATION, 0f);
            move = -moveV_deltaTime;
        }
        else // 아무것도 눌리지 않은 경우
        {
            playerAnimator.SetBool("move", false);
            move = 0f;
        }

        if (run)
        {
            move *= 2f;
        }

        rb.transform.Translate(0f, 0f, move);


        //// 플레이어가 움직이는 경우
        //if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        //{

        //    // 카메라 각도 - 플레이어의 각도 = 플레이어가 회전해야 할 각도
        //    float angleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y;
        //    float backAngleTerm = playerCamera.transform.eulerAngles.y - transform.eulerAngles.y + 180;

        //    // angleTerm이 180도 보다 크다면, 3xx 도 -> x도
        //    if (angleTerm > 180f)
        //    {
        //        angleTerm -= 360f;
        //    } // angleTerm이 -180도 보다 작다면, x도 -> 3xx도
        //    else if (angleTerm < -180f)
        //    {
        //        angleTerm += 360f;
        //    }

        //    // backAngleTerm 180도 보다 크다면, 3xx 도 -> x도
        //    if (backAngleTerm > 180f)
        //    {
        //        backAngleTerm -= 360f;
        //    } // angleTerm이 -180도 보다 작다면, x도 -> 3xx도
        //    else if (backAngleTerm < -180f)
        //    {
        //        backAngleTerm += 360f;
        //    }



        //    if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        //    {
        //        rb.transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
        //        move = (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
        //    }
        //    else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        //    {
        //        rb.transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
        //        move = (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
        //    }
        //    else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        //    {
        //        rb.transform.Rotate(0f, (backAngleTerm - 45) / FRAME_ROTATION, 0f);
        //        move = (moveV_deltaTime < -moveH_deltaTime ? -moveV_deltaTime : moveH_deltaTime);
        //    }
        //    else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        //    {
        //        rb.transform.Rotate(0f, (backAngleTerm + 45) / FRAME_ROTATION, 0f);
        //        move = (moveV_deltaTime < moveH_deltaTime ? -moveV_deltaTime : -moveH_deltaTime);
        //    }
        //    else if (Input.GetKey(KeyCode.D))
        //    {
        //        rb.transform.Rotate(0f, (angleTerm + 90) / FRAME_ROTATION, 0f);
        //        move = moveH_deltaTime;
        //    }
        //    else if (Input.GetKey(KeyCode.A))
        //    {
        //        rb.transform.Rotate(0f, (angleTerm - 90) / FRAME_ROTATION, 0f);
        //        move = -moveH_deltaTime;
        //    }
        //    else if (Input.GetKey(KeyCode.S))
        //    {
        //        rb.transform.Rotate(0f, backAngleTerm / FRAME_ROTATION, 0f);
        //        move = -moveV_deltaTime;
        //    }
        //    else
        //    {
        //        rb.transform.Rotate(0f, angleTerm / FRAME_ROTATION, 0f);
        //        move = moveV_deltaTime;
        //    }
        //}
        //else
        //{
        //    move = (Mathf.Abs(moveV) < Mathf.Abs(moveH) ? Mathf.Abs(moveH_deltaTime) : moveV_deltaTime);
        //}

        //if (run)
        //{
        //    move *= 2f;
        //}

        //rb.transform.Translate(0f, 0f, move);

        return move;
    }

}


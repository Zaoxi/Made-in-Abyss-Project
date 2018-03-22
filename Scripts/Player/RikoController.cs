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
    // 플레이어 오른손 아이템 장착 위치
    private PlayerRightHand playerRightHand;

    private Rigidbody rb;

    // 플레이어 주변에 존재하는 무기
    private List<GameObject> aroundWeapons;

    // 플레이어가 현재 가지고 있는 무기
    private GameObject[] playerWeapons;
    private int curWeapon = 0;

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
    // 플레이어 구르기 애니메이션 대기시간
    private const float WAIT_ROLLING = 1.3f;

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
        playerRightHand = GameObject.FindObjectOfType<PlayerRightHand>();

    }

    void Update()
    {
        CheckGetWeapon();
        CheckSwapWeapon();
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


    // 주변에 무기가 있고, 무기를 조준 후 G키를 눌렀을때 아이템을 장착하는 메소드
    private void CheckGetWeapon()
    {
        if (aroundWeapons.Count > 0 && Input.GetKeyDown(KeyCode.G))
        {
            GameObject aimed = crossHair.GetAimedObject();

            if (aroundWeapons.Contains(aimed))
            {
                aimed.tag = "Equipped";
                aroundWeapons.Remove(aimed);
                ChangeWeapon(aimed);
            }
        }
    }

    // 1번키, 2번키를 통해 무기를 스왑하는 메소드
    private void CheckSwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && curWeapon == 1)
        {
            curWeapon = 0;

            SwapWeaponPosition(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && curWeapon == 0)
        {
            curWeapon = 1;

            SwapWeaponPosition(0, 1);
        }
    }

    // 1번 무기, 2번 무기 중 from번을 to번으로 무기의 위치를 교체하는 메소드
    private void SwapWeaponPosition(int from, int to)
    {
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
    }

    // 무기를 교체하는 메소드
    private void ChangeWeapon(GameObject weapon)
    {
        if (playerWeapons[curWeapon] != null)
        {
            playerWeapons[curWeapon].transform.parent = null;
            playerWeapons[curWeapon].transform.position = transform.position;
            playerWeapons[curWeapon].transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 90f));
        }

        playerWeapons[curWeapon] = weapon;
        weapon.transform.rotation = playerRightHand.transform.rotation;
        weapon.transform.position = playerRightHand.transform.position;
        weapon.transform.parent = playerRightHand.transform;
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


    // 플레이어가 이동하는 메소드, 플레이어의 다음 이동할 거리 반환(Vector3)
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



            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
            {
                rb.transform.Rotate(0f, (angleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > moveH_deltaTime ? moveV_deltaTime : moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
            {
                rb.transform.Rotate(0f, (angleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime > -moveH_deltaTime ? moveV_deltaTime : -moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            {
                rb.transform.Rotate(0f, (backAngleTerm - 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < -moveH_deltaTime ? -moveV_deltaTime : moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
            {
                rb.transform.Rotate(0f, (backAngleTerm + 45) / FRAME_ROTATION, 0f);
                move = (moveV_deltaTime < moveH_deltaTime ? -moveV_deltaTime : -moveH_deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rb.transform.Rotate(0f, (angleTerm + 90) / FRAME_ROTATION, 0f);
                move = moveH_deltaTime;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                rb.transform.Rotate(0f, (angleTerm - 90) / FRAME_ROTATION, 0f);
                move = -moveH_deltaTime;
            }
            else if(Input.GetKey(KeyCode.S))
            {
                rb.transform.Rotate(0f, backAngleTerm / FRAME_ROTATION, 0f);
                move = -moveV_deltaTime;
            }
            else
            {
                rb.transform.Rotate(0f, angleTerm / FRAME_ROTATION, 0f);
                move = moveV_deltaTime;
            }
        }
        else
        {
            move = (Mathf.Abs(moveV) < Mathf.Abs(moveH) ? Mathf.Abs(moveH_deltaTime) : moveV_deltaTime);
        }

        if (run)
        {
            move *= 2f;
        }

        rb.transform.Translate(0f, 0f, move);

        return move;
    }

}


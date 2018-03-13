using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // 마우스 민감도
    public float sensitibity;
    
    
    private GameObject playerObject;
    private Vector3 playerPosition;
    private float mouseAngleX = 0.0f;
    private float mouseAngleY = 0.0f;

    

    // 마우스 수직 이동 허용 범위
    private const float MOUSE_VERTICAL_HEIGHT_MAX = 0.8f;
    private const float MOUSE_VERTICAL_HEIGHT_MIN = -0.6f;
    // 마우스 수직 이동에 대한 회전값
    private const float MOUSE_VERTICAL_ROTATION_SPEED = 70f;
    private const float MOUSE_VERTICAL_BASE_SENSITIVITY = 0.015f;

    // Use this for initialization
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");

        CheckMouseMove(moveX, moveY);
        
    }

    // 마우스의 움직임을 확인하여 카메라를 이동, 회전시키는 함수
    private void CheckMouseMove(float moveX, float moveY)
    {
        mouseAngleX += -moveX * sensitibity;
        mouseAngleX %= 360;
        float tempAngle = mouseAngleY + -moveY * sensitibity * MOUSE_VERTICAL_BASE_SENSITIVITY;
        if (tempAngle < MOUSE_VERTICAL_HEIGHT_MAX && tempAngle > MOUSE_VERTICAL_HEIGHT_MIN)
        {
            mouseAngleY = tempAngle;
            mouseAngleY %= 360;
        }

        float mouseMovementX = mouseAngleX * Mathf.PI / 180.0f;
        float mouseMovementY = mouseAngleY * Mathf.PI / 180.0f;

        // 마우스 좌우 이동과 상하 이동에 삼각함수를 적용, 자세한 설명은 카메라 명세서 참조
        transform.position = new Vector3(Mathf.Cos(mouseMovementX) + Mathf.Sqrt(3) * Mathf.Sin(mouseMovementX) + playerObject.transform.position.x,
            playerObject.transform.position.y + PlayerConstant.PLAYER_HEIGHT + Mathf.Sqrt(3) * Mathf.Tan(mouseAngleY), Mathf.Sin(mouseMovementX) - Mathf.Sqrt(3) * Mathf.Cos(mouseMovementX) + playerObject.transform.position.z);

        transform.rotation = Quaternion.Euler(mouseAngleY * MOUSE_VERTICAL_ROTATION_SPEED, -mouseAngleX, 0.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour {
    private static CrosshairController instance = null;


    private const float MAX_RANGE = 30f;
    private const float ROTATION_SPEED = 200f;
    private const float MIN_RANGE = 2.5f;
    private const float RANGE_ERROR = 0.1f;

    private PlayerManager playerManager;

    private Ray ray;
    // Floor를 제외한 감지된 물체
    private GameObject detectedObject;

    public static CrosshairController GetInstance()
    {
        if (instance == null) instance = FindObjectOfType<CrosshairController>();
        return instance;
    }

	void Start () {
        playerManager = PlayerManager.GetInstance();
        
	}
	
	void Update () {
        RotateCrosshair();
        MakeRayAndDetectObject();
    }

    // 크로스헤어를 거리에 따라서 회전시키는 메소드
    private void RotateCrosshair()
    {
        transform.Rotate(0f, 0f, (ROTATION_SPEED - transform.localPosition.z) * Time.deltaTime);
    }

    private void MakeRayAndDetectObject()
    {
        RikoController riko = playerManager.GetRiko();
        ray = new Ray(new Vector3(riko.transform.position.x, riko.transform.position.y + 1.0f, riko.transform.position.z),
            new Vector3(Mathf.Sin(transform.parent.eulerAngles.y * Mathf.PI / 180f), -Mathf.Tan(transform.parent.eulerAngles.x * Mathf.PI / 180f), Mathf.Cos(transform.parent.eulerAngles.y * Mathf.PI / 180f)));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            if (!hit.transform.CompareTag("Floor") && !hit.transform.CompareTag("Player"))
            {
                detectedObject = hit.transform.gameObject;
                float averageScale = (hit.transform.localScale.x + hit.transform.localScale.y + hit.transform.localScale.z) / 3f;
                // 플레이어로 부터 물체까지의 거리 + a
                float DistanceA = Vector3.Distance(riko.transform.position, hit.transform.position) - averageScale;
                // 플레이어로부터 크로스헤어까지의 거리
                float DistanceB = Vector3.Distance(riko.transform.position, transform.position);

                if (Mathf.Abs(DistanceA - DistanceB) < RANGE_ERROR) { }
                else if (DistanceA > DistanceB && transform.localPosition.z < MAX_RANGE)
                {
                    transform.Translate(0f, 0f, RANGE_ERROR);
                    //transform.Translate(0f, 0f, DistanceA - DistanceB);
                }
                else if (transform.localPosition.z >= MIN_RANGE)
                {
                    transform.Translate(0f, 0f, -(DistanceB - DistanceA));
                }
            }
            else if (hit.transform.CompareTag("Floor"))
            {
                detectedObject = null;
                float tan = Mathf.Tan((90f - transform.parent.transform.eulerAngles.x) * Mathf.PI / 180f);

                if (Mathf.Abs(tan - transform.localPosition.z) < RANGE_ERROR)
                { }
                else if (tan > transform.localPosition.z
                    && transform.localPosition.z < MAX_RANGE)
                {
                    transform.Translate(0f, 0f, RANGE_ERROR);
                }
                else if (transform.localPosition.z >= MIN_RANGE && tan < MAX_RANGE)
                {
                    transform.Translate(0f, 0f, -(transform.localPosition.z - tan));

                }
            }



        } // 감지되지 않은 경우
        else if (transform.localPosition.z < MAX_RANGE)
        {
            detectedObject = null;
            transform.Translate(0f, 0f, 0.3f);
        }
    }

    
    public GameObject GetAimedObject()
    {
        return detectedObject;
    }
}

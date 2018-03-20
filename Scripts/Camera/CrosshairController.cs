using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour {

    public const float MAX_RANGE = 30f;
    public const float ROTATION_SPEED = 200f;
    private Ray ray;
    private GameObject player;
    // Floor를 제외한 감지된 물체
    private GameObject detectedObject;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        RotateCrosshair();
        MakeRayAndDetectObject();


    }

    private void RotateCrosshair()
    {
        transform.Rotate(0f, 0f, (ROTATION_SPEED - transform.localPosition.z) * Time.deltaTime);
    }

    private void MakeRayAndDetectObject()
    {
        ray = new Ray(new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z),
            new Vector3(Mathf.Sin(transform.parent.eulerAngles.y * Mathf.PI / 180f), -Mathf.Tan(transform.parent.eulerAngles.x * Mathf.PI / 180f), Mathf.Cos(transform.parent.eulerAngles.y * Mathf.PI / 180f)));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            if (!hit.transform.CompareTag("Floor"))
            {
                detectedObject = hit.transform.gameObject;
                float averageScale = (hit.transform.localScale.x + hit.transform.localScale.y + hit.transform.localScale.z) / 3f;
                // 플레이어로 부터 물체까지의 거리 + a
                float DistanceA = Vector3.Distance(player.transform.position, hit.transform.position) - hit.transform.localScale.z;
                // 플레이어로부터 크로스헤어까지의 거리
                float DistanceB = Vector3.Distance(player.transform.position, transform.position);

                if (Mathf.Abs(DistanceA - DistanceB) < 0.3f) { }
                else if (DistanceA > DistanceB && transform.localPosition.z < MAX_RANGE)
                {
                    transform.Translate(0f, 0f, 0.3f);
                    //transform.Translate(0f, 0f, DistanceA - DistanceB);
                }
                else if (transform.localPosition.z >= 2.5f)
                {
                    transform.Translate(0f, 0f, -(DistanceB - DistanceA));
                }
            }
            else if (hit.transform.CompareTag("Floor"))
            {
                detectedObject = null;
                float tan = Mathf.Tan((90f - transform.parent.transform.eulerAngles.x) * Mathf.PI / 180f);

                if (Mathf.Abs(tan - transform.localPosition.z) < 0.3f)
                { }
                else if (tan > transform.localPosition.z
                    && transform.localPosition.z < MAX_RANGE)
                {
                    transform.Translate(0f, 0f, 0.3f);
                }
                else if (transform.localPosition.z >= 2.5f)
                {
                    if (transform.localPosition.z - (transform.localPosition.z - tan) <= MAX_RANGE)
                    {
                        transform.Translate(0f, 0f, -(transform.localPosition.z - tan));
                    }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChangePointController : MonoBehaviour
{
    public bool isSceneChanger;
    public Transform tempTransform;
    private bool isFirstTime = true;

    private void Awake()
    {
        if (!isSceneChanger)
            tempTransform = transform.GetChild(0);
    }

    private void Update()
    {
        // if(!isSceneChanger)
        // {
        //     if (timer % 2 == 0)
        //         CameraController.Instance.ResetFollowTarget();
        //     else
        //         CameraController.Instance.ChangeFollowTarget(tempTransform);
        // }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Ship"))
            return;

        if (isSceneChanger)
        {
            GetComponent<StateChange>().CrossScene();
            return;
        }
        if (isFirstTime)
        {
            tempTransform.position = other.transform.position;
            CameraController.Instance.ChangeFollowTarget(tempTransform);
            isFirstTime = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Ship"))
            return;

        CameraController.Instance.ResetFollowTarget();
        isFirstTime = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin_OtherCam;
    private float startShakeIntensity;
    private float startShakeIntensity_OtherCam;
    private float shakeTimer;
    private float shakeTimer_OtherCam;
    private float shakeTimerTotal;
    private float shakeTimerTotal_OtherCam;
    private bool usingCurve = false;
    private bool usingCurve_OtherCam = false;
    private Transform originalFollowTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        originalFollowTarget = cinemachineVirtualCamera.Follow;
    }

    public void ShakeCamera(float intensity, float time, bool isCurved)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        startShakeIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        usingCurve = isCurved;
    }
    public void ShakeCamera(CinemachineVirtualCamera camera, float intensity, float time, bool isCurved)
    {
        cinemachineBasicMultiChannelPerlin_OtherCam = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin_OtherCam.m_AmplitudeGain = intensity;
        startShakeIntensity_OtherCam = intensity;
        shakeTimer_OtherCam = time;
        shakeTimerTotal_OtherCam = time;
        usingCurve_OtherCam = isCurved;
    }



    public void ChangeFollowTarget(Transform target)
    {
        cinemachineVirtualCamera.Follow = target;
        cinemachineVirtualCamera.LookAt = target;
        cinemachineVirtualCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void ResetFollowTarget()
    {
        cinemachineVirtualCamera.Follow = originalFollowTarget;
        cinemachineVirtualCamera.LookAt = originalFollowTarget;
        cinemachineVirtualCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (!usingCurve)
            {
                if (shakeTimer <= 0f)
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
            else
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
                    Mathf.Lerp(startShakeIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
            }
        }
        if (shakeTimer_OtherCam > 0)
        {
            shakeTimer_OtherCam -= Time.deltaTime;
            if (!usingCurve_OtherCam)
            {
                if (shakeTimer_OtherCam <= 0f)
                    cinemachineBasicMultiChannelPerlin_OtherCam.m_AmplitudeGain = 0f;
            }
            else
            {
                cinemachineBasicMultiChannelPerlin_OtherCam.m_AmplitudeGain =
                    Mathf.Lerp(startShakeIntensity_OtherCam, 0f, (1 - (shakeTimer_OtherCam / shakeTimerTotal_OtherCam)));
            }
        }



    }

    public CinemachineVirtualCamera GetMainCamera()
    {
        return cinemachineVirtualCamera;
    }
}

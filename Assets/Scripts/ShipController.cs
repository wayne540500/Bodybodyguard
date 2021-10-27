using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public bool isOnControl = false;
    public bool isMapUnlocked;
    public bool addForce = false;
    public bool isBoostMode = false;
    public TextMeshProUGUI boostModeUIText;
    public ParticleSystem boostModeParticle;
    public Rigidbody2D shipRbody;
    public Transform ship;
    public Animator mapAnimator;
    public float boostersAvoidShacking = 1.5f;
    public BoosterData boosterData;

    private Vector2 moveInput = Vector2.zero;
    private PlayerInput playerInput;
    private InputAction m_ShipMove;
    private InputAction m_AddForce;
    private InputAction m_OpenMap;
    private bool isMapOpen = false;
    private float shipSpeed;
    private float boosterModeTimer;
    private int boosterPaticleIndex;
    private float[] boosterAngle_Temp;

    private void Awake()
    {
        shipRbody = ship.GetComponent<Rigidbody2D>();
        boosterAngle_Temp = new float[4];
    }

    private void Update()
    {
        if (isOnControl)
        {
            if (playerInput == null)
            {
                playerInput = GetComponentInChildren<PlayerInput>();
                m_ShipMove = playerInput.actions["Move"];
                m_AddForce = playerInput.actions["Attack"];
                m_OpenMap = playerInput.actions["Interactions"];
            }
            addForce = m_AddForce.ReadValue<float>() == 1 ? true : false;

            if (m_OpenMap.triggered && isMapUnlocked)
            {
                isMapOpen = mapAnimator.GetBool("isOpen");
                mapAnimator.SetBool("isOpen", isMapOpen ? false : true);
            }

            if (isBoostMode)
            {
                boosterPaticleIndex = 1;
                for (int i = 0; i < 4; i++)
                {
                    boosterData.boosters[i].GetChild(0).gameObject.SetActive(false);
                    boosterData.boosters[i].GetChild(1).gameObject.SetActive(true);
                }
                shipSpeed = boosterData.shipBoostSpeed;
                boostModeUIText.transform.parent.gameObject.SetActive(true);
                boostModeUIText.SetText((boosterData.shipBoostModeDuration - boosterModeTimer).ToString("0.00") + "s");
                if (boosterModeTimer < boosterData.shipBoostModeDuration)
                    boosterModeTimer += Time.deltaTime;
                else
                {
                    boosterPaticleIndex = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        boosterData.boosters[i].GetChild(1).gameObject.SetActive(false);
                        boosterData.boosters[i].GetChild(0).gameObject.SetActive(true);
                    }
                    boostModeUIText.transform.parent.gameObject.SetActive(false);
                    shipSpeed = boosterData.shipSpeed;
                    boosterModeTimer = 0f;
                    isBoostMode = false;
                }
            }
            else
                shipSpeed = boosterData.shipSpeed;

            ShipMove(m_ShipMove);

        }
        else
        {
            playerInput = null;
            m_ShipMove = null;
            m_AddForce = null;

            if (mapAnimator.GetBool("isOpen"))
            {
                mapAnimator.SetBool("isOpen", false);
            }

            for (int i = 0; i < 4; i++)
            {
                boosterData.boosters[i].GetChild(boosterPaticleIndex).GetComponent<ParticleSystem>().Stop();
                boosterAngle_Temp[i] = boosterData.boosters[i].eulerAngles.z;
            }
        }




    }

    public void SetBoostMode(float duration)
    {
        boosterModeTimer = 0f;
        boostModeParticle.Play();
        boosterData.shipBoostModeDuration = duration;
        isBoostMode = true;
    }

    public void ShipMove(InputAction context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (moveInput == Vector2.zero)
        {
            SoundManager.Instance.StopPlaySound(SoundManager.SoundType.booster);
            for (int i = 0; i < 4; i++)
            {
                boosterData.boosters[i].GetChild(boosterPaticleIndex).GetComponent<ParticleSystem>().Stop();
            }
            return;
        }

        var x = moveInput.x;
        var y = moveInput.y;

        if (x < 0)
        {
            if (y < 0)
            {
                BoostersControl(1);
            }
            else
            {
                BoostersControl(3);
            }
        }
        else
        {
            if (y < 0)
            {
                BoostersControl(0);
            }
            else
            {
                BoostersControl(2);
            }
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public void SetBoosterAngle()
    {
        if (boosterData.boosters.Length == 0)
            return;
        for (int i = 0; i < boosterData.boosters.Length; i++)
        {
            boosterData.boosters[i].eulerAngles = Vector3.forward * boosterAngle_Temp[i];
        }
    }

    private void BoostersControl(int theUsingOne)
    {
        for (int i = 0; i < boosterData.boosters.Length; i++)
        {
            if (i == theUsingOne)
                continue;

            boosterData.boosters[i].GetChild(boosterPaticleIndex).GetComponent<ParticleSystem>().Stop();
        }

        float inputAngleDelta = Mathf.Acos(Vector2.Dot(boosterData.boosters[theUsingOne].up, moveInput.normalized)) * Mathf.Rad2Deg;

        Debug.Log($"{theUsingOne} inputAngleDelta : " + inputAngleDelta);

        Vector2 boosterUp_temp = boosterData.boosters[theUsingOne].up;

        if (inputAngleDelta > boostersAvoidShacking)
        {
            var degree = boosterData.boostersRotateSpeed * Time.deltaTime;
            var RotatedX_N = (boosterUp_temp.x * Mathf.Cos(-degree * Mathf.Deg2Rad) - boosterUp_temp.y * Mathf.Sin(-degree * Mathf.Deg2Rad));
            var RotatedY_N = (boosterUp_temp.x * Mathf.Sin(-degree * Mathf.Deg2Rad) + boosterUp_temp.y * Mathf.Cos(-degree * Mathf.Deg2Rad));
            boosterUp_temp = new Vector2(RotatedX_N, RotatedY_N);

            var inputAngleDelta_temp = Mathf.Acos(Vector2.Dot(boosterUp_temp, moveInput.normalized)) * Mathf.Rad2Deg;

            if (inputAngleDelta_temp > inputAngleDelta)
            {
                boosterData.boosters[theUsingOne].Rotate(Vector3.forward * degree);
                Debug.Log("加角度");
            }
            else
            {
                boosterData.boosters[theUsingOne].Rotate(Vector3.forward * -degree);
                Debug.Log("減角度");
            }

        }
        else
        {
            boosterData.boosters[theUsingOne].up = moveInput.normalized;
            if (theUsingOne == 0)
                boosterData.boosters[theUsingOne].eulerAngles = Vector3.forward * boosterData.boosters[theUsingOne].eulerAngles.z;
            Debug.Log("貼其");
        }

        boosterAngle_Temp[theUsingOne] = boosterData.boosters[theUsingOne].eulerAngles.z;

        if (addForce)
        {
            SoundManager.Instance.PlaySoundLoop(SoundManager.SoundType.booster, false);
            boosterData.boosters[theUsingOne].GetChild(boosterPaticleIndex).GetComponent<ParticleSystem>().Play();
            shipRbody.velocity += ((Vector2)boosterData.boosters[theUsingOne].up * shipSpeed * Time.deltaTime);
        }
        else
        {
            SoundManager.Instance.StopPlaySound(SoundManager.SoundType.booster);
            boosterData.boosters[theUsingOne].GetChild(boosterPaticleIndex).GetComponent<ParticleSystem>().Stop();
        }
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigmapController : MonoBehaviour
{
    public GameObject bigmapCamera;
    public GameObject shipIcon;

    private void Start()
    {
        bigmapCamera.SetActive(false);
        shipIcon.SetActive(false);
    }
    public void OpenMap()
    {
        //Time.timeScale = 0f;
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.MapOpen, false);
        bigmapCamera.SetActive(true);
        shipIcon.SetActive(true);
    }

    public void CloseMap()
    {
        //Time.timeScale = 1f;
        SoundManager.Instance.PlaySoundOneShot(SoundManager.SoundType.MapClose, false);
        bigmapCamera.SetActive(false);
        shipIcon.SetActive(false);
    }

}

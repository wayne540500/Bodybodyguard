using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    public AudioSource[] sound;

    public void PlaySoundOneShot(int index){
        sound[index].PlayOneShot(sound[index].clip);
    }
}

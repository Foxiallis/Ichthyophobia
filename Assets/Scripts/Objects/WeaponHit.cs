using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    private AudioSource source;
    private bool startedPlaying;

    public void Initialize(AudioClip clip)
    {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        source.pitch = Random.Range(0.75f, 1f);
        source.Play();
        startedPlaying = true;
    }

    private void Update()
    {
        if (startedPlaying && !source.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

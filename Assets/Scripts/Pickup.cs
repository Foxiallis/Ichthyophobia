using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Light lamp;
    private bool interactable;
    public bool lit = true;
    private PlayerController player;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            interactable = false;
        }
    }

    private void Update()
    {
        if (lit && interactable && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<PlayerController>().Heal(); //can't use player variable because if anything enters the trigger it will break it
            source.pitch = Random.Range(0.75f, 1f);
            source.Play();
            StartCoroutine(Relight());
        }
    }

    IEnumerator Relight()
    {
        lamp.gameObject.SetActive(false);
        lit = false;
        yield return new WaitForSecondsRealtime(20);
        lamp.gameObject.SetActive(true);
        lit = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 6;
    private EnemyBehaviour enemy;
    private bool isSuperSaiyan;
    public AudioSource swingSound;
    public AudioSource hitSound;
    private ParticleSystem particleSystemLocal;
    private ParticleSystem particleSystemWorld;

    private void Start()
    {
        particleSystemLocal = GetComponent<ParticleSystem>();
        particleSystemWorld = GetComponentInParent<ParticleSystem>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        enemy = collision.gameObject.GetComponentInParent<EnemyBehaviour>();
        if (enemy != null)
        {
            hitSound.pitch = Random.Range(0.75f, 1f);
            hitSound.Play();
            enemy.TakeDamage(damage);
        }
    }

    public void SuperSaiyan() //because what else would it be
    {
        if (!isSuperSaiyan)
        {
            damage = 200;
            particleSystemLocal.Play();
            particleSystemWorld.Play();
        }
        else
        {
            damage = 6;
            particleSystemLocal.Stop();
            particleSystemWorld.Stop();
        }
        isSuperSaiyan = !isSuperSaiyan;
    }
}

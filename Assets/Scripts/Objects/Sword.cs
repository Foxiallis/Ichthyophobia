using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 6;
    private EnemyBehaviour enemy;
    private bool isSuperSaiyan;
    public AudioSource swingSource;
    public AudioClip hitSound;
    public GameObject weaponHitPrefab;
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
            ContactPoint cp = collision.GetContact(0);
            WeaponHit weaponHit = Instantiate(weaponHitPrefab, cp.point, Quaternion.FromToRotation(cp.otherCollider.transform.position, cp.point)).GetComponent<WeaponHit>();
            weaponHit.Initialize(hitSound);
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

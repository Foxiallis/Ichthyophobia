using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedProjectile : MonoBehaviour
{
    public int damage;
    public bool targetPlayer;
    public bool floating;
    public float speed;
    public Vector3 destination;
    public AudioSource startSound;
    public AudioClip hitSound;
    public GameObject weaponHitPrefab;
    private EnemyBehaviour enemy;
    private PlayerController player;
    private bool hit = false;

    private void Start()
    {
        StartCoroutine(SelfDestruct());
        startSound.pitch = Random.Range(0.75f, 1f);
        startSound.Play();
    }

    private void Update()
    {
        if (floating)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hit)
        {
            hit = true;
            if (!targetPlayer)
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
            else
            {
                player = collision.gameObject.GetComponentInParent<PlayerController>();
                if (player != null)
                {
                    if (collision.gameObject.GetComponentInParent<Sword>() == null) //make projectiles cuttable with a sword
                    {
                        ContactPoint cp = collision.GetContact(0);
                        WeaponHit weaponHit = Instantiate(weaponHitPrefab, cp.point, Quaternion.FromToRotation(cp.otherCollider.transform.position, cp.point)).GetComponent<WeaponHit>();
                        weaponHit.Initialize(hitSound);
                        player.TakeDamage(damage);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSecondsRealtime(3);
        Destroy(gameObject);
    }
}

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
    public GameObject projectileHitPrefab;
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
                    ProjectileHitAudio hitAudio = Instantiate(projectileHitPrefab, transform.position, transform.rotation).GetComponent<ProjectileHitAudio>();
                    hitAudio.Initialize(hitSound);
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
                        ProjectileHitAudio hitAudio = Instantiate(projectileHitPrefab, transform.position, transform.rotation).GetComponent<ProjectileHitAudio>();
                        hitAudio.Initialize(hitSound);
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

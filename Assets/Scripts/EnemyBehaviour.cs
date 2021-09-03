using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BehaviourType { Melee, Ranged, Boss }
public class EnemyBehaviour : MonoBehaviour
{
    public BehaviourType type;
    public int maxHP;
    public int currentHP;
    public int minAttackDistance;
    public int lockedAttackDistance;
    public float attackCooldown;
    public float attackDelay;
    public int damage;
    public float invincibilityFrames = 0.2f;
    public bool invincible;
    public Vector3 projectilePosition;
    public GameObject projectilePrefab;
    public GameObject spawnObjectPrefab;
    public GameObject deadPrefab;

    private PlayerController player;
    private NavMeshAgent agent;
    private Animator animator;
    private GameManager manager;
    private AudioSource attackSound;

    private bool canAttack = true;
    private bool canMove = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameManager.player;
        currentHP = maxHP;
        animator = GetComponentInChildren<Animator>();
        manager = FindObjectOfType<GameManager>();
        attackSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (canMove)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (canAttack) //so that enemies can either attack or move
            {
                agent.SetDestination(player.transform.position);
            }

            if (distance < lockedAttackDistance) //don't shoot if the player is too close
            {
                if (type == BehaviourType.Ranged)
                {
                    type = BehaviourType.Melee;
                }
                else if (type == BehaviourType.Boss && canAttack)
                {
                    StartCoroutine(SpecialAttack()); //special boss attack
                    canAttack = false;
                }
            }
            else if (distance > lockedAttackDistance && projectilePrefab != null && type == BehaviourType.Melee) //switch back to ranged if player went far enough
            {
                type = BehaviourType.Ranged;
            }

            if (canAttack && distance < minAttackDistance)
            {
                if (type != BehaviourType.Melee)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + projectilePosition, player.transform.position - (transform.position + projectilePosition), out hit, Mathf.Infinity))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            StartCoroutine(Attack());
                        }
                    }
                }
                else
                {
                    StartCoroutine(Attack());
                }
            }
        }
    }

    public void TakeDamage(int d)
    {
        if (!invincible && canMove)
        {
            currentHP -= d;
            if (currentHP <= 0)
            {
                Instantiate(deadPrefab, transform.position, transform.rotation);
                manager.HandleDead(gameObject);
                Destroy(gameObject);
            }
            StartCoroutine(InvincibilityFrames());
        }
    }

    public void Pause()
    {
        Debug.Log("Pasuing");
        canMove = false;
        agent.isStopped = true;
        animator.enabled = false;
    }

    public void Resume()
    {
        canMove = true;
        agent.isStopped = false;
        animator.enabled = true;
    }

    private void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < minAttackDistance && canMove)
        {
            player.TakeDamage(damage);
        }
    }

    IEnumerator InvincibilityFrames()
    {
        invincible = true;
        yield return new WaitForSecondsRealtime(invincibilityFrames);
        invincible = false;
    }

    IEnumerator SpecialAttack()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");
        attackSound.pitch = Random.Range(0.75f, 1f);
        attackSound.Play();
        GameObject summon = Instantiate(spawnObjectPrefab, transform);
        summon.transform.localPosition = projectilePosition;
        summon.transform.SetParent(null);
        manager.spawnedEnemies.Add(summon);
        agent.isStopped = false;
        animator.SetBool("isWalking", true);
        yield return new WaitForSecondsRealtime(attackCooldown * 4);
        canAttack = true;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");
        attackSound.pitch = Random.Range(0.75f, 1f);
        attackSound.Play();
        if (type == BehaviourType.Melee)
        {
            Invoke("DealDamage", attackDelay);
        }
        else
        {
            GameObject projectile = Instantiate(projectilePrefab, transform);
            projectile.transform.localPosition = projectilePosition;
            projectile.transform.SetParent(null);
            projectile.transform.LookAt(player.transform);
        }
        yield return new WaitForSecondsRealtime(attackCooldown);
        canAttack = true;
        agent.isStopped = false;
        animator.SetBool("isWalking", true);
    }
}

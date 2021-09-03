using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerController : MonoBehaviour
{
    public float mouseSensitivity;
    public float camMinRotation;
    public float camMaxRotation;
    public float moveSpeed;
    public float jumpHeight;
    public float dashLength;
    public float gravityAmount;
    public float jumpCooldown;
    public float dashCooldown;
    public float doubleJumpMargin;
    public GameObject daggerPrefab;
    public GameObject superDaggerPrefab;
    public Vector3 spawnedDaggerPosition;
    public Vector3 spawnedDaggerRotation;
    public float daggerPower;
    public int maxHP;
    public int currentHP;
    public Sword sword;
    public UIManager uiManager;
    public PostProcessingProfile normalProfile;
    public PostProcessingProfile deadProfile;
    public int respawnTime;
    public bool respawning;
    public AudioSource soundtrackPlayer;
    public List<AudioClip> soundtrack;

    private Camera cam;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Animator animator;
    private AudioSource dashSound;
    private Tutorial tutorial;
    private GameManager manager;

    private bool alive = true;
    private bool canJump = true;
    private bool canDash = true;
    private bool isSuperSaiyan;
    private bool isInvincible;

    private float Minusize(float angle) //because transform.localEulerAngles doesn't return negative values
    {
        if (angle > 128)
        {
            return angle - 360;
        }
        return angle; 
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        dashSound = GetComponent<AudioSource>();
        tutorial = uiManager.GetComponent<Tutorial>();
        manager = uiManager.GetComponent<GameManager>();
        cam = GetComponentInChildren<Camera>(); //using Camera.main is cringe and unoptimized
        Cursor.lockState = CursorLockMode.Locked;
        currentHP = maxHP;
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.5f) * 100;
    }

    private void OnTriggerEnter(Collider other)
    {
        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup != null && pickup.lit)
        {
            uiManager.SetTorchberry(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Pickup>() != null)
        {
            uiManager.SetTorchberry(false);
        }
    }

    void Update()
    {
        if (alive)
        {
            MoveCamera();
            MovePlayer();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //Trigger an animation, the animator then handles the rest
                animator.SetTrigger("Throw");
            }
            #region Cheat Sheet
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                //Damage cheat
                if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.E))
                {
                    isSuperSaiyan = !isSuperSaiyan;
                    sword.SuperSaiyan();
                    if (isSuperSaiyan)
                    {
                        uiManager.Alert("Activated damage cheat", 2);
                    }
                    else
                    {
                        uiManager.Alert("Dectivated damage cheat", 2);
                    }
                    if (!tutorial.damageCheatDone && !tutorial.gameStarted && !tutorial.tutorialPassed)
                    {
                        tutorial.damageCheatDone = true;
                        tutorial.UpdateText();
                    }
                }
                //Health cheat
                if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.R))
                {
                    isInvincible = !isInvincible;
                    if (isInvincible)
                    {
                        uiManager.Alert("Activated health cheat", 2);
                    }
                    else
                    {
                        uiManager.Alert("Dectivated health cheat", 2);
                    }
                    if (!tutorial.healthCheatDone && !tutorial.gameStarted && !tutorial.tutorialPassed)
                    {
                        tutorial.healthCheatDone = true;
                        tutorial.UpdateText();
                    }

                }
                //Instant suicide
                if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetKeyDown(KeyCode.F))
                {
                    TakeDamage(1000, true);
                    uiManager.Alert("Commited suicide", 2);
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                //Damage cheat
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.E))
                {
                    isSuperSaiyan = !isSuperSaiyan;
                    sword.SuperSaiyan();
                    if (isSuperSaiyan)
                    {
                        uiManager.Alert("Activated damage cheat", 2);
                    }
                    else
                    {
                        uiManager.Alert("Dectivated damage cheat", 2);
                    }
                    if (!tutorial.damageCheatDone && !tutorial.gameStarted && !tutorial.tutorialPassed)
                    {
                        tutorial.damageCheatDone = true;
                        tutorial.UpdateText();
                    }
                }
                //Health cheat
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
                {
                    isInvincible = !isInvincible;
                    if (isInvincible)
                    {
                        uiManager.Alert("Activated health cheat", 2);
                    }
                    else
                    {
                        uiManager.Alert("Dectivated health cheat", 2);
                    }
                    if (!tutorial.healthCheatDone && !tutorial.gameStarted && !tutorial.tutorialPassed)
                    {
                        tutorial.healthCheatDone = true;
                        tutorial.UpdateText();
                    }
                }
                //Instant suicide
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F))
                {
                    TakeDamage(1000, true);
                    uiManager.Alert("Commited suicide", 2);
                }
            }
            #endregion
        }
    }

    void MoveCamera()
    {
        float vertical = Input.GetAxis("Mouse Y");
        float horizontal = Input.GetAxis("Mouse X");
        if (horizontal != 0)
        {
            transform.Rotate(new Vector3(0, horizontal * mouseSensitivity * Time.deltaTime, 0));
        }
        if ((vertical > 0 && Minusize(cam.transform.localEulerAngles.x) > camMinRotation) || (vertical < 0 && Minusize(cam.transform.localEulerAngles.x) < camMaxRotation))
            //locks so player can only turn his head so much
        {
            cam.transform.Rotate(new Vector3(-vertical * mouseSensitivity * Time.deltaTime, 0, 0));
        }
    }


    void MovePlayer()
    {
        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        controller.Move(transform.TransformDirection(forward, 0, -strafe) * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            //Perform a higher jump in the air
            if (playerVelocity.y < gravityAmount * doubleJumpMargin) {
                playerVelocity.y += jumpHeight * 2;
            }
            else
            {
                playerVelocity.y += jumpHeight;
            }
            StartCoroutine(CooldownJump());
        }

        playerVelocity.y += gravityAmount * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse1) && canDash)
        {
            controller.Move(transform.TransformDirection(dashLength, 0, 0));
            dashSound.pitch = Random.Range(0.75f, 1f);
            dashSound.Play();
            StartCoroutine(CooldownDash());
        }
        if (transform.position.x > 50 || transform.position.x < -50 || transform.position.z > 50 || transform.position.z < -50)
        {
            TakeDamage(1000, true);
        }
    }

    void Attack()
    {
        bool up = Random.value > 0.5f;
        bool left = Random.value > 0.5f;

        animator.SetBool("Up", up);
        animator.SetBool("Left", left);
        animator.SetTrigger("Swing");
        sword.swingSource.pitch = Random.Range(0.75f, 1f);
        sword.swingSource.Play();
    }

    void SpawnDagger()
    {
        //triggered in the animation
        GameObject dagger = isSuperSaiyan ? Instantiate(superDaggerPrefab, cam.transform) : Instantiate(daggerPrefab, cam.transform);
        dagger.transform.localPosition = spawnedDaggerPosition;
        dagger.transform.localEulerAngles = spawnedDaggerRotation;
        dagger.transform.SetParent(null);
        Rigidbody daggerRB = dagger.GetComponent<Rigidbody>();
        daggerRB.useGravity = true;
        daggerRB.isKinematic = false;
        daggerRB.AddForce(dagger.transform.TransformDirection(0, daggerPower, 0), ForceMode.Impulse);
    }

    public void TakeDamage(int d, bool bypassInvicibility = false)
    {
        if ((!isInvincible || bypassInvicibility) && alive)
        {
            currentHP = Mathf.Clamp(currentHP - d, 0, maxHP);
            if (d > 0)
            {
                uiManager.TakeDamage(d);
            }
            uiManager.UpdateHealthbar(currentHP, maxHP);
            if (currentHP == 0 && !isInvincible)
            {
                StopAllCoroutines();
                StartCoroutine(Respawn());
            }
        }
    }

    public void Heal()
    {
        TakeDamage(-20);
        uiManager.SetTorchberry(false);
        if (!tutorial.torchberryDone && !tutorial.gameStarted && !tutorial.tutorialPassed)
        {
            tutorial.torchberryDone = true;
            tutorial.UpdateText();
        }
    }

    public void Pause()
    {
        alive = false;
        Cursor.lockState = CursorLockMode.None;
        cam.GetComponent<PostProcessingBehaviour>().profile = deadProfile;
        animator.enabled = false;
        controller.enabled = false;
        soundtrackPlayer.Pause();
    }

    public void Resume()
    {
        controller.enabled = true;
        animator.enabled = true;
        cam.GetComponent<PostProcessingBehaviour>().profile = normalProfile;
        Cursor.lockState = CursorLockMode.Locked;
        alive = true;
        soundtrackPlayer.UnPause();
    }

    public void PlayMusic() //triggered in GameManager on the first wave
    {
        soundtrackPlayer.clip = soundtrack[Random.Range(0, soundtrack.Count)]; //play a random soundtrack each time
        soundtrackPlayer.Play();
    }

    public void Reset()
    {
        transform.position = new Vector3(0, 2.5f, 0);
        currentHP = maxHP;
        canDash = true;
        canJump = true;
        uiManager.SetDash(true);
        uiManager.SetJump(true);
        uiManager.UpdateHealthbar(currentHP, maxHP);
        uiManager.SetTorchberry(false);
        soundtrackPlayer.Stop();
        manager.Reset();
        Resume();
    }

    IEnumerator Respawn()
    {
        respawning = true;
        Pause();
        for (int i = respawnTime; i > 0; i--)
        {
            uiManager.Alert("Respawning in " + i + "...", 1);
            yield return new WaitForSecondsRealtime(1);
        }
        Reset();
        respawning = false;
    }

    IEnumerator CooldownJump()
    {
        canJump = false;
        uiManager.SetJump(false);
        yield return new WaitForSecondsRealtime(jumpCooldown);
        while (!alive) { yield return new WaitForSecondsRealtime(jumpCooldown); } //so cooldown doesn't wear off during pause
        canJump = true;
        uiManager.SetJump(true);
    }

    IEnumerator CooldownDash()
    {
        canDash = false;
        uiManager.SetDash(false);
        yield return new WaitForSecondsRealtime(dashCooldown);
        while (!alive) { yield return new WaitForSecondsRealtime(dashCooldown); } //so cooldown doesn't wear off during pause
        canDash = true;
        uiManager.SetDash(true);
    }
}

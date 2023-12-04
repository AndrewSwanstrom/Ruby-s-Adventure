using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;

    public int score = 0;
    public TMP_Text scoreText;

    public int winState = 0; //0 is currently playing, 1 is winning, and 2 is losing
    public TMP_Text winText;
    
    public GameObject projectilePrefab;
    public GameObject HealthPrefab;
    public GameObject DamagePrefab;

    public float DamageTime = 0.35f;
    public float HealthTime = 0.35f;
    bool IsReceivingDamage;
    bool IsReceivingHealth;
    float DamageTimer;
    float HealthTimer;
    
    public AudioClip throwSound;
    public AudioClip hitSound;

    public AudioClip winSound;
    public AudioClip lossSound;
    bool audioPlayed = false;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    //new dash variables (made by Clay (with help from Andrew))
    public float timeDashing = .05f;
    bool isDashing;
    float dashingTimer;
    bool canDash = true;
    public float timeCanDashing = 0.5f;
    float canDashingTimer;
    public AudioClip dashSound;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        } else {
            horizontal = 0;
            vertical = 0;
            winState = 2; //the player lost, so winState is set to 2
        }
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (IsReceivingDamage)
        {
            DamageTimer -= Time.deltaTime;
            if (DamageTimer < 0){
                IsReceivingDamage = false;
                Destroy(DamagePrefab);
            }
        }

        if (IsReceivingHealth)
        {
            HealthTimer -= Time.deltaTime;
            if (HealthTimer < 0){
                IsReceivingHealth = false;
                Destroy(HealthPrefab);
            }
        }

        //NEW CODE MADE FOR FINAL PROJECT ASSIGNMENT: dash ability (made by Clay (with help from Andrew))
        if (isDashing)
        {
            dashingTimer -= Time.deltaTime;
            if (dashingTimer < 0){
                isDashing = false;
                speed = 3.0f;
            }
        }

        if (!canDash)
        {
            print ("Reached the statement");
            canDashingTimer -= Time.deltaTime;
            if (canDashingTimer < 0){
                canDash = true;
                print ("Can Dash");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentHealth > 0 && canDash){
            speed = 50.0f;
            isDashing = true;
            canDash = false;
            dashingTimer = timeDashing;
            canDashingTimer = timeCanDashing;
            PlaySound(dashSound);
        }
        //END OF NEW DASH ABILITY CODE
        
        if(Input.GetKeyDown(KeyCode.C) && currentHealth > 0)
        {
            Launch();
        }
        
        if (Input.GetKeyDown(KeyCode.X) && currentHealth > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        ChangeWin();

        if (Input.GetKeyDown(KeyCode.R) && winState == 2){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position;
        position.x = 0;
        position.y = 0;

        position = rigidbody2d.position;

        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (currentHealth > 0)
        {
            if (amount < 0)
            {
                if (isInvincible)
                    return;
                
                isInvincible = true;
                invincibleTimer = timeInvincible;
                
                PlaySound(hitSound);
            }
            
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            
            UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

            if (amount > 0){
                GameObject DamageEffect = Instantiate(DamagePrefab, rigidbody2d.position + Vector2.up * 1f, Quaternion.identity);
                IsReceivingDamage = true;
                DamageTimer = DamageTime;
            } else if (amount < 0){
                GameObject HealthEffect = Instantiate(HealthPrefab, rigidbody2d.position + Vector2.up * 1f, Quaternion.identity);
                IsReceivingHealth = true;
                HealthTimer = HealthTime;
            }
        }
    }

    public void ChangeScore(int scoreAmount)
    {   
        score += scoreAmount;
        scoreText.text = "Fixed Robots: " + score.ToString();
        ScoreScript.instance.SetText(scoreText);

        if (score == 5) //change this number to how many robots are in the scene
        {
            winState = 1;
        }
    }

    public void ChangeWin()
    {
        if (winState == 1)
        {
            winText.text = "You Win! Game Created by Group 16";
            WinTextScript.instance.SetText(winText);
            if (!audioPlayed)
            {
                PlaySound(winSound); //audio added by Andrew
                audioPlayed = true;
            }
        } else if (winState == 2)
        {
            winText.text = "Game Over! Press R to Restart";
            WinTextScript.instance.SetText(winText);
            if (!audioPlayed)
            {
                PlaySound(lossSound); //audio added by Andrew
                audioPlayed = true;
            }
        } else if (winState == 0)
        {
            winText.text = "";
            WinTextScript.instance.SetText(winText);
        }
    }
    
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
    }
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
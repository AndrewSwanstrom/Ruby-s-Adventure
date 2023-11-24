using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;

    public int score = 0;
    public TMP_Text scoreText;

    public int winState = 0; //0 is currently playing, 1 is winning, and 2 is losing
    public TMP_Text winText;
    
    public GameObject projectilePrefab;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
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
    }
    
    void FixedUpdate()
    {
        Vector2 position;
        position.x = 0;
        position. y = 0;

        if (Input.GetKeyDown(KeyCode.R) && winState == 2) //resetting the game when pressing R
        {
            currentHealth = maxHealth;
            ChangeHealth(currentHealth);
            score = 0;
            winState = 0;
            position.x = 0;
            position. y = 0;
        } else {
            position = rigidbody2d.position;
        }
        
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
        }
    }

    public void ChangeScore(int scoreAmount)
    {   
        score += scoreAmount;
        scoreText.text = "Fixed Robots: " + score.ToString();
        ScoreScript.instance.SetText(scoreText);

        if (score == 2) //change this number to how many robots are in the scene
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
        } else if (winState == 2)
        {
            winText.text = "Game Over! Press R to Restart";
            WinTextScript.instance.SetText(winText);
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
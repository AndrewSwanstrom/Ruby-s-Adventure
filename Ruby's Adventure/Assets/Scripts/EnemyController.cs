using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    public ParticleSystem smokeEffect;
    
    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;
    
    //new variables for final project turret helper (Andrew) (putting the code in the enemy script because the turret also needs to be fixed)
    public bool isTurret;
    float turretShootTime = 2.0f;
    float turretShootTimer;
    bool canTurretShoot = true;
    public AudioClip turretSound;
    AudioSource audioSource;
    Vector2 lookDirection = new Vector2(0,1);
    public GameObject projectilePrefab;
    public RubyController ruby;
    
    Animator animator;

    private RubyController rubyController;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>(); //storing the rubyController in a variable

            print ("Found the RubyController Script!");
        }

        if (rubyController == null)
        {
            print ("Cannot find GameController Script!");
        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won't be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won't be executed.
        if(!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            if (!isTurret)
            {
                animator.SetFloat("Move X", 0);
                animator.SetFloat("Move Y", direction);
            }
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            if (!isTurret)
            {
                animator.SetFloat("Move X", direction);
                animator.SetFloat("Move Y", 0);
            }
        }
        
        rigidbody2D.MovePosition(position);

        //NEW TURRET CODE FOR FINAL PROJECT (Andrew)
        if (isTurret)
        {
            if (!canTurretShoot)
            {
                turretShootTimer -= Time.deltaTime;
                if (turretShootTimer < 0)
                    canTurretShoot = true;
            } else
            {
                TurretShoot();
            }

            return;
        }
        //END OF NEW TURRET CODE FOR FINAL PROJECT (Andrew)
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
    
    //NEW TURRET CODE FOR FINAL PROJECT (Andrew)
    public void TurretShoot()
    {
        canTurretShoot = false;
        turretShootTimer = turretShootTime;

        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2D.position + Vector2.up * 1.0f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        
        ruby.PlaySound(turretSound);
    }
    //END OF NEW TURRET CODE FOR FINAL PROJECT (Andrew)

    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        
        smokeEffect.Stop();

        if (rubyController != null)
        {
            rubyController.ChangeScore(1); //incrementing the score by 1
        }
    }
}
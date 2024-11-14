using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActualPlayerController : MonoBehaviour
{
    public float speed = 3.0f;

    public float timeInvincable = 2;

    public int maxHealth = 5;

    public GameObject projectilePrefab;
    public int health { get { return currentHealth; } }
    int currentHealth;

    bool isInvincible;
    float invincableTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    // animation
    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

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
            invincableTimer -= Time.deltaTime;
            if(invincableTimer < 0)
            {
                isInvincible = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        //

        if(Input.GetKey(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 2.0f, LayerMask.GetMask("NPC"));
            if(hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if(character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    //moving

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime; ;

        rigidbody2d.MovePosition(position);
    }
    
    //hp

    public void ChangeHealth(int amount)
    {
        if(amount < 0)
        {
            animator.SetTrigger("Hit");
            if(isInvincible)
            {
                return;
            }
            isInvincible = true;
            invincableTimer = timeInvincable;
            PlaySound(hitSound);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth/(float)maxHealth);
    }

    //gear

    void Launch()
    {
     GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
     
     Projectile projectile = projectileObject.GetComponent<Projectile>();
     projectile.Launch(lookDirection, 300);
    
     
     animator.SetTrigger("Launch");
        PlaySound(throwSound);
    }

    // idk

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public int life = 2;

    public float speed;

    public float jumpForce;
    public float cooldownTime = 1f;

    public GameObject bulletPrefab;

    public GameObject lifesPanel;
    public Vector2 respawnpoint;

    private Rigidbody2D rigidBody2D;

    public float horizontal;

    private bool isGrounded;
    private bool isInCooldown;

    private Animator animator;

    private Vector2 initialPosition;

    private float lastShoot;
    //private GameObject warpA;
    private GameObject destinyWarp;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
       // respawnpoint = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            // if(isGrounded){
            //respawnPoint=transform.position;
            //}
            horizontal = Input.GetAxis("Horizontal") * speed;
            if (horizontal < 0.0f)
            {
                //Debug.Log("-");
                transform.localScale = new Vector2(-20.0f, 20.0f);
            }
            else if (horizontal > 0.0f)
            {
                //Debug.Log("-");
                transform.localScale = new Vector2(20.0f, 20.0f);
            }
            animator.SetBool("isRunning", horizontal != 0.0f);

            //Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.blue);


            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }

            if (!isGrounded)
            {
                animator.SetBool("isJumping", true);
            }
            else
            {
                animator.SetBool("isJumping", false);
            }

            if (Input.GetKeyDown(KeyCode.E) && Time.time > lastShoot + 1f)
            {
                Shoot();
                lastShoot = Time.time;
            }
            if(Input.GetKeyDown(KeyCode.S) && destinyWarp){
                transform.position = destinyWarp.transform.position;
            }
            DeathOnFall();
        }
    }

    public void Death()
    {
        transform.position = initialPosition;
        //respawnpoint = initialPosition;
        if (life <= 0)
        {
            life = 2;
            for (int i = 0; i < lifesPanel.transform.childCount; i++)
            {
                lifesPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void Hit(float knockback, GameObject enemy)
    {
        if (!isInCooldown)
        {
            StartCoroutine(cooldown());
            if (life > 0)
            {
                lifesPanel.transform.GetChild(life).gameObject.SetActive(false);
                life -= 1;
                if (enemy)
                {
                    Vector2 difference = (transform.position - enemy.transform.position).normalized;
                    Vector2 force = difference * knockback;
                    float knockbackDirection = force.x >= 0 ? 1 : -1;
                    rigidBody2D.velocity = new Vector2(knockbackDirection * knockback,knockback/2);
                   //Vector2 difference = (transform.position - enemy.transform.position);
                    //float knockbackDirection = difference.x >= 0 ? 1 : -1;
                    //rigidBody2D.velocity = new Vector2(knockbackDirection * knockback, knockback);
                    //rigidBody2D.AddForce(force * knockback, ForceMode2D.Impulse);
                    //GetComponent<Rigidbody2D>().AddForce(force * knockback + rigidBody2D.velocity, ForceMode2D.Impulse);
                }
            }
            else
            {
                Death();
            }
        }
    }


    IEnumerator cooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isInCooldown = false;
    }
    private void Jump()
    {
        rigidBody2D.AddForce(Vector2.up * jumpForce);
    }

    private void DeathOnFall()
    {
        if (transform.position.y < -10f)
        {
            transform.position = respawnpoint;
            Hit(0, null);
        }
    }

    public void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x > 0)
            direction = Vector3.right;
        else
            direction = Vector3.left;
        GameObject bullet =
            Instantiate(bulletPrefab,
            transform.position + direction * 0.8f,
            Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }

    private void FixedUpdate()
    {
        if (!isInCooldown)
        {
            rigidBody2D.velocity = new Vector2(horizontal, rigidBody2D.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Tilemap") isGrounded = true;
        if (collider.name == "PointA" ||
        collider.name == "PointB")
        {
             GameObject warp = collider.transform.parent.gameObject;
        if (collider.name == "PointA")
        {
            destinyWarp = warp.transform.Find("PointB").gameObject;
        }
        else
        {
            destinyWarp = warp.transform.Find("PointA").gameObject;
        }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
                    GameObject warp = collider.transform.parent.gameObject;

        if (collider.name == "Tilemap") isGrounded = false;
        if (collider.name == "PointA" ||
        collider.name == "PointB")
        {
            
            destinyWarp = null;

        }
    }


}

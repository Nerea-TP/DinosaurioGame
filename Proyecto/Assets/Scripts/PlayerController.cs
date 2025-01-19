using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    [Header("VALORES CONFIGURABLES")]
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] GameObject checkPoint;
    [Header("SONIDOS")]
    [SerializeField] AudioClip sfxJump;
    [SerializeField] AudioClip sfxHit;
    [SerializeField] AudioClip sfxDeath;
    [SerializeField] AudioClip sfxDestroy;

    GameManager gm;

    Rigidbody2D rb;
    AudioSource sfx;
    Collider2D col;
    Animator anim;
    SpriteRenderer sprite;
    Color original;

    float moveX;

    bool jump;
    bool isButtonPressed;
    public static bool isClimbing = false;
    bool hitBack = false;

    Vector3 posIni;
    private bool isMobile = false;

    public Vector3 GetInitialPosition() => posIni;
    private void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        gm = GameManager.Instance;
        posIni = transform.position;
        InitializeComponents();

#if UNITY_ANDROID
        isMobile = true;
#else
        isMobile = false;
#endif
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        original = sprite.color;
        gm.ReadPreferences();
    }
    public void AdjustVolume(float volume)
    {
        sfx.volume = volume;
    }


    void Update()
    {

        if (!gm.isGameOver() && !gm.isFading())
        {
            if (!isMobile)
            {
                // Lógica para ordenador
                moveX = Input.GetAxisRaw("Horizontal");
                if (!jump && Input.GetButtonDown("Jump"))
                {
                    jump = true;
                }

                if (isClimbing)
                {
                    float inputVertical = Input.GetAxis("Vertical");
                    rb.linearVelocity = new Vector2(0, inputVertical * 3f);
                    if (inputVertical != 0)
                    {
                        anim.Play("ClimLadder");
                    }
                    else
                    {
                        anim.Play("StillLadder");
                    }
                }
            }
            else if (isMobile)
            {
                if (!jump && isButtonPressed)
                {
                    jump = true;
                }
            }
        }

    }

    public void OnJumpButtonPressed()
    {
        jump = true; // Establece el flag para saltar
    }
    public void OnLeftButtonDown()
    {
        moveX = -1 * speed;// Mueve a la izquierda
    }

    public void OnLeftButtonUp()
    {
        moveX = 0; // Detiene el movimiento a la izquierda
    }

    public void OnRightButtonDown()
    {
        moveX = 1 * speed; // Mueve a la derecha
    }

    public void OnRightButtonUp()
    {
        moveX = 0; // Detiene el movimiento a la derecha
    }

    public void OnUpButtonDown()
    {
        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, 3f); // Sube
            anim.Play("ClimLadder");
        }
    }

    public void OnUpButtonUp()
    {
        if (isClimbing)
        {
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento vertical
            anim.Play("StillLadder");
        }
    }

    public void OnDownButtonDown()
    {
        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, -3f); // Baja
            anim.Play("ClimLadder");
        }
    }

    public void OnDownButtonUp()
    {
        if (isClimbing)
        {
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento vertical
            anim.Play("StillLadder");
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isButtonPressed = true;
        switch (eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "LeftButton":
                OnLeftButtonDown();
                break;
            case "RightButton":
                OnRightButtonDown();
                break;
            case "UpButton":
                OnUpButtonDown();
                break;
            case "DownButton":
                OnDownButtonDown();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isButtonPressed = false;
        moveX = 0;
        switch (eventData.pointerCurrentRaycast.gameObject.name)
        {
            case "LeftButton":
                OnLeftButtonUp();
                break;
            case "RightButton":
                OnRightButtonUp();
                break;
            case "UpButton":
                OnUpButtonUp();
                break;
            case "DownButton":
                OnDownButtonUp();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!gm.isGameOver() && !gm.isFading())// Verifica si el juego no está en modo "game over" y el fondo no se está desvaneciendo
        {
            Walk();
            Flip();
            Jump();
        }
    }

    private void Jump()
    {
        if (!jump) { return; }
        jump = false;
        if (!col.IsTouchingLayers(LayerMask.GetMask("Terreno", "Plataformas"))) { return; }
        rb.linearVelocity = new Vector2(0, jumpSpeed);
        anim.SetTrigger("isJumping");
        sfx.clip = sfxJump;
        sfx.Play();
    }

    public void Flip()
    {
        float vx = rb.linearVelocity.x;
        if (Mathf.Abs(vx) > Mathf.Epsilon)
        {
            transform.localScale = new Vector2(Mathf.Sign(vx), 1);
        }
    }

    void Walk()
    {
        Vector2 vel;
        float adjustedSpeed = 0;
        if (!hitBack)
        {
            if (isMobile)
            {
                // En Android, no multiplicar por Time.fixedDeltaTime
                adjustedSpeed = 1f;
                vel = new Vector2(moveX * adjustedSpeed * Time.fixedDeltaTime, rb.linearVelocity.y);
            }
            else
            {
                // En el ordenador, mantener la multiplicación por Time.fixedDeltaTime
                vel = new Vector2(moveX * speed * Time.fixedDeltaTime, rb.linearVelocity.y);
            }
            rb.linearVelocity = vel;

            //miramos si la velocidad es mayor que 0
            anim.SetBool("isWalking", Mathf.Abs(rb.linearVelocity.x) > Mathf.Epsilon);

        }

    }

    [Obsolete]
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "MobilePlatform")
        {
            //cuando el jugador está en la plataforma movil pasa a ser parte de esta plataforma, por lo 
            //que cuando se mueve la plataforma el jugador se mueve con ella.
            transform.parent = other.transform;
        }
        if (other.gameObject.tag == "Enemy")
        {
            EnemyDamage(other.transform.position.x);
        }
        if (!hitBack && other.gameObject.tag == "Back")
        {
            rb.linearVelocity = Vector2.zero;
            //para que el jugador rebote encima el enemigo
            rb.AddForce(new Vector2(0.0f, 0.9f), ForceMode2D.Impulse);
            Destroy(other.gameObject);
            sfx.clip = sfxDestroy;
            sfx.Play();
        }
        if (other.gameObject.tag == "NextLevel")
        {
            gm.LoadScene(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "MobilePlatform")
        {   //para que cuando el jugador salga de plataforma deje de depender de ningun objeto
            transform.parent = null;
        }
    }

    [Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Spikes")
        {

            sfx.clip = sfxDeath;
            sfx.Play();
            gm.LoseLife();
            Dead();
        }
        if (other.gameObject.tag == "Fall")
        {
            sfx.clip = sfxDeath;
            sfx.Play();
            gm.LoseLife();
            if (!gm.isGameOver())
            {
                if (!gm.isChecking())
                {
                    StartCoroutine(RespawnAfterAnimation());
                }
                else if (gm.isChecking())
                {
                    StartCoroutine(RespawnPlayer());
                }
            }
        }
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = true;
            rb.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            isClimbing = false;
            rb.gravityScale = 2;
            anim.Play("Quieto");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    [Obsolete]
    private void EnemyDamage(float posX)
    {
        if (!gm.isGameOver())
        {
            //creamos un color para que cuando el jugador se haga daño se muestre de ese color
            //los valores los hay que normalizarlos en el rango de 0 a 1.  
            //por eso divide entre 255 que es el rango máximo
            Color nuevoColor = new Color(255f / 255f, 100f / 255f, 100f / 255f);
            sprite.color = nuevoColor;
            sfx.clip = sfxHit;
            sfx.Play();
            if (!hitBack)
            {
                // Calcula la dirección del enemigo en relación con el jugador
                float direction = Mathf.Sign(posX - transform.position.x);
                rb.linearVelocity = Vector2.zero;

                // Se aplica una fuerza de retroceso al jugador cuando toca a un enemigo
                //la dirección es negativa para que el jugador salte hacia atrás
                rb.AddForce(new Vector2(0.5f * -direction, 0.5f), ForceMode2D.Impulse);

                hitBack = true;
                gm.LoseLife();
                if (gm.GetLives() > 0)
                {
                    StartCoroutine(ResetHitBack());
                }
                else
                {
                    Dead();
                }

            }
        }
    }

    IEnumerator ResetHitBack()
    {
        yield return new WaitForSeconds(0.5f);
        hitBack = false;
        // Restablece el color del sprite al color original
        sprite.color = original;
    }

    [Obsolete]

    public void Dead()
    {
        // Desactiva los controles del personaje
        this.enabled = false;

        // Reproducir la animación DeadPlayer
        anim.Play("DeadPlayer");

        rb.linearVelocity = Vector2.zero;
        //darle un saltito
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);

        //y que se caiga hacia abajo
        col.enabled = false;
        if (!gm.isGameOver() && !gm.isChecking())
        {
            StartCoroutine(RespawnAfterAnimation());
        }
        else if (gm.isChecking())
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    [Obsolete]
    IEnumerator RespawnAfterAnimation()
    {
        yield return new WaitForSeconds(2.5f);
        gm.LoadScene();
    }
    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(3f);
        //reactivamod los controles del personaje 
        this.enabled = true;
        //reactivar el collider y la animacion inicial del jugaor
        anim.Play("Quieto");
        col.enabled = true;
        // Reiniciar la posición del jugador al checkpoint y la velocidad
        transform.position = checkPoint.transform.position;
        rb.linearVelocity = Vector2.zero;
    }
}



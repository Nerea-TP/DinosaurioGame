using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinsController : MonoBehaviour
{
    [SerializeField] AudioClip sfxCoins;
    [SerializeField] int coinValue;
    SpriteRenderer sprite;
    GameManager gameManager;
    ParticleSystem particles;
    AudioSource sfx;
    bool active = true;
    public string coinKey;

    void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        sfx = GameObject.Find("AllCoins").GetComponent<AudioSource>();
        coinKey = SceneManager.GetActiveScene().name + "_Coin_" + gameObject.name;
        if (PlayerPrefs.GetInt(coinKey, 0) == 1)
        {
            sprite.enabled = false; active = false;
        }
    }
    public void AdjustVolume(float volume)
    {
        sfx.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && active)
        {
            GameManager.SumCoins(coinValue);
            sfx.clip = sfxCoins;
            sfx.Play();
            sprite.enabled = false;
            particles.Play();
            active = false;

            // Guardar el estado de la moneda como recogida 
            PlayerPrefs.SetInt(coinKey, 1);
            Debug.Log("Moneda recogida: " + coinKey);
        }
    }
}


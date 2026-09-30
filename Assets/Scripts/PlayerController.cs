using UnityEngine;

// Coloque este script na nave do jogador.
// Move no eixo horizontal (← →) e atira com Espaço.
// A vida é controlada pelo PlayerHealth (mesmo objeto).
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float limiteEsquerda = -8f;
    [SerializeField] private float limiteDireita = 8f;

    [Header("Tiro")]
    [SerializeField] private GameObject prefabTiro;
    [SerializeField] private Transform pontoDeTiro;
    [SerializeField] private float cadenciaDeTiro = 0.35f;
    [SerializeField] private AudioClip somDeTiro;

    [Header("Dano")]
    [SerializeField] private float danoTiro = 10f;
    [SerializeField] private float danoColisao = 25f;

    private const string TagTiroInimigo = "TiroInimigo";
    private const string TagInimigo = "Inimigo";

    private float proximoTiro;
    private AudioSource audioSource;
    private PlayerHealth vida;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        vida = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!vida.EstaVivo) return;

        Mover();

        if (Input.GetKey(KeyCode.Space))
            TentarAtirar();
    }

    private void Mover()
    {
        float direcao = Input.GetAxisRaw("Horizontal");
        if (direcao == 0f) return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + direcao * velocidade * Time.deltaTime,
                            limiteEsquerda, limiteDireita);
        transform.position = pos;
    }

    private void TentarAtirar()
    {
        if (Time.time < proximoTiro || prefabTiro == null) return;

        proximoTiro = Time.time + cadenciaDeTiro;

        Vector3 origem = pontoDeTiro != null ? pontoDeTiro.position : transform.position;
        Instantiate(prefabTiro, origem, Quaternion.identity);

        if (somDeTiro != null && audioSource != null)
            audioSource.PlayOneShot(somDeTiro);
    }

    // Usado por power-ups de velocidade de tiro.
    public void AjustarCadencia(float novaCadencia)
    {
        cadenciaDeTiro = novaCadencia;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagTiroInimigo))
        {
            Destroy(other.gameObject);
            vida.ReceberDano(danoTiro);
        }
        else if (other.CompareTag(TagInimigo))
        {
            Destroy(other.gameObject);
            vida.ReceberDano(danoColisao);
        }
    }
}
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

    // Garante que só exista UMA nave, mesmo que cada cena tenha uma (útil pra testar fases soltas).
    private static PlayerController instancia;

    private float proximoTiro;
    private AudioSource audioSource;
    private PlayerHealth vida;

    // Usado pelo GameManager ao reiniciar/começar um jogo novo:
    // a nave morta fica inativa e persiste entre cenas, então precisa ser destruída.
    public static void DestruirInstancia()
    {
        if (instancia != null) Destroy(instancia.gameObject);
        instancia = null;
    }

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        instancia = this;

        audioSource = GetComponent<AudioSource>();
        vida = GetComponent<PlayerHealth>();

        // A nave sobrevive à troca de cena — assim ela só existe uma vez,
        // criada na primeira fase, e continua a mesma (com a vida atual) nas próximas.
        DontDestroyOnLoad(gameObject);
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
            // Passa pelo Enemy.LevarDano -> Morrer(), que avisa a formação.
            // Sem isso, a formação nunca esvazia e a fase não termina.
            Enemy inimigo = other.GetComponent<Enemy>();
            if (inimigo != null) inimigo.LevarDano(9999f);
            else Destroy(other.gameObject);

            vida.ReceberDano(danoColisao);
        }
    }
}

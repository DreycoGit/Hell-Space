using UnityEngine;

// Coloque este script na nave do jogador.
// A nave só se move no eixo horizontal (setas ← →) e atira com Espaço.
// A vida agora é cuidada pelo componente PlayerHealth (mesmo objeto).
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerCombat))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float limiteEsquerda = -8f;
    [SerializeField] private float limiteDireita = 8f;

    [Header("Tiro")]
    [SerializeField] private GameObject prefabTiro;
    [SerializeField] private Transform pontoDeTiro;
    [SerializeField] private float cadenciaDeTiro = 0.35f;
    [SerializeField] private AudioClip somDeTiro;

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
        Atirar();
    }

    private void Mover()
    {
        float direcao = Input.GetAxisRaw("Horizontal");
        Vector3 pos = transform.position;
        pos.x += direcao * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, limiteEsquerda, limiteDireita);
        transform.position = pos;
    }

    private void Atirar()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= proximoTiro)
        {
            proximoTiro = Time.time + cadenciaDeTiro;
            Vector3 origem = pontoDeTiro != null ? pontoDeTiro.position : transform.position;
            Instantiate(prefabTiro, origem, Quaternion.identity);

            if (somDeTiro != null && audioSource != null)
                audioSource.PlayOneShot(somDeTiro);
        }
    }

    // Permite ajustar a cadência de tiro (ex: power-up de velocidade de tiro).
    public void AjustarCadencia(float novaCadencia)
    {
        cadenciaDeTiro = novaCadencia;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TiroInimigo"))
        {
            Destroy(other.gameObject);
            vida.ReceberDano(10f); // ajuste o valor de dano por tiro aqui
        }
        else if (other.CompareTag("Inimigo"))
        {
            Destroy(other.gameObject);
            vida.ReceberDano(25f); // colisão direta dói mais que um tiro
        }
    }
}

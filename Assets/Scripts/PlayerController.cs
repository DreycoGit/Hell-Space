using UnityEngine;

// Coloque este script na nave do jogador.
// A nave só se move no eixo horizontal (setas ← →) e atira com Espaço.
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float limiteEsquerda = -8f;
    [SerializeField] private float limiteDireita = 8f;

    [Header("Tiro")]
    [SerializeField] private GameObject prefabTiro;
    [SerializeField] private Transform pontoDeTiro; // objeto vazio na frente da nave
    [SerializeField] private float cadenciaDeTiro = 0.35f; // segundos entre tiros
    [SerializeField] private AudioClip somDeTiro;

    [Header("Vida")]
    [SerializeField] private int vidas = 3;
    [SerializeField] private float tempoInvulneravel = 1.5f;

    private float proximoTiro;
    private bool invulneravel;
    private AudioSource audioSource;
    private SpriteRenderer sprite;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Mover();
        Atirar();
    }

    private void Mover()
    {
        // GetAxisRaw usando apenas as setas (Horizontal já está mapeado
        // para ← → e A/D por padrão no Input Manager do Unity).
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

    // Chamado pelo Bullet.cs ou pela colisão com um inimigo em mergulho.
    public void LevarDano()
    {
        if (invulneravel) return;

        vidas--;
        GameManager.Instancia.AtualizarVidas(vidas);

        if (vidas <= 0)
        {
            GameManager.Instancia.FimDeJogo();
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(FicarInvulneravel());
        }
    }

    private System.Collections.IEnumerator FicarInvulneravel()
    {
        invulneravel = true;
        float tempo = 0f;
        while (tempo < tempoInvulneravel)
        {
            if (sprite != null)
                sprite.enabled = !sprite.enabled; // pisca a nave
            tempo += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        if (sprite != null) sprite.enabled = true;
        invulneravel = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TiroInimigo"))
        {
            Destroy(other.gameObject);
            LevarDano();
        }
        else if (other.CompareTag("Inimigo"))
        {
            // Colisão direta com inimigo em mergulho: destrói os dois.
            Destroy(other.gameObject);
            LevarDano();
        }
    }
}
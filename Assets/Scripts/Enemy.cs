using UnityEngine;

// 8 tipos de inimigo comum: 4 níveis de força pra cada raça.
// Crimson aparece nas fases 1, 2, 4 e 5. White Sceles aparece nas fases 7 e 8
// (a mesma lógica de misturar os 4 níveis dentro da formação, só que com a raça
// e os valores trocados — White Sceles é mais forte no geral por vir depois no jogo).
public enum TipoDeInimigo
{
    CrimsonFraco,     // fileira da frente: 1 tiro, rápido, pouca pontuação
    CrimsonMedio,     // 2 tiros
    CrimsonForte,     // 3 tiros, atira mais
    CrimsonTanque,    // fileira de trás: bastante vida, lento, atira bastante

    WhiteScelesFraco,
    WhiteScelesMedio,
    WhiteScelesForte,
    WhiteScelesTanque
}

[System.Serializable]
public struct DadosDoInimigo
{
    public TipoDeInimigo tipo;
    public float vida;
    public float multiplicadorVelocidade;
    public float chanceDeAtirarPorSegundo;
    public int pontos;
}

public class Enemy : MonoBehaviour
{
    [Header("Configuração do tipo (ajuste vida/velocidade/pontos livremente aqui)")]
    [SerializeField] private DadosDoInimigo dados = new DadosDoInimigo
    {
        tipo = TipoDeInimigo.CrimsonFraco,
        vida = 1f,
        multiplicadorVelocidade = 1.3f,
        chanceDeAtirarPorSegundo = 0.12f,
        pontos = 50
    };

    [Header("Tiro do inimigo")]
    [SerializeField] private GameObject prefabTiroInimigo;

    [Header("Mergulho")]
    [SerializeField] private float velocidadeMergulho = 4f;

    private float vidaAtual;
    private bool emMergulho;
    private Vector3 posicaoNaFormacao;
    private FormationManager formacao;

    public TipoDeInimigo Tipo => dados.tipo;
    public bool EstaVivo => vidaAtual > 0f;

    private void Awake()
    {
        vidaAtual = dados.vida;
    }

    public void Inicializar(FormationManager gerenciador, Vector3 posicaoBase)
    {
        formacao = gerenciador;
        posicaoNaFormacao = posicaoBase;
    }

    private void Update()
    {
        if (emMergulho) return;

        if (prefabTiroInimigo != null && Random.value < dados.chanceDeAtirarPorSegundo * Time.deltaTime)
        {
            Instantiate(prefabTiroInimigo, transform.position, Quaternion.identity);
        }
    }

    public void SeguirFormacao(Vector3 novaPosicao)
    {
        if (!emMergulho)
            transform.position = novaPosicao;
    }

    public void IniciarMergulho(Vector3 alvo)
    {
        if (emMergulho) return;
        emMergulho = true;
        StartCoroutine(RotinaDeMergulho(alvo));
    }

    private System.Collections.IEnumerator RotinaDeMergulho(Vector3 alvo)
    {
        Vector3 origem = transform.position;
        float duracao = Vector3.Distance(origem, alvo) / (velocidadeMergulho * dados.multiplicadorVelocidade);
        float t = 0f;

        while (t < duracao)
        {
            t += Time.deltaTime;
            float progresso = t / duracao;
            float ondulacao = Mathf.Sin(progresso * Mathf.PI * 3f) * 1.2f;
            transform.position = Vector3.Lerp(origem, alvo, progresso) + new Vector3(ondulacao, 0f, 0f);
            yield return null;
        }

        emMergulho = false;
        transform.position = posicaoNaFormacao;
    }

    public void LevarDano(float dano)
    {
        vidaAtual -= dano;
        if (vidaAtual <= 0f)
        {
            Morrer();
        }
    }

    private void Morrer()
    {
        GameManager.Instancia?.AdicionarPontos(dados.pontos);
        formacao?.RemoverInimigo(this);
        Destroy(gameObject);
    }
}

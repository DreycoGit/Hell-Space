using UnityEngine;
using System.Collections;

// Boss final da Fase 10 — "Deus Antigo", com 2 estágios.
// Estágio 1: dormindo, não ataca (música de fundo para). Zerar a vida dele acorda o boss
// em vez de matá-lo de verdade.
// Estágio 2: muda de sprite e entra na rotação de ataques (cantos -> laser grande no meio -> tiros rápidos).
// Ao morrer no estágio 2, mostra a tela de vitória.
public class AncientGodBoss : BossBase
{
    [Header("Sprites por estágio")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteDormindo;
    [SerializeField] private Sprite spriteAcordado;

    // A vida do combate de verdade (estágio 2) é a "Vida Maxima" do BossBase, no Inspector.
    [Header("Vida do estágio 1 (dormindo; zerar só acorda o boss)")]
    [SerializeField] private float vidaEstagio1 = 50f;

    [Header("Prefabs de ataque (estágio 2)")]
    [SerializeField] private GameObject prefabTiroGrande;
    [SerializeField] private GameObject prefabTiroPequeno;
    [SerializeField] private GameObject prefabLaserGrande;

    [Header("Pontos de disparo")]
    [SerializeField] private Transform pontoEsquerda;
    [SerializeField] private Transform pontoDireita;
    [SerializeField] private Transform pontoCentro;

    [Header("Movimento (vai e volta na horizontal)")]
    [SerializeField] private float velocidadeMovimento = 1.5f;
    [SerializeField] private float limiteEsquerda = -3.5f;
    [SerializeField] private float limiteDireita = 3.5f;

    private int direcaoMovimento = 1;
    private int estagioAtual = 1;
    private float vidaDoCombate;

    // Guarda a vida do Inspector pro estágio 2 e começa com a vida do estágio 1.
    protected override void Awake()
    {
        vidaDoCombate = vidaMaxima;
        vidaMaxima = vidaEstagio1;
        base.Awake();
    }

    private void Update()
    {
        if (!EstaVivo) return;

        transform.position += Vector3.right * direcaoMovimento * velocidadeMovimento * Time.deltaTime;

        if (transform.position.x <= limiteEsquerda || transform.position.x >= limiteDireita)
            direcaoMovimento *= -1;
    }

    protected override void IniciarBatalha()
    {
        if (spriteRenderer != null && spriteDormindo != null)
            spriteRenderer.sprite = spriteDormindo;

        AudioManager.Instancia?.PararMusica();
    }

    // Sobrescreve ReceberDano porque zerar a vida do estágio 1 não mata o boss —
    // só faz ele acordar e entrar no estágio 2, com vida nova.
    public override void ReceberDano(float dano)
    {
        if (!EstaVivo) return;

        vidaAtual -= dano;
        vidaAtual = Mathf.Max(0f, vidaAtual);
        GameManager.Instancia?.AtualizarVidaBoss(VidaPercentual);

        if (vidaAtual <= 0f)
        {
            if (estagioAtual == 1)
                AcordarParaEstagio2();
            else
                Derrotado();
        }
    }

    private void AcordarParaEstagio2()
    {
        estagioAtual = 2;
        vidaMaxima = vidaDoCombate;
        vidaAtual = vidaDoCombate;

        if (spriteRenderer != null && spriteAcordado != null)
            spriteRenderer.sprite = spriteAcordado;

        AudioManager.Instancia?.TocarMusicaBoss();

        StartCoroutine(RotinaDeAtaquesEstagio2());
    }

    private IEnumerator RotinaDeAtaquesEstagio2()
    {
        while (EstaVivo)
        {
            // Tiros grandes nos cantos
            Instantiate(prefabTiroGrande, pontoEsquerda.position, Quaternion.identity);
            Instantiate(prefabTiroGrande, pontoDireita.position, Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
            if (!EstaVivo) yield break;

            // Carrega e solta um laser grande no meio, ativo por um bom tempo
            GameObject Laser = Instantiate(prefabLaserGrande, pontoCentro.position, Quaternion.identity);
            Laser.transform.SetParent(pontoCentro.transform); // faz o ponto de disparo do laser se mover junto com ele
            yield return new WaitForSeconds(3.5f); // ajuste conforme o tempoAtivo do Laser
            if (!EstaVivo) yield break;

            // Tiros pequenos e mais rápidos
            for (int i = 0; i < 6; i++)
            {
                if (!EstaVivo) yield break;
                Transform ponto = (i % 2 == 0) ? pontoEsquerda : pontoDireita;
                Instantiate(prefabTiroPequeno, ponto.position, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // Só roda quando o estágio 2 zera (a morte de verdade do boss final)
    protected override void Derrotado()
    {
        base.Derrotado();
        GameManager.Instancia?.FimDeJogoVitoria();
        TelaVitoria.Instancia?.Mostrar(); // <- linha nova: mostra o painel de vitória
    }
}
using UnityEngine;
using System.Collections;

// Boss final da Fase 10 — "Deus Antigo", com 2 estágios.
// Estágio 1: dormindo, não ataca (música de fundo para). Zerar a vida dele acorda o boss
// em vez de matá-lo de verdade.
// Estágio 2: muda de sprite e entra na rotação de ataques (cantos -> laser grande no meio -> tiros rápidos).
public class AncientGodBoss : BossBase
{
    [Header("Sprites por estágio")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteDormindo;
    [SerializeField] private Sprite spriteAcordado;

    [Header("Vida de cada estágio")]
    [SerializeField] private float vidaEstagio1 = 300f;
    [SerializeField] private float vidaEstagio2 = 700f;

    [Header("Prefabs de ataque (estágio 2)")]
    [SerializeField] private GameObject prefabTiroGrande;
    [SerializeField] private GameObject prefabTiroPequeno;
    [SerializeField] private GameObject prefabLaserGrande;

    [Header("Pontos de disparo")]
    [SerializeField] private Transform pontoEsquerda;
    [SerializeField] private Transform pontoDireita;
    [SerializeField] private Transform pontoCentro;

    private int estagioAtual = 1;

    protected override void Awake()
    {
        vidaMaxima = vidaEstagio1;
        base.Awake();
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
        vidaMaxima = vidaEstagio2;
        vidaAtual = vidaEstagio2;

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
            Instantiate(prefabLaserGrande, pontoCentro.position, Quaternion.identity);
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

    protected override void Derrotado()
    {
        base.Derrotado();
        GameManager.Instancia?.FimDeJogoVitoria();
    }
}

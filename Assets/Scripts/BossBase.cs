using UnityEngine;
using System;

// Classe-base pra todos os bosses (Meteoro, LaserBoss, SpamBoss, AncienteGodBoss).
// Cada boss só precisa implementar IniciarBatalha() com sua própria rotina de ataque.
public abstract class BossBase : MonoBehaviour
{
    [Header("Vida do boss")]
    [SerializeField] protected float vidaMaxima = 500f;
    protected float vidaAtual;

    // O PhaseManager assina este evento pra saber quando avançar de fase.
    public event Action OnBossDerrotado;

    public float VidaPercentual => vidaAtual / vidaMaxima;
    public bool EstaVivo => vidaAtual > 0f;

    protected virtual void Awake()
    {
        vidaAtual = vidaMaxima;
    }

    protected virtual void Start()
    {
        IniciarBatalha();
    }

    // Cada boss implementa sua própria rotina de ataque (normalmente com uma Coroutine).
    protected abstract void IniciarBatalha();

    // Chamado centralmente pelo Bullet.cs quando um tiro do jogador acerta o boss.
    public virtual void ReceberDano(float dano)
    {
        if (!EstaVivo) return;

        vidaAtual -= dano;
        vidaAtual = Mathf.Max(0f, vidaAtual);
        GameManager.Instancia?.AtualizarVidaBoss(VidaPercentual);

        if (vidaAtual <= 0f)
        {
            Derrotado();
        }
    }

    protected virtual void Derrotado()
    {
        StopAllCoroutines();
        OnBossDerrotado?.Invoke();
        Destroy(gameObject, 0.5f); // dá tempo de tocar uma animação/som de explosão antes de sumir
    }
}

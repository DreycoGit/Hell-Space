using UnityEngine;
using System.Collections;

// Boss da Fase 9: ataque simples de spam contínuo de tiros, esquivável mas exigente.
public class SpamBoss : BossBase
{
    [SerializeField] private GameObject prefabTiro;
    [SerializeField] private Transform[] pontosDeTiro;
    [SerializeField] private float intervaloEntreTiros = 0.35f;

    protected override void Awake()
    {
        vidaMaxima = 900f;
        base.Awake();
    }

    protected override void IniciarBatalha()
    {
        StartCoroutine(RotinaDeSpam());
    }

    private IEnumerator RotinaDeSpam()
    {
        while (EstaVivo)
        {
            Transform ponto = pontosDeTiro[Random.Range(0, pontosDeTiro.Length)];
            Instantiate(prefabTiro, ponto.position, Quaternion.identity);
            yield return new WaitForSeconds(intervaloEntreTiros);
        }
    }
}
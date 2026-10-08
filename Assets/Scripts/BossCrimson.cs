using UnityEngine;
using System.Collections;

// King Crimson, boss da Fase 6. Rotação de ataques que se repete até ele morrer:
// 1) alguns tiros médios em sequência
// 2) dois lasers nas pontas (esquerda e direita) — jogador precisa ir pro meio
// 3) um laser no meio — jogador precisa ir pras pontas
public class BossCrimson : BossBase
{
    [Header("Prefabs")]
    [SerializeField] private GameObject prefabTiroMedio;
    [SerializeField] private GameObject prefabLaser;

    [Header("Pontos de disparo")]
    [SerializeField] private Transform pontoCentro;
    [SerializeField] private Transform pontoEsquerda;
    [SerializeField] private Transform pontoDireita;

    [Header("Timings (ajuste pra dar tempo do jogador reagir)")]
    [SerializeField] private int quantidadeTirosSequencia = 4;
    [SerializeField] private float intervaloEntreTiros = 0.4f;
    [SerializeField] private float esperaAntesDosLaseresLaterais = 1f;
    [SerializeField] private float esperaEntreLateraisEMeio = 1.5f;
    [SerializeField] private float esperaAposLaserMeio = 1.5f;

    [Header("Movimento (vai e volta na horizontal)")]
    [SerializeField] private float velocidadeMovimento = 1.5f;
    [SerializeField] private float limiteEsquerda = -3.5f;
    [SerializeField] private float limiteDireita = 3.5f;

    private int direcaoMovimento = 1;

    protected override void Awake()
    {
        vidaMaxima = 1000f;
        base.Awake();
    }

    protected override void IniciarBatalha()
    {
        StartCoroutine(RotinaDeAtaques());
    }

    private void Update()
    {
        if (!EstaVivo) return;

        transform.position += Vector3.right * direcaoMovimento * velocidadeMovimento * Time.deltaTime;

        if (transform.position.x <= limiteEsquerda || transform.position.x >= limiteDireita)
            direcaoMovimento *= -1;
    }

    private IEnumerator RotinaDeAtaques()
    {
        while (EstaVivo)
        {
            for (int i = 0; i < quantidadeTirosSequencia; i++)
            {
                if (!EstaVivo) yield break;
                Instantiate(prefabTiroMedio, pontoCentro.position, Quaternion.identity);
                yield return new WaitForSeconds(intervaloEntreTiros);
            }

            yield return new WaitForSeconds(esperaAntesDosLaseresLaterais);
            if (!EstaVivo) yield break;

            Instantiate(prefabLaser, pontoEsquerda.position, Quaternion.identity);
            GameObject Laser1 = Instantiate(prefabLaser, pontoDireita.position, Quaternion.identity);
            Laser1.transform.SetParent(pontoCentro.transform);

            yield return new WaitForSeconds(esperaEntreLateraisEMeio);
            if (!EstaVivo) yield break;

            Instantiate(prefabLaser, pontoCentro.position, Quaternion.identity);
            GameObject Laser2 = Instantiate(prefabLaser, pontoCentro.position, Quaternion.identity);
            Laser2.transform.SetParent(pontoCentro.transform);

            yield return new WaitForSeconds(esperaAposLaserMeio);
        }
    }
}
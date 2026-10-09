using UnityEngine;
using System.Collections;

// O Cultista, boss da Fase 9: ataque simples de spam contínuo de tiros, esquivável mas exigente.
public class BossWhiteScale : BossBase
{
    [SerializeField] private GameObject prefabTiro;
    [SerializeField] private Transform[] pontosDeTiro;
    [SerializeField] private float intervaloEntreTiros = 0.35f;

    [Header("Movimento (vai e volta na horizontal)")]
    [SerializeField] private float velocidadeMovimento = 1.5f;
    [SerializeField] private float limiteEsquerda = -3.5f;
    [SerializeField] private float limiteDireita = 3.5f;

    private int direcaoMovimento = 1;

    protected override void IniciarBatalha()
    {
        StartCoroutine(RotinaDeSpam());
    }

    private void Update()
    {
        if (!EstaVivo) return;

        transform.position += Vector3.right * direcaoMovimento * velocidadeMovimento * Time.deltaTime;

        if (transform.position.x <= limiteEsquerda || transform.position.x >= limiteDireita)
            direcaoMovimento *= -1;
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
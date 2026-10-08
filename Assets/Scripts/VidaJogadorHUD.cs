using UnityEngine;
using UnityEngine.UI;

// Coloque este script em um objeto de UI com um componente Image (o retrato de vida).
// Ele troca o sprite do retrato conforme a vida do jogador.
[RequireComponent(typeof(Image))]
public class VidaJogadorHUD : MonoBehaviour
{
    [Header("Retratos de vida")]
    [SerializeField] private Sprite retratoVerde;     // vida alta
    [SerializeField] private Sprite retratoAmarelo;   // vida média
    [SerializeField] private Sprite retratoVermelho;  // vida baixa

    [Header("Limites (em % da vida máxima)")]
    [SerializeField] private float limiteMedia = 66f;
    [SerializeField] private float limiteBaixa = 33f;

    private Image imagem;
    private PlayerHealth jogador;

    private void Awake()
    {
        imagem = GetComponent<Image>();
    }

    private void Update()
    {
        // O jogador pode ser criado depois (ele persiste entre cenas), então procura até achar.
        if (jogador == null)
        {
            jogador = FindObjectOfType<PlayerHealth>();
            if (jogador == null) return;
        }

        AtualizarRetrato(jogador.VidaPercentual * 100f);
    }

    private void AtualizarRetrato(float percentual)
    {
        Sprite novo;

        if (percentual <= limiteBaixa)
            novo = retratoVermelho;
        else if (percentual <= limiteMedia)
            novo = retratoAmarelo;
        else
            novo = retratoVerde;

        if (novo != null)
            imagem.sprite = novo;
    }
}
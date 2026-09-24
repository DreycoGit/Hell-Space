using UnityEngine;

public enum TipoDePowerUp
{
    TiroRapido,
    MaisDano,
    Cura
}

// Coloque este script num objeto com Collider2D (Is Trigger) e um sprite do power-up.
// Ao colidir com o jogador, aplica o efeito e se destrói.
public class PowerUp : MonoBehaviour
{
    [SerializeField] private TipoDePowerUp tipo;
    [SerializeField] private float valorCura = 25f;
    [SerializeField] private float novaCadenciaDeTiro = 0.15f;
    [SerializeField] private float cadenciaPadraoParaVoltar = 0.35f;
    [SerializeField] private float multiplicadorDano = 2f;
    [SerializeField] private float duracaoDoBuff = 8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Jogador")) return;

        switch (tipo)
        {
            case TipoDePowerUp.TiroRapido:
                PlayerController controlador = other.GetComponent<PlayerController>();
                if (controlador != null)
                    controlador.StartCoroutine(AplicarTiroRapidoTemporario(controlador));
                break;

            case TipoDePowerUp.MaisDano:
                PlayerCombat combate = other.GetComponent<PlayerCombat>();
                combate?.AtivarMaisDano(multiplicadorDano, duracaoDoBuff);
                break;

            case TipoDePowerUp.Cura:
                PlayerHealth vida = other.GetComponent<PlayerHealth>();
                vida?.Curar(valorCura);
                break;
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator AplicarTiroRapidoTemporario(PlayerController controlador)
    {
        controlador.AjustarCadencia(novaCadenciaDeTiro);
        yield return new WaitForSeconds(duracaoDoBuff);
        controlador.AjustarCadencia(cadenciaPadraoParaVoltar);
    }
}

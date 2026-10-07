using UnityEngine;

// Coloque esse script em UM GameObject vazio por fase (ex: "GerenciadorDeFase"),
// ao lado do FormationManager. Configure as ondas no Inspector — cada onda roda
// em sequência: quando o FormationManager avisa que a onda morreu inteira
// (OnFormacaoLimpa), a próxima onda começa automaticamente.
public class GerenciadorDeFase : MonoBehaviour
{
    [System.Serializable]
    public struct Onda
    {
        [Tooltip("Prefabs de inimigo, um por fileira (o primeiro é a fileira da frente/mais fraca)")]
        public GameObject[] linhas;
        public int colunas;
        public float espacoX;
        public float espacoY;
        public float origemX;
        public float origemY;
        public float velocidade;
    }

    [SerializeField] private FormationManager formationManager;
    [SerializeField] private Onda[] ondas;

    private int ondaAtual;

    private void Start()
    {
        formationManager.OnFormacaoLimpa += ProximaOnda;
        ChamarOnda(0);
    }

    private void ProximaOnda()
    {
        ondaAtual++;
        if (ondaAtual < ondas.Length)
        {
            ChamarOnda(ondaAtual);
        }
        else
        {
            Debug.Log("Fase concluída! Todas as ondas derrotadas.");
            // Aqui depois entra a troca de cena pra próxima fase.
        }
    }

    private void ChamarOnda(int indice)
    {
        Onda onda = ondas[indice];
        Vector3 origem = new Vector3(onda.origemX, onda.origemY, 0f);
        formationManager.MontarFormacao(onda.linhas, onda.colunas, onda.espacoX, onda.espacoY, origem, onda.velocidade);
    }

    private void OnDestroy()
    {
        if (formationManager != null)
            formationManager.OnFormacaoLimpa -= ProximaOnda;
    }
}
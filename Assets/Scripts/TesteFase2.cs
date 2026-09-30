using UnityEngine;

public class TesteFase2 : MonoBehaviour
{
    [SerializeField] private FormationManager formationManager;
    [SerializeField] private GameObject prefabCrimisomFraco;

    private void Start()
    {
        GameObject[] linhas = { prefabCrimisomFraco, prefabCrimisomFraco };
        formationManager.MontarFormacao(linhas, colunas: 10, espacoX: 1.3f, espacoY: 1f, origem: new Vector3(-5.85f, 5f, 0f), velocidade: 1f);
    }
}
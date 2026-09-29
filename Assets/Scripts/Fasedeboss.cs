using UnityEngine;
using UnityEngine.SceneManagement;


public class FaseDeBoss : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private string nomeDaFase = "Boss";
    [SerializeField] private int numeroDaFase = 3;
    [SerializeField] private string nomeDaProximaCena; // deixe vazio se for a última (ex: Fase 10)

    private void Start()
    {
        GameManager.Instancia?.AtualizarFase(numeroDaFase, nomeDaFase);
        AudioManager.Instancia?.TocarMusicaBoss();

        if (boss != null)
            boss.OnBossDerrotado += IrParaProximaFase;
    }

    private void IrParaProximaFase()
    {
        if (!string.IsNullOrEmpty(nomeDaProximaCena))
            SceneManager.LoadScene(nomeDaProximaCena);
    }
}
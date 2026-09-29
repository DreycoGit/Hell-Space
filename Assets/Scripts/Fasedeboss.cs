using UnityEngine;
using UnityEngine.SceneManagement;

// Coloque este script num objeto vazio nas cenas de boss (fases 3, 6, 9 e 10).
// Arraste o boss que já está montado na própria cena no campo "Boss" abaixo.
// Quando ele for derrotado, carrega a próxima cena automaticamente.
public class FaseDeBoss : MonoBehaviour
{
    [SerializeField] private BossBase boss;
    [SerializeField] private string nomeDaFase = "Boss";
    [SerializeField] private int numeroDaFase = 3;
    [SerializeField] private string nomeDaProximaCena; // deixe vazio se for a última (ex: Fase 10)

    private void Start()
    {
        GameManager.Instancia?.AtualizarFase(numeroDaFase, nomeDaFase);
        GameManager.Instancia?.AtualizarVidaBoss(1f); // reseta a barra pro boss novo (evita mostrar a vida do boss anterior)
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
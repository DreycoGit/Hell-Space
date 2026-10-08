using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Coloque num objeto vazio nas cenas de boss (fases 3, 6, 9 e 10).
// Espera o boss ser derrotado (evento OnBossDerrotado do BossBase) e troca de cena.
public class TrocaDeFaseBoss : MonoBehaviour
{
    [Header("Identificação da fase")]
    [SerializeField] private string nomeDaFase = "Boss";
    [SerializeField] private int numeroDaFase = 3;

    [Header("Transição de cena (deixe vazio se for a última: mostra a vitória)")]
    [SerializeField] private string nomeDaProximaCena;
    [SerializeField] private float atrasoParaTrocar = 1.5f;

    [Header("Áudio")]
    [SerializeField] private bool tocarMusicaDeBoss = true;

    private BossBase boss;
    private bool trocando;

    private void Start()
    {
        GameManager.Instancia?.AtualizarFase(numeroDaFase, nomeDaFase);
        if (tocarMusicaDeBoss) AudioManager.Instancia?.TocarMusicaBoss();
        StartCoroutine(ProcurarBoss());
    }

    private IEnumerator ProcurarBoss()
    {
        float esperado = 0f;
        while (boss == null && esperado < 10f)
        {
            boss = FindAnyObjectByType<BossBase>();
            if (boss != null) break;
            esperado += 0.25f;
            yield return new WaitForSeconds(0.25f);
        }

        if (boss == null)
        {
            Debug.LogWarning("TrocaDeFaseBoss: nenhum BossBase encontrado na cena.");
            yield break;
        }

        boss.OnBossDerrotado += AoBossDerrotado;
    }

    private void OnDestroy()
    {
        if (boss != null) boss.OnBossDerrotado -= AoBossDerrotado;
    }

    private void AoBossDerrotado()
    {
        if (trocando) return;
        trocando = true;
        StartCoroutine(Trocar());
    }

    private IEnumerator Trocar()
    {
        yield return new WaitForSeconds(atrasoParaTrocar);
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nomeDaProximaCena))
            SceneManager.LoadScene(nomeDaProximaCena);
        else
            GameManager.Instancia?.FimDeJogoVitoria();
    }
}
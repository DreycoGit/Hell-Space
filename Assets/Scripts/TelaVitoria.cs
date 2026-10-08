using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaVitoria : MonoBehaviour
{
    public static TelaVitoria Instancia;

    [SerializeField] private GameObject painelVitoria;
    [SerializeField] private string nomeCenaMenu = "Menu";

    private bool venceu = false;

    void Awake()
    {
        Instancia = this;
        if (painelVitoria != null)
            painelVitoria.SetActive(false);
    }

    public void Mostrar()
    {
        if (venceu) return;
        venceu = true;

        painelVitoria.SetActive(true);
        Time.timeScale = 0f; // pausa o jogo
    }

    void Update()
    {
        if (venceu && Input.GetKeyDown(KeyCode.Return))
            Voltar();
    }

    public void Voltar()
    {
        Time.timeScale = 1f; // sempre volte ao normal antes de trocar de cena
        SceneManager.LoadScene(nomeCenaMenu);
    }
}
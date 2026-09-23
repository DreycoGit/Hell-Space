using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Cena do jogo")]
    public string nomeCenaJogo = "Jogo"; 
    [Header("Painéis")]
    public GameObject painelMenu;
    public GameObject painelConfiguracoes;
    public GameObject painelCosmeticos;

    void Start()
    {
        AbrirMenuPrincipal();
    }

    public void Jogar()
    {
        SceneManager.LoadScene(nomeCenaJogo);
    }

    public void AbrirConfiguracoes()
    {
        painelMenu.SetActive(false);
        painelConfiguracoes.SetActive(true);
    }

    public void AbrirCosmeticos()
    {
        painelMenu.SetActive(false);
        painelCosmeticos.SetActive(true);
    }

    public void AbrirMenuPrincipal()
    {
        painelMenu.SetActive(true);
        painelConfiguracoes.SetActive(false);
        painelCosmeticos.SetActive(false);
    }

    public void SairDoJogo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
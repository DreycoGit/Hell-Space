using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Botão PLAY
    public void JogarJogo()
    {
        SceneManager.LoadScene("Game"); // troque "Game" pelo nome exato da sua cena do jogo
    }

    // Botão OPÇÕES
    public GameObject painelOpcoes; // arraste um painel de opções aqui no Inspector, se tiver um

    public void AbrirOpcoes()
    {
        if (painelOpcoes != null)
            painelOpcoes.SetActive(true);
        else
            Debug.Log("Painel de opções ainda não configurado.");
    }

    public void FecharOpcoes()
    {
        if (painelOpcoes != null)
            painelOpcoes.SetActive(false);
    }

    // Botão SAIR
    public void SairJogo()
    {
        Application.Quit();
        Debug.Log("Saindo do jogo...");
    }
}
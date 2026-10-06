using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject painelOpcoes;
    public Slider sliderVolume;

    void Start()
    {
        // Se o jogador voltou pro menu depois de Game Over/Vitória, o tempo ainda estaria parado.
        Time.timeScale = 1f;

        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volume;

        if (sliderVolume != null)
        {
            sliderVolume.minValue = 0f;
            sliderVolume.maxValue = 1f;
            sliderVolume.wholeNumbers = false;
            sliderVolume.SetValueWithoutNotify(volume);
            sliderVolume.onValueChanged.AddListener(MudarVolume);
        }

        if (painelOpcoes != null)
            painelOpcoes.SetActive(false);
    }

    // Botão PLAY
    public void JogarJogo()
    {
        // Zera pontos, vida e a nave da partida anterior (se existirem).
        GameManager.Instancia?.NovoJogo();
        SceneManager.LoadScene("Fase-1");
    }

    // Botão OPÇÕES
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

    // Slider de volume
    public void MudarVolume(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("Volume", valor);
    }

    // Botão SAIR
    public void SairJogo()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

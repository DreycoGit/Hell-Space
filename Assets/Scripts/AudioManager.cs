using UnityEngine;

// Gerenciador simples de música de fundo e efeitos sonoros (singleton).
// Acesse de qualquer script via AudioManager.Instancia.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Fontes de áudio")]
    [SerializeField] private AudioSource fonteMusica;
    [SerializeField] private AudioSource fonteEfeitos;

    [Header("Músicas")]
    [SerializeField] private AudioClip musicaFase;
    [SerializeField] private AudioClip musicaBoss;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TocarMusicaFase()
    {
        if (fonteMusica == null || musicaFase == null) return;
        fonteMusica.clip = musicaFase;
        fonteMusica.loop = true;
        fonteMusica.Play();
    }

    public void TocarMusicaBoss()
    {
        if (fonteMusica == null || musicaBoss == null) return;
        fonteMusica.clip = musicaBoss;
        fonteMusica.loop = true;
        fonteMusica.Play();
    }

    public void PararMusica()
    {
        if (fonteMusica != null) fonteMusica.Stop();
    }

    public void TocarEfeito(AudioClip clipe)
    {
        if (fonteEfeitos != null && clipe != null)
            fonteEfeitos.PlayOneShot(clipe);
    }
}

using UnityEngine;

// Representa um raio laser (usado pelo CrimisonBoss da Fase 6 e pelo AncientGodBoss da Fase 10).
// depois fica sólido e ativo causando dano por um tempo, e então some.
public class Laser : MonoBehaviour
{
    [SerializeField] private float tempoAviso = 1f;
    [SerializeField] private float tempoAtivo = 1.5f;
    [SerializeField] private float dano = 20f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D colisor; // deixe "Is Trigger" marcado no prefab

    private void Start()
    {
        StartCoroutine(Rotina());
    }

    private System.Collections.IEnumerator Rotina()
    {
        if (colisor != null) colisor.enabled = false;

        float t = 0f;
        while (t < tempoAviso)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(t * 6f, 1f) * 0.6f + 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = Color.white;
        if (colisor != null) colisor.enabled = true;

        yield return new WaitForSeconds(tempoAtivo);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Jogador"))
        {
            other.GetComponent<PlayerHealth>()?.ReceberDano(dano);
        }
    }
}
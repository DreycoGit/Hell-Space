using UnityEngine;

// Coloque este script no prefab de cada inimigo.
// Quando o inimigo é destruído dentro da tela, sorteia um power-up para dropar.
public class DropDePowerUp : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        [Tooltip("Prefab do power-up (com o script PowerUpItem)")]
        public GameObject prefab;
        [Tooltip("Peso: quanto maior, mais comum. Ex.: vida 1, rápido 2, forte 2")]
        public float peso = 1f;
    }

    [Header("Chance")]
    [Range(0f, 1f)]
    [Tooltip("Chance de dropar ALGUM power-up (0.15 = 15%)")]
    [SerializeField] private float chanceDeDrop = 0.15f;

    [Header("Power-ups possíveis")]
    [SerializeField] private Item[] itens;

    private bool jaDropou;

    private void OnDestroy()
    {
        // Não dropa ao fechar o jogo ou trocar de cena.
        if (!Application.isPlaying || !gameObject.scene.isLoaded) return;
        if (jaDropou || itens == null || itens.Length == 0) return;

        // Não dropa se o inimigo só saiu da tela.
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 v = cam.WorldToViewportPoint(transform.position);
            if (v.x < 0f || v.x > 1f || v.y < 0f || v.y > 1f) return;
        }

        jaDropou = true;

        if (Random.value > chanceDeDrop) return;

        GameObject escolhido = Sortear();
        if (escolhido != null)
            Instantiate(escolhido, transform.position, Quaternion.identity);
    }

    // Sorteia um item respeitando os pesos.
    private GameObject Sortear()
    {
        float total = 0f;
        foreach (var i in itens)
            if (i.prefab != null) total += i.peso;

        if (total <= 0f) return null;

        float sorteio = Random.value * total;
        foreach (var i in itens)
        {
            if (i.prefab == null) continue;
            sorteio -= i.peso;
            if (sorteio <= 0f) return i.prefab;
        }
        return null;
    }
}
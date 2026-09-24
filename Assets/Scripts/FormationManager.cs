using System.Collections.Generic;
using UnityEngine;

// Monta UMA formação de inimigos com a configuração que o PhaseManager mandar,
// move ela em bloco (estilo Space Invaders/Galaga) e avisa quando todos morrerem.
// Não decide mais sozinho quando trocar de onda — isso agora é papel do PhaseManager.
public class FormationManager : MonoBehaviour
{
    [Header("Movimento da formação")]
    [SerializeField] private float limiteEsquerda = -6.5f;
    [SerializeField] private float limiteDireita = 6.5f;
    [SerializeField] private float passoParaBaixo = 0.4f;

    [Header("Mergulhos")]
    [SerializeField] private float chanceDeMergulhoPorSegundo = 0.15f;
    [SerializeField] private Transform jogador;

    // O PhaseManager assina este evento pra saber quando pode avançar de fase.
    public event System.Action OnFormacaoLimpa;

    private readonly List<Enemy> inimigos = new List<Enemy>();
    private readonly Dictionary<Enemy, Vector3> posicoesBase = new Dictionary<Enemy, Vector3>();
    private int direcao = 1;
    private float velocidadeAtual = 1.5f;

    // Chamado pelo PhaseManager no início de cada fase de inimigos comuns.
    // linhasDeInimigos[0] é a linha da frente (mais fraca), as últimas são as mais fortes.
    public void MontarFormacao(GameObject[] linhasDeInimigos, int colunas, float espacoX, float espacoY, Vector3 origem, float velocidade)
    {
        // Limpa qualquer inimigo restante de uma formação anterior.
        foreach (Enemy restante in inimigos)
        {
            if (restante != null) Destroy(restante.gameObject);
        }

        inimigos.Clear();
        posicoesBase.Clear();
        direcao = 1;
        velocidadeAtual = velocidade;

        for (int linha = 0; linha < linhasDeInimigos.Length; linha++)
        {
            GameObject prefab = linhasDeInimigos[linha];
            if (prefab == null) continue;

            for (int coluna = 0; coluna < colunas; coluna++)
            {
                Vector3 posicao = origem + new Vector3(coluna * espacoX, -linha * espacoY, 0f);
                GameObject obj = Instantiate(prefab, posicao, Quaternion.identity, transform);
                Enemy inimigo = obj.GetComponent<Enemy>();
                inimigo.Inicializar(this, posicao);

                inimigos.Add(inimigo);
                posicoesBase[inimigo] = posicao;
            }
        }
    }

    private void Update()
    {
        if (inimigos.Count == 0) return;
        MoverFormacao();
        SortearMergulho();
    }

    private void MoverFormacao()
    {
        float deslocamento = direcao * velocidadeAtual * Time.deltaTime;
        bool bateuNaBorda = false;

        List<Enemy> chaves = new List<Enemy>(posicoesBase.Keys);
        foreach (Enemy inimigo in chaves)
        {
            if (inimigo == null) continue;
            Vector3 posicao = posicoesBase[inimigo];
            posicao.x += deslocamento;
            posicoesBase[inimigo] = posicao;

            if (posicao.x <= limiteEsquerda || posicao.x >= limiteDireita)
                bateuNaBorda = true;

            inimigo.SeguirFormacao(posicao);
        }

        if (bateuNaBorda)
        {
            direcao *= -1;
            chaves = new List<Enemy>(posicoesBase.Keys);
            foreach (Enemy inimigo in chaves)
            {
                if (inimigo == null) continue;
                Vector3 posicao = posicoesBase[inimigo];
                posicao.y -= passoParaBaixo;
                posicoesBase[inimigo] = posicao;
            }
        }
    }

    private void SortearMergulho()
    {
        if (jogador == null || inimigos.Count == 0) return;

        if (Random.value < chanceDeMergulhoPorSegundo * Time.deltaTime)
        {
            Enemy escolhido = inimigos[Random.Range(0, inimigos.Count)];
            if (escolhido != null)
                escolhido.IniciarMergulho(jogador.position);
        }
    }

    public void RemoverInimigo(Enemy inimigo)
    {
        inimigos.Remove(inimigo);
        posicoesBase.Remove(inimigo);

        if (inimigos.Count == 0)
        {
            OnFormacaoLimpa?.Invoke();
        }
    }
}

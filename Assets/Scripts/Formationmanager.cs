using System.Collections.Generic;
using UnityEngine;

// Coloque este script em um GameObject vazio "GerenciadorDeFormacao".
// Ele monta a grade de inimigos, move todos em bloco (estilo Space Invaders/Galaga)
// e sorteia mergulhos aleatórios.
public class FormationManager : MonoBehaviour
{
    [Header("Prefabs dos 4 tipos (do mais fraco ao mais forte)")]
    [SerializeField] private GameObject prefabZangao;
    [SerializeField] private GameObject prefabBorboleta;
    [SerializeField] private GameObject prefabComandante;
    [SerializeField] private GameObject prefabImperador;

    [Header("Grade")]
    [SerializeField] private int colunas = 7;
    [SerializeField] private int linhas = 4;
    [SerializeField] private float espacoX = 1.6f;
    [SerializeField] private float espacoY = 1.2f;
    [SerializeField] private Vector3 origem = new Vector3(-4.8f, 4.2f, 0f);

    [Header("Movimento da formação")]
    [SerializeField] private float velocidadeBase = 1.5f;
    [SerializeField] private float limiteEsquerda = -6.5f;
    [SerializeField] private float limiteDireita = 6.5f;
    [SerializeField] private float passoParaBaixo = 0.4f;

    [Header("Mergulhos")]
    [SerializeField] private float chanceDeMergulhoPorSegundo = 0.15f;
    [SerializeField] private Transform jogador;

    private readonly List<Enemy> inimigos = new List<Enemy>();
    private readonly Dictionary<Enemy, Vector3> posicoesBase = new Dictionary<Enemy, Vector3>();
    private int direcao = 1;
    private int onda = 1;

    private void Start()
    {
        MontarOnda();
    }

    private void Update()
    {
        if (inimigos.Count == 0) return;

        MoverFormacao();
        SortearMergulho();
    }

    public void MontarOnda()
    {
        inimigos.Clear();
        posicoesBase.Clear();

        for (int linha = 0; linha < linhas; linha++)
        {
           
            int indiceTipo = Mathf.Min(3, linha + (onda - 1) / 2);
            GameObject prefab = PrefabPorIndice(indiceTipo);

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

    private GameObject PrefabPorIndice(int indice)
    {
        switch (indice)
        {
            case 0: return prefabZangao;
            case 1: return prefabBorboleta;
            case 2: return prefabComandante;
            default: return prefabImperador;
        }
    }

    private void MoverFormacao()
    {
        float deslocamento = direcao * velocidadeBase * (1f + (onda - 1) * 0.1f) * Time.deltaTime;
        bool bateuNaBorda = false;

        List<Enemy> chave = new List<Enemy>(posicoesBase.Keys);
        foreach (Enemy inimigo in chave)
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
            // Desce um degrau quando bate na borda (como no Space Invaders/Galaga clássico).
            List<Enemy> chaves = new List<Enemy>(posicoesBase.Keys);
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
            onda++;
            GameManager.Instancia.AtualizarOnda(onda);
            Invoke(nameof(MontarOnda), 1.5f); // pequena pausa antes da próxima onda
        }
    }
}
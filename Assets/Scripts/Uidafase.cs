using UnityEngine;
<<<<<<< HEAD

public class Uidafase : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
=======
using TMPro;

// Coloque este script no Canvas de CADA cena de fase (Fase1, Fase2... Fase10).
// Ele conecta os textos/painéis daquela cena no GameManager assim que ela carrega —
// necessário porque o GameManager sobrevive entre cenas, mas a UI de cada cena não.
public class UIDaFase : MonoBehaviour
{
    [Header("UI desta cena")]
    [SerializeField] private TMP_Text textoPontos;
    [SerializeField] private TMP_Text textoFase;
    [SerializeField] private TMP_Text textoVidaJogador;
    [SerializeField] private TMP_Text textoVidaBoss; // deixe vazio em cenas sem boss
    [SerializeField] private GameObject painelFimDeJogo;
    [SerializeField] private GameObject painelVitoria; // deixe vazio, exceto na cena da Fase 10

    private void Start()
    {
        GameManager.Instancia?.RegistrarUI(textoPontos, textoFase, textoVidaJogador, textoVidaBoss, painelFimDeJogo, painelVitoria);
>>>>>>> 78ff94292a5dd240b8cfdbf52ec1862bb2033a08
    }
}

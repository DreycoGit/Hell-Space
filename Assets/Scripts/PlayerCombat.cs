using UnityEngine;

// Guarda buffs temporários de combate do jogador (hoje: multiplicador de dano do power-up).
public class PlayerCombat : MonoBehaviour
{
    public float MultiplicadorDeDano { get; private set; } = 1f;

    public void AtivarMaisDano(float multiplicador, float duracao)
    {
        StartCoroutine(RotinaDeBuff(multiplicador, duracao));
    }

    private System.Collections.IEnumerator RotinaDeBuff(float multiplicador, float duracao)
    {
        MultiplicadorDeDano = multiplicador;
        yield return new WaitForSeconds(duracao);
        MultiplicadorDeDano = 1f;
    }
}

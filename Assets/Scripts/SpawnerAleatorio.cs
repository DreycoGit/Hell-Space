/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerAleatorio : MonoBehaviour
{
    private readonly List<GameObject> vivos = new List<GameObject>();
    private Coroutine rotina;

    public void Iniciar(ConfiguracaoDeFase fase, System.Action aoTerminar)
    {
        if (rotina != null) StopCoroutine(rotina);
        vivos.Clear();
        rotina = StartCoroutine(Rotina(fase, aoTerminar));
    }

    private List<GameObject> MontarFila(ConfiguracaoDeFase fase)
    {
        List<GameObject> fila = new List<GameObject>();

        if (fase.inimigosAleatorios != null)
        {
            foreach (InimigoNaFase item in fase.inimigosAleatorios)
            {
                if (item.prefab == null) continue;
                for (int i = 0; i < item.quantidade; i++)
                    fila.Add(item.prefab);
            }
        }

        for (int i = fila.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = fila[i];
            fila[i] = fila[j];
            fila[j] = temp;
        }
        return fila;
    }

    private IEnumerator Rotina(ConfiguracaoDeFase fase, System.Action aoTerminar)
    {
        List<GameObject> fila = MontarFila(fase);
        if (fila.Count == 0)
            Debug.LogWarning("Fase '" + fase.nome + "' sem inimigos configurados em 'Inimigos Aleatorios'.");

        yield return new WaitForSeconds(1f);

        int indice = 0;
        while (indice < fila.Count)
        {
            int nestaOnda = Mathf.Min(fase.inimigosPorOnda, fila.Count - indice);
            for (int i = 0; i < nestaOnda; i++)
            {
                vivos.Add(Instantiate(fila[indice], EscolherPosicao(fase), Quaternion.identity));
                indice++;
            }
            yield return new WaitForSeconds(fase.intervaloEntreOndas);
        }

        while (true)
        {
            vivos.RemoveAll(e => e == null);
            if (vivos.Count == 0) break;
            yield return new WaitForSeconds(0.25f);
        }

        aoTerminar?.Invoke();
    }

    private Vector3 EscolherPosicao(ConfiguracaoDeFase fase)
    {
        vivos.RemoveAll(e => e == null);
        Vector3 pos = Vector3.zero;

        for (int t = 0; t < 15; t++)
        {
            pos = new Vector3(
                Random.Range(fase.areaMin.x, fase.areaMax.x),
                Random.Range(fase.areaMin.y, fase.areaMax.y),
                0f);

            bool livre = true;
            foreach (GameObject e in vivos)
            {
                if (Vector2.Distance(e.transform.position, pos) < fase.distanciaMinima)
                {
                    livre = false;
                    break;
                }
            }
            if (livre) return pos;
        }
        return pos;
    }
}*/
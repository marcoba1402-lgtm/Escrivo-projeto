using UnityEngine;
using UnityEngine.SceneManagement; // Biblioteca necessária para carregar cenas

public class Tutorialinicio : MonoBehaviour
{
    // Esta função precisa ser 'public' para aparecer nas opções do botão no Unity
    public void IrParaCenaTemas()
    {
        // Certifique-se de que a cena "Temas" foi adicionada ao Build Settings
        SceneManager.LoadScene("Temas");
    }

    void Start()
    {
        // Código de inicialização (se precisar)
    }

    void Update()
    {
        // Código de atualização por frame (se precisar)
    }
}
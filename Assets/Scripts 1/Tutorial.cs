using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorTutorial : MonoBehaviour
{
    // Esta função será chamada quando o botão for clicado
    public void IniciarJogo()
    {
        // Certifique-se de que o nome da cena seja EXATAMENTE igual ao que está no seu projeto
        SceneManager.LoadScene("3_Cena2");
    }
}
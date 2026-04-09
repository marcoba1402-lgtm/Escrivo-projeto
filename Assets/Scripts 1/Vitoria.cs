using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaVitoria : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somVitoria;

    void Start()
    {
        if (audioSource != null && somVitoria != null)
            audioSource.PlayOneShot(somVitoria);
    }

    public void JogarNovamente()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void VoltarTemas()
    {
        SceneManager.LoadScene("Temas");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaDerrota : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip somDerrota;

    void Start()
    {
        if (audioSource != null && somDerrota != null)
            audioSource.PlayOneShot(somDerrota);
    }

    public void JogarNovamente()
    {
        SceneManager.LoadScene("3_Cena2");
    }

    public void VoltarTemas()
    {
        SceneManager.LoadScene("Temas");
    }
}
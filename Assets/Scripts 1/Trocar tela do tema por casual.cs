using UnityEngine;
using UnityEngine.SceneManagement;

public class Trocarteladotemaporcasual : MonoBehaviour
{
    public void IrParaCasual()
    {
        SceneManager.LoadScene("Casual");
    }

    public void VoltarParaTemas()
    {
        SceneManager.LoadScene("Temas");
    }

    public void IrParaJogo()
    {
        SceneManager.LoadScene("Jogo");
    }

    public void BluetoothManager()
    {
        SceneManager.LoadScene("2_BluetoothLowEnergyExample");
    }

    public void EscolherFacil()
    {
        PlayerPrefs.SetString("dificuldade", "facil");
        PlayerPrefs.Save();
    }

    public void EscolherMedio()
    {
        PlayerPrefs.SetString("dificuldade", "medio");
        PlayerPrefs.Save();
    }

    public void EscolherDificil()
    {
        PlayerPrefs.SetString("dificuldade", "dificil");
        PlayerPrefs.Save();
    }
}
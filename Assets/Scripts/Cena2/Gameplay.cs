using TMPro;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public ControlesCenaDois controlesCenaDois;



    [Header("Configuração do Jogo")]
    public string palavraCorreta = "ARDUINO"; // Coloque a resposta certa aqui

    [SerializeField] TextMeshProUGUI textoDica; // Arraste seu texto de dica aqui


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PalavraVinda(string palavraVinda)
    {
        textoDica.text = "...";
        // 2. Faz a comparação NA HORA
        if (palavraVinda == palavraCorreta.ToUpper())
        {
            // ACERTOU
            textoDica.text = "VOCÊ ACERTOU!";
            textoDica.color = Color.green;

            controlesCenaDois.Enviar("CERTO\n"); // Manda o comando pro LED Verde
        }
        else
        {
            // ERROU
            textoDica.text = "ERROU! TENTE DE NOVO";
            textoDica.color = Color.red;

            controlesCenaDois.Enviar("ERRADO\n"); // Manda o comando pro LED Vermelho
        }
    }
}

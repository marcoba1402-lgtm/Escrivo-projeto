using System;
using TMPro;
using UnityEngine;

public class ControlesCenaDois : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI recebidos;

    Action<String> Enviador;

    void Start()
    {
        GameObject gm = GameObject.Find("Comunicacao");
        if (gm != null)
        {
            GerenciarComunicacao gc = gm.GetComponent<GerenciarComunicacao>();
            gc.RegistraRecebedor(Receber);
            Enviador = gc.Enviar;
        }
    }

    public void Receber(string[] dados)
    {
        // 1. Pega a palavra e limpa (tira o \n que vem do println do Arduino)
        string palavraVinda = dados[0].Trim().ToUpper();
        recebidos.text = palavraVinda;

        // 2. Procura o script do Jogo na cena
        JogoController jogo = FindFirstObjectByType<JogoController>();

        if (jogo != null)
        {
            // Injeta a palavra no input e manda verificar
            jogo.inputField.text = palavraVinda;
            jogo.VerificarResposta();

            // 3. Verifica o resultado para mandar pro LED do Arduino
            // Se o texto de dica mudou para "ACERTOU!"
            if (jogo.textDica.text.Contains("ACERTOU"))
            {
                Enviar("CERTO"); // Liga LED Verde (Pino 12)
            }
            else if (jogo.textDica.text.Contains("ERROU"))
            {
                Enviar("ERRADO"); // Liga LED Vermelho (Pino 13)
            }
        }
    }

    // ESSA FUNÇÃO PRECISA EXISTIR PARA O "RECEBER" ACIMA FUNCIONAR
    public void Enviar(string dados)
    {
        if (Enviador != null)
        {
            Enviador(dados);
        }
    }
}
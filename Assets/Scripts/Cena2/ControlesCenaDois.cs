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

    // ISSO AQUI AGORA É AUTOMÁTICO
    public void Receber(string[] dados)
    {
        // 1. Pega a palavra que você digitou no teclado do Arduino
        string palavraVinda = dados[0].Trim().ToUpper();
        recebidos.text = palavraVinda;

        // 2. Acha o script do jogo na cena
        JogoController jogoAtivo = FindFirstObjectByType<JogoController>();

        if (jogoAtivo != null)
        {
            // 3. Injeta a palavra no campo de texto do jogo
            jogoAtivo.inputField.text = palavraVinda;

            // 4. Manda o jogo conferir a resposta (computar)
            jogoAtivo.VerificarResposta();

            // 5. CHECAGEM AUTOMÁTICA DO RESULTADO
            // O JogoController muda o texto da Dica quando você acerta ou erra.
            // Vamos ler esse texto e mandar o comando pro Arduino NA HORA.

            if (jogoAtivo.textDica.text.Contains("ACERTOU"))
            {
                Enviar("CERTO\n"); // Manda pro Arduino ligar o Verde
            }
            else if (jogoAtivo.textDica.text.Contains("ERROU"))
            {
                Enviar("ERRADO\n"); // Manda pro Arduino ligar o Vermelho
            }
        }
    }

    public void Enviar(string dados)
    {
        if (Enviador != null)
        {
            Enviador(dados);
        }
    }
}
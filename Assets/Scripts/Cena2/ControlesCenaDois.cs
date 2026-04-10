using System;
using TMPro;
using UnityEngine;

public class ControlesCenaDois : MonoBehaviour
{
    [Header("Configurações de UI")]
    [SerializeField] TextMeshProUGUI recebidos;


    [SerializeField] TextMeshProUGUI textoDica;

    // Ação para enviar dados de volta para o Arduino
    Action<string> Enviador;

    public Gameplay gameplay;

    void Start()
    {
        // Conecta com o script de Bluetooth/Comunicação
        GameObject gm = GameObject.Find("Comunicacao");
        if (gm != null)
        {
            GerenciarComunicacao gc = gm.GetComponent<GerenciarComunicacao>();
            gc.RegistraRecebedor(Receber);
            Enviador = gc.Enviar;
        }
    }

    // Chamado toda vez que você digita no Arduino e envia
    public void Receber(string[] dados)
    {
        // 1. Pega o que veio do Arduino e limpa espaços
        string palavraVinda = dados[0].Trim().ToUpper();
        recebidos.text = palavraVinda;
        textoDica.text = "ok";

        gameplay.PalavraVinda( palavraVinda);
       
    }
   

    // Função que despacha a mensagem para o Bluetooth
    public void Enviar(string comando)
    {
        if (Enviador != null)
        {
            Enviador(comando);
            Debug.Log("Enviado para o Arduino: " + comando);
        }
    }
}
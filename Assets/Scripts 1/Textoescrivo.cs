using System;
using TMPro;
using UnityEngine;

public class Textoescrivo : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputEnviar;

    [SerializeField]
    TextMeshProUGUI recebidos;

    Action<String> Enviador;

    void Start()
   {
    GerenciarComunicacao gc = FindFirstObjectByType<GerenciarComunicacao>();
    
    if (gc == null)
    {
        Debug.LogError("GerenciarComunicacao não encontrado!");
        return;
    }
    
    gc.RegistraRecebedor(Receber);
    Enviador = gc.Enviar;
   }

    void Update()
    {

    }

    public void Receber(string[] dados)
    {
        recebidos.text = dados[0];
    }

    public void Enviar()
    {
        if (inputEnviar != null && !string.IsNullOrEmpty(inputEnviar.text))
        {
            Enviador(inputEnviar.text);
            inputEnviar.text = ""; // Limpa o campo após enviar
        }
    }
}
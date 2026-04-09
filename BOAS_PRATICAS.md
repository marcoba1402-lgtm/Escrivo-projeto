# ?? Boas Práticas - Projeto BLE Unity

## ?? Objetivo

Este documento reúne boas práticas, padrões e convenções usadas neste projeto para manter código limpo, manutenível e profissional.

---

## ?? Convenções de Código

### Nomenclatura

#### Classes e Structs
```csharp
// ? BOM: PascalCase
public class BleManager { }
public struct DadosSensor { }

// ? RUIM
public class bleManager { }
public class ble_manager { }
```

####Métodos e Propriedades
```csharp
// ? BOM: PascalCase
public void ConectarDispositivo() { }
public string NomeDispositivo { get; set; }

// ? RUIM
public void conectar_dispositivo() { }
public string nomeDispositivo { get; set; }
```

#### Variáveis Privadas
```csharp
// ? BOM: camelCase com underscore
private string _deviceUuid;
private bool _isConnected;

// ? RUIM
private string DeviceUuid;
private bool isConnected;
```

#### Campos Serializados
```csharp
// ? BOM: camelCase com underscore
[SerializeField]
private float _scanTime = 10f;

// ? RUIM
[SerializeField]
private float scanTime;
```

#### Constantes
```csharp
// ? BOM: PascalCase ou UPPER_CASE
private const string ServiceUuid = "ffe0";
private const int MAX_PACKET_SIZE = 20;

// ? RUIM
private const string service_uuid = "ffe0";
```

### Organização de Classe

```csharp
public class ExemploOrganizado : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField]
    private Button _meuBotao;
    #endregion

    #region Public Properties
    public bool EstaConectado { get; private set; }
    #endregion

    #region Private Fields
    private string _deviceUuid;
    private float _timer;
    #endregion

 #region Constants
  private const float TIMEOUT = 5f;
    #endregion

    #region Unity Lifecycle
    private void Awake() { }
    private void Start() { }
    private void Update() { }
    private void OnDestroy() { }
    #endregion

    #region Public Methods
    public void Conectar() { }
    #endregion

    #region Private Methods
    private void ValidarConfiguracao() { }
    #endregion

    #region Callbacks
    private void OnConnected(string uuid) { }
    #endregion
}
```

---

## ??? Tratamento de Erros

### Sempre Valide Entradas

```csharp
// ? BOM: Valida antes de usar
public void Conectar(string uuid)
{
    if (string.IsNullOrEmpty(uuid))
    {
    Debug.LogError("? UUID vazio!");
        return;
    }

    if (_isConnected)
    {
        Debug.LogWarning("?? Já conectado!");
        return;
    }

    // Código de conexão...
}

// ? RUIM: Não valida
public void Conectar(string uuid)
{
    // Pode dar NullReferenceException!
    _deviceUuid = uuid;
}
```

### Use Try-Catch para Operações Arriscadas

```csharp
// ? BOM: Protege contra falhas
public void EnviarDados(string dados)
{
    try
    {
        byte[] bytes = Encoding.ASCII.GetBytes(dados);
 // Enviar...
    }
    catch (Exception ex)
    {
        Debug.LogError($"? Erro ao enviar: {ex.Message}");
    }
}

// ? RUIM: Pode crashar o app
public void EnviarDados(string dados)
{
    byte[] bytes = Encoding.ASCII.GetBytes(dados);
    // Se 'dados' for null, CRASH!
}
```

### Verifique Null Antes de Usar

```csharp
// ? BOM: Null-safe
private void AtualizarUI()
{
    if (_textStatus != null)
    {
        _textStatus.text = "Conectado";
    }
}

// ? MELHOR: Null-conditional operator
private void AtualizarUI()
{
    _textStatus?.SetText("Conectado");
}

// ? RUIM: Pode dar NullReferenceException
private void AtualizarUI()
{
    _textStatus.text = "Conectado";
}
```

---

## ?? Documentação de Código

### Use XML Comments

```csharp
/// <summary>
/// Conecta ao dispositivo BLE especificado.
/// </summary>
/// <param name="uuid">UUID único do dispositivo</param>
/// <param name="onSuccess">Callback chamado em caso de sucesso</param>
/// <param name="onFailure">Callback chamado em caso de falha</param>
/// <returns>True se o comando foi enfileirado com sucesso</returns>
/// <exception cref="ArgumentNullException">Se uuid for null</exception>
public bool Conectar(string uuid, Action onSuccess, Action onFailure)
{
    // Implementação...
}
```

### Comente Código Complexo

```csharp
// ? BOM: Explica o "porquê"
private void ProcessarDados(byte[] dados)
{
    // BLE pode enviar dados fragmentados, então acumulamos no buffer
 // até receber um caractere de fim de linha (\n)
    _buffer.Append(Encoding.ASCII.GetString(dados));
    
    if (_buffer.ToString().EndsWith("\n"))
    {
        ProcessarMensagemCompleta(_buffer.ToString());
        _buffer.Clear();
}
}

// ? RUIM: Comenta o óbvio
private void ProcessarDados(byte[] dados)
{
    // Adiciona dados ao buffer
    _buffer.Append(Encoding.ASCII.GetString(dados));
    
    // Verifica se termina com \n
    if (_buffer.ToString().EndsWith("\n"))
    {
        // Processa mensagem
      ProcessarMensagemCompleta(_buffer.ToString());
        // Limpa buffer
     _buffer.Clear();
    }
}
```

---

## ?? Unity Específico

### Use SerializeField em Vez de Public

```csharp
// ? BOM: Encapsula mas permite edição no Inspector
[SerializeField]
private Button _meuBotao;

public Button MeuBotao => _meuBotao;

// ? RUIM: Quebra encapsulamento
public Button MeuBotao;
```

### Tooltips Ajudam Muito

```csharp
[Header("Configuração BLE")]

[SerializeField]
[Tooltip("Tempo máximo de scan em segundos. Valores típicos: 5-15s")]
[Range(5, 30)]
private int _scanTime = 10;

[SerializeField]
[Tooltip("UUID do serviço BLE (formato: 'ffe0' ou 'FFE0')")]
private string _serviceUuid = "ffe0";
```

### DontDestroyOnLoad para Managers

```csharp
// ? BOM: Persiste entre cenas
private void Awake()
{
    if (_instance != null && _instance != this)
    {
    Destroy(gameObject);
        return;
  }

    _instance = this;
    DontDestroyOnLoad(gameObject);
}
```

### Limpe Listeners

```csharp
// ? BOM: Remove listeners ao destruir
private void OnEnable()
{
    _button.onClick.AddListener(OnButtonClick);
}

private void OnDisable()
{
    _button.onClick.RemoveListener(OnButtonClick);
}

// ? RUIM: Memory leak!
private void Start()
{
    _button.onClick.AddListener(OnButtonClick);
    // Nunca remove o listener
}
```

---

## ?? BLE Específico

### Sempre Verifique Tamanho de Pacote

```csharp
// ? BOM: Valida tamanho
public void EnviarDados(string dados)
{
    byte[] bytes = Encoding.ASCII.GetBytes(dados + '\n');
    
    if (bytes.Length > 20)
    {
        Debug.LogWarning($"?? Dados muito grandes: {bytes.Length} bytes! " +
    "Considere usar EnviarDadosFragmentados()");
        return;
    }

    // Enviar...
}

// ? RUIM: Ignora limitação do BLE
public void EnviarDados(string dados)
{
    byte[] bytes = Encoding.ASCII.GetBytes(dados + '\n');
    // Pode enviar 100 bytes e só 20 chegam!
}
```

### Use Callbacks Apropriados

```csharp
// ? BOM: Callbacks claros
public class MeuController : MonoBehaviour
{
    private void IniciarScan()
    {
        DiscoverDevices comando = new DiscoverDevices(
            OnDispositivoEncontrado,
            OnScanCompleto,
     OnErroScan,
     10000
    );
        BleManager.Instance.QueueCommand(comando);
    }

    private void OnDispositivoEncontrado(string uuid, string nome)
    {
      Debug.Log($"Encontrado: {nome}");
    }

    private void OnScanCompleto()
    {
        Debug.Log("Scan completo!");
    }

    private void OnErroScan(string erro)
    {
  Debug.LogError($"Erro: {erro}");
    }
}
```

### Buffer para Dados Fragmentados

```csharp
// ? BOM: Acumula dados fragmentados
private StringBuilder _bufferRecepcao = new StringBuilder();

private void OnDadosRecebidos(byte[] dados)
{
    string texto = Encoding.ASCII.GetString(dados);
    _bufferRecepcao.Append(texto);
    
    // Processa quando tiver mensagem completa
    string buffer = _bufferRecepcao.ToString();
    if (buffer.Contains("\n"))
    {
        string mensagem = buffer.Substring(0, buffer.IndexOf("\n"));
        ProcessarMensagem(mensagem);
  
  // Remove mensagem processada do buffer
        _bufferRecepcao.Clear();
        _bufferRecepcao.Append(buffer.Substring(buffer.IndexOf("\n") + 1));
    }
}

// ? RUIM: Assume que dados chegam completos
private void OnDadosRecebidos(byte[] dados)
{
    string texto = Encoding.ASCII.GetString(dados);
    // Pode processar dados incompletos!
    ProcessarMensagem(texto);
}
```

---

## ?? Debug e Logging

### Logs Informativos

```csharp
// ? BOM: Logs claros com emojis e contexto
Debug.Log($"? Conectado ao dispositivo: {nome} ({uuid})");
Debug.LogWarning($"?? Scan timeout após {scanTime}s. Dispositivo '{nome}' não encontrado.");
Debug.LogError($"? Erro ao enviar dados: {ex.Message}\nStack: {ex.StackTrace}");

// ? RUIM: Logs vagos
Debug.Log("Connected");
Debug.Log("Error");
```

### Use Modo Debug

```csharp
// ? BOM: Debug configurável
[SerializeField]
private bool _modoDebug = true;

private void LogDebug(string mensagem)
{
    if (_modoDebug)
    {
        Debug.Log($"[{GetType().Name}] {mensagem}");
 }
}

// Uso
LogDebug("Iniciando conexão...");
```

### Estatísticas Úteis

```csharp
// ? BOM: Rastreie métricas importantes
public class Estatisticas
{
    public int TotalConexoes;
    public int TotalDesconexoes;
    public int MensagensEnviadas;
    public int MensagensRecebidas;
    public DateTime UltimaConexao;
    public TimeSpan TempoConectadoTotal;
    
public override string ToString()
    {
        return $"Estatísticas BLE:\n" +
           $"  Conexões: {TotalConexoes}\n" +
      $"  Mensagens TX: {MensagensEnviadas}\n" +
        $"  Mensagens RX: {MensagensRecebidas}\n" +
            $"  Uptime: {TempoConectadoTotal:hh\\:mm\\:ss}";
    }
}
```

---

## ? Performance

### Evite FindObjectOfType em Update

```csharp
// ? BOM: Cache referências
private GerenciarComunicacao _comunicacao;

private void Start()
{
  _comunicacao = FindFirstObjectByType<GerenciarComunicacao>();
}

private void Update()
{
    _comunicacao?.ProcessarDados();
}

// ? RUIM: Busca todo frame (muito lento!)
private void Update()
{
    FindFirstObjectByType<GerenciarComunicacao>()?.ProcessarDados();
}
```

### Use StringBuilder para Concatenação

```csharp
// ? BOM: Eficiente
private StringBuilder _builder = new StringBuilder();

private string ConstruirMensagem(string[] partes)
{
  _builder.Clear();
    for (int i = 0; i < partes.Length; i++)
    {
_builder.Append(partes[i]);
  if (i < partes.Length - 1)
    _builder.Append(";");
    }
    return _builder.ToString();
}

// ? RUIM: Cria muitas strings temporárias
private string ConstruirMensagem(string[] partes)
{
    string resultado = "";
    for (int i = 0; i < partes.Length; i++)
    {
        resultado += partes[i];
        if (i < partes.Length - 1)
   resultado += ";";
    }
    return resultado;
}
```

### Evite Alocações em Update

```csharp
// ? BOM: Reutiliza arrays
private byte[] _bufferBytes = new byte[20];

private void Update()
{
    if (TemDados())
    {
        int bytesLidos = LerDados(_bufferBytes);
        ProcessarBytes(_bufferBytes, bytesLidos);
    }
}

// ? RUIM: Aloca todo frame (GC Pressure!)
private void Update()
{
    if (TemDados())
 {
        byte[] buffer = new byte[20]; // Aloca novo array!
        int bytesLidos = LerDados(buffer);
        ProcessarBytes(buffer, bytesLidos);
    }
}
```

---

## ?? UI/UX

### Feedback ao Usuário

```csharp
// ? BOM: Sempre dê feedback
public void Conectar()
{
    // Mostra que está processando
    _statusText.text = "?? Conectando...";
    _connectButton.interactable = false;
    
    ConnectToDevice comando = new ConnectToDevice(
        _uuid,
        (uuid) => {
      // Sucesso
            _statusText.text = "? Conectado!";
   _statusText.color = Color.green;
_connectButton.interactable = true;
  },
        (uuid) => {
            // Falha
            _statusText.text = "? Falha na conexão";
            _statusText.color = Color.red;
      _connectButton.interactable = true;
        }
    );
    
    BleManager.Instance.QueueCommand(comando);
}
```

### Previna Duplo Clique

```csharp
// ? BOM: Desabilita botão durante operação
public void OnBotaoConectar()
{
    if (_isOperating) return;
    
    _isOperating = true;
    _button.interactable = false;
    
    Conectar(() => {
        _isOperating = false;
        _button.interactable = true;
    });
}
```

---

## ?? Checklist Final

Antes de enviar código, verifique:

- [ ] ? Nomes seguem convenções (PascalCase, camelCase)
- [ ] ? Classes organizadas com #regions
- [ ] ? Todos os campos públicos têm Tooltip
- [ ] ? Validação de entradas (null, empty, range)
- [ ] ? Try-catch em operações arriscadas
- [ ] ? Logs informativos com emojis
- [ ] ? Listeners removidos no OnDestroy
- [ ] ? Referências cacheadas (não FindObject em Update)
- [ ] ? Comentários explicam o "porquê", não o "o que"
- [ ] ? Tamanho de pacote BLE respeitado (?20 bytes)
- [ ] ? Callbacks para feedback ao usuário
- [ ] ? Sem warnings de compilação
- [ ] ? Testado em dispositivo Android real

---

## ?? Recursos Adicionais

### Aprender Mais

- **C# Coding Conventions**: [Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Unity Best Practices**: [Unity Manual](https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity.html)
- **Clean Code**: Livro de Robert C. Martin
- **Refactoring**: Livro de Martin Fowler

### Ferramentas Úteis

- **ReSharper**: Análise de código C#
- **SonarLint**: Detecta code smells
- **Unity Profiler**: Análise de performance
- **Android Logcat**: Debug em dispositivo

---

**Código limpo é código profissional! ??**

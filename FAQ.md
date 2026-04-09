# ? FAQ - Perguntas Frequentes

## Questões Gerais

### O que é BLE?
**BLE (Bluetooth Low Energy)** é uma versão de baixo consumo do Bluetooth, ideal para dispositivos IoT que precisam economizar bateria. Diferente do Bluetooth Clássico usado para áudio, o BLE é otimizado para pequenas transferências de dados.

### Por que usar BLE em vez de WiFi?
- ? **Menor consumo de energia**
- ? **Não precisa de rede WiFi**
- ? **Conexão direta dispositivo-a-dispositivo**
- ? **Mais simples de configurar**
- ? Menor alcance (10-50m vs 100m+)
- ? Menor velocidade de dados

### Qual Arduino/ESP32 usar?
- **ESP32**: ? Recomendado! BLE integrado, WiFi incluído, mais barato
- **Arduino Nano 33 BLE**: ? Bom, mas mais caro
- **Arduino Uno/Mega + módulo HM-10**: ?? Funciona, mais complexo

---

## Problemas Comuns

### ? "Permission Denied" no Android

**Causa**: App não tem permissões necessárias.

**Solução**:
1. Vá em **Configurações** do Android
2. **Apps** > Seu App > **Permissões**
3. Ative:
   - ?? **Localização** (necessário para BLE no Android!)
   - ?? **Dispositivos próximos** (Android 12+)
4. Se não resolver, **desinstale e reinstale** o app
5. Aceite TODAS as permissões quando pedir

**Por que localização é necessária?**  
O Android requer permissão de localização para escanear dispositivos BLE por questões de privacidade (desde Android 6).

---

### ? "Device not found" ao fazer Scan

**Causas Possíveis**:

#### 1. ESP32 não está transmitindo
```cpp
// Verifique no Serial Monitor do Arduino
void setup() {
  Serial.begin(115200);
  // ...
  Serial.println("BLE iniciado!"); // ? Esta mensagem deve aparecer
}
```

**Solução**: Verifique se o código foi carregado corretamente e o ESP32 está ligado.

#### 2. Nome do dispositivo incorreto
```csharp
// No Unity, o nome deve ser EXATAMENTE igual
String nomeBLE = "ESP32_BLE";  // Arduino
string procurar = "ESP32_BLE"; // Unity (case-sensitive!)
```

**Solução**: Copie e cole o nome para evitar erros de digitação.

#### 3. Bluetooth desligado
**Solução**: Ative o Bluetooth nas configurações do Android.

#### 4. Dispositivo já conectado a outro app
**Solução**: 
- Feche outros apps BLE
- Reinicie o Bluetooth
- Reinicie o ESP32

#### 5. Alcance muito grande
**Solução**: Aproxime o celular do ESP32 (máx. 5m para teste).

---

### ? "Connection failed" ou desconecta rapidamente

**Causas Possíveis**:

#### 1. Interferência
- Muitos dispositivos Bluetooth/WiFi próximos
- Microondas ligado
- Paredes/obstáculos

**Solução**: Teste em ambiente com menos interferência.

#### 2. Alimentação instável do ESP32
**Solução**: 
- Use cabo USB de qualidade
- Use fonte de alimentação externa (5V)
- Adicione capacitor de 100µF entre VCC e GND

#### 3. Intervalo de conexão muito curto
```cpp
// No ESP32, aumente o intervalo
BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
pAdvertising->setMinInterval(0x20); // Padrão
pAdvertising->setMaxInterval(0x40); // Padrão
// Tente valores maiores se desconectar muito
```

---

### ? Dados chegam corrompidos ou incompletos

**Causa**: Excedeu limite de 20 bytes ou dados fragmentados.

**Solução 1**: Reduza tamanho dos dados
```cpp
// ? RUIM: 25 bytes (vai corromper!)
String dados = "TEMP:25.5;UMID:60.2;LUZ:750";

// ? BOM: 10 bytes
String dados = "25.5;60.2";
```

**Solução 2**: Use códigos curtos
```cpp
// ? RUIM: 18 bytes
String dados = "TEMPERATURA:25.5";

// ? BOM: 6 bytes
String dados = "T:25.5";
```

**Solução 3**: Fragmente dados grandes
```csharp
// No Unity
gc.EnviarDadosGrandes("Mensagem muito longa aqui...", 0.1f);
```

**Solução 4**: Buffer no receptor
```csharp
// Já implementado no GerenciarComunicacao.cs
// Acumula fragmentos até receber \n
```

---

### ? App crashando no Android

**Causas Possíveis**:

#### 1. NullReferenceException
```csharp
// ? RUIM
_meuBotao.onClick.AddListener(Clicar);

// ? BOM
if (_meuBotao != null)
{
  _meuBotao.onClick.AddListener(Clicar);
}
```

#### 2. Build Settings incorretos
**Solução**:
- Minimum API Level: **Android 12 (API 31)**
- Target API Level: **33+**
- IL2CPP backend (recomendado para performance)

#### 3. Permissões faltando no AndroidManifest
Verifique se tem:
```xml
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

---

### ? Input System não funciona

**Causa**: Ainda está usando Input antigo ou não reiniciou o Unity.

**Solução**:
1. Verifique `ProjectSettings/ProjectSettings.asset`:
   ```
   activeInputHandler: 1  # (Novo Input System)
   ```
2. Reinicie o Unity quando pedir
3. Use novo código:
   ```csharp
   // ? ANTIGO
   if (Input.GetMouseButtonDown(0)) { }
   
 // ? NOVO
   using UnityEngine.InputSystem;
   if (Mouse.current.leftButton.wasPressedThisFrame) { }
   ```

**Veja também**: [README_INPUT_SYSTEM.md](README_INPUT_SYSTEM.md)

---

## Desenvolvimento

### Como adicionar um novo sensor?

**Passo 1**: Hardware (ESP32)
```cpp
#include <DHT.h>

#define DHTPIN 4
#define DHTTYPE DHT22
DHT dht(DHTPIN, DHTTYPE);

void setup() {
  // ...
  dht.begin();
}

void loop() {
  if (deviceConnected) {
    float temp = dht.readTemperature();
    float umid = dht.readHumidity();
    float pressao = lerPressao(); // Seu sensor
    
    // Formato: temp;umid;pressao
    String dados = String(temp, 1) + ";" + 
  String(umid, 1) + ";" + 
  String(pressao, 0);
    
    // Verifica tamanho (MAX 20 bytes!)
    if (dados.length() < 20) {
pCharacteristic->setValue(dados.c_str());
  pCharacteristic->notify();
    }
    
    delay(2000);
  }
}
```

**Passo 2**: Software (Unity)
```csharp
public class ControleDados : MonoBehaviour
{
    void Start()
    {
        GerenciarComunicacao gc = FindObjectOfType<GerenciarComunicacao>();
  gc.RegistraRecebedor(OnDadosRecebidos);
    }
    
    void OnDadosRecebidos(string[] dados)
    {
        if (dados.Length >= 3)
        {
            float temp = float.Parse(dados[0]);
float umid = float.Parse(dados[1]);
            float pressao = float.Parse(dados[2]);
            
    AtualizarUI(temp, umid, pressao);
        }
    }
}
```

---

### Como mudar os UUIDs do serviço?

**?? Importante**: UUIDs devem ser iguais no ESP32 e Unity!

**ESP32**:
```cpp
#define SERVICE_UUID  "0000ffe0-0000-1000-8000-00805f9b34fb"
#define CHARACTERISTIC_UUID "0000ffe1-0000-1000-8000-00805f9b34fb"
```

**Unity** (`GerenciarComunicacao.cs`):
```csharp
[SerializeField]
private string _servico = "ffe0";  // ? Últimos 4 dígitos

[SerializeField]
private string _caracteristica = "ffe1";  // ? Últimos 4 dígitos
```

**Dica**: Use UUIDs padrão se possível. Se precisar customizar, use um [gerador de UUID](https://www.uuidgenerator.net/).

---

### Como implementar reconexão automática?

```csharp
public class ReconexaoAutomatica : MonoBehaviour
{
    [SerializeField]
 private float _intervaloReconexao = 5f;
    
    private string _ultimoDispositivo;
    private float _timer;
    private bool _tentandoReconectar;
    
    public void OnDesconectado(string uuid)
    {
_ultimoDispositivo = uuid;
        _tentandoReconectar = true;
        _timer = 0f;
        
        Debug.Log("Iniciando tentativas de reconexão...");
    }
    
    private void Update()
    {
    if (_tentandoReconectar)
        {
    _timer += Time.deltaTime;
            
            if (_timer >= _intervaloReconexao)
          {
          _timer = 0f;
                TentarReconectar();
    }
        }
    }
    
    private void TentarReconectar()
    {
        Debug.Log("Tentando reconectar...");
        
        ConnectToDevice cmd = new ConnectToDevice(
         _ultimoDispositivo,
            OnReconectado,
        OnFalhaReconexao
        );
        
        BleManager.Instance.QueueCommand(cmd);
    }
    
    private void OnReconectado(string uuid)
    {
      _tentandoReconectar = false;
        Debug.Log("Reconectado com sucesso!");
    }
    
    private void OnFalhaReconexao(string uuid)
{
        Debug.Log("Falha na reconexão. Tentando novamente...");
        // _tentandoReconectar permanece true
    }
}
```

---

### Como salvar dados localmente?

**Opção 1**: PlayerPrefs (pequenos dados)
```csharp
// Salvar
PlayerPrefs.SetFloat("ultima_temperatura", 25.5f);
PlayerPrefs.SetString("ultimo_dispositivo", "ESP32_01");
PlayerPrefs.Save();

// Carregar
float temp = PlayerPrefs.GetFloat("ultima_temperatura", 0f);
string device = PlayerPrefs.GetString("ultimo_dispositivo", "");
```

**Opção 2**: JSON (estruturado)
```csharp
using System.IO;
using UnityEngine;

[System.Serializable]
public class DadosSalvos
{
    public float temperatura;
 public float umidade;
    public string timestamp;
}

public class SalvarDados : MonoBehaviour
{
    public void Salvar(float temp, float umid)
    {
        DadosSalvos dados = new DadosSalvos
     {
            temperatura = temp,
            umidade = umid,
            timestamp = System.DateTime.Now.ToString()
      };
   
        string json = JsonUtility.ToJson(dados, true);
 string caminho = Application.persistentDataPath + "/dados.json";
      File.WriteAllText(caminho, json);
    
        Debug.Log($"Salvo em: {caminho}");
    }
    
    public DadosSalvos Carregar()
    {
      string caminho = Application.persistentDataPath + "/dados.json";
        
     if (File.Exists(caminho))
     {
 string json = File.ReadAllText(caminho);
            return JsonUtility.FromJson<DadosSalvos>(json);
        }
 
     return null;
    }
}
```

**Opção 3**: CSV (para exportar)
```csharp
using System.Text;

public class LogCSV : MonoBehaviour
{
    private string _arquivoCSV;
    
    void Start()
    {
        _arquivoCSV = Application.persistentDataPath + "/log.csv";
 
        // Cria header se arquivo não existe
        if (!File.Exists(_arquivoCSV))
        {
      File.WriteAllText(_arquivoCSV, "Timestamp,Temperatura,Umidade\n");
        }
    }
    
    public void LogDado(float temp, float umid)
  {
        string linha = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{temp},{umid}\n";
        File.AppendAllText(_arquivoCSV, linha);
    }
    
    public void CompartilharCSV()
    {
        // Usar plugin Android Native Share ou similar
        Debug.Log($"Arquivo em: {_arquivoCSV}");
    }
}
```

---

## Performance

### App está lento / travando

**Verificações**:

1. **Logs excessivos?**
   ```csharp
   [SerializeField]
   private bool _modoDebug = false; // ? Desative em produção
   ```

2. **FindObject em Update?**
   ```csharp
   // ? RUIM
   void Update() {
   FindObjectOfType<BleManager>().Update();
   }
   
   // ? BOM
   private BleManager _ble;
 void Start() {
       _ble = FindObjectOfType<BleManager>();
   }
   void Update() {
       _ble.Update();
   }
   ```

3. **Muitas alocações?**
   ```csharp
   // Use Unity Profiler para identificar:
   // Window > Analysis > Profiler
   ```

4. **Build em Debug mode?**
   - Build Settings > Development Build: **Desmarque**
   - Scripting Backend: **IL2CPP** (mais rápido)

---

## Android Específico

### Como ver logs do Android?

**Método 1**: Unity Logcat (mais fácil)
1. Window > Analysis > Android Logcat
2. Conecte dispositivo via USB
3. Rode o app
4. Veja logs em tempo real

**Método 2**: ADB (linha de comando)
```bash
# Instale Android SDK Platform Tools
# https://developer.android.com/studio/releases/platform-tools

# Liste dispositivos
adb devices

# Ver logs
adb logcat -s Unity

# Ver apenas erros
adb logcat *:E

# Salvar logs em arquivo
adb logcat > logs.txt
```

### Como debugar no dispositivo?

1. **Build Settings**:
   - ? Development Build
   - ? Script Debugging
   - ? Wait for Managed Debugger

2. **Build and Run**

3. **Attach to Unity Debugger**:
   - Unity: Window > Analysis > Debugger
   - Selecione seu dispositivo

4. **Coloque breakpoints** no código

---

## Próximos Passos

### Já domino o básico, e agora?

**Nível Intermediário**:
- [ ] Adicionar múltiplos sensores
- [ ] Criar interface gráfica bonita
- [ ] Implementar gráficos de dados
- [ ] Salvar histórico de dados
- [ ] Adicionar sistema de alertas

**Nível Avançado**:
- [ ] Conectar múltiplos dispositivos
- [ ] Implementar OTA (Over-The-Air updates) no ESP32
- [ ] Criar servidor cloud para armazenar dados
- [ ] Machine Learning para análise de padrões
- [ ] Publicar app na Play Store

---

## Recursos Úteis

### Documentação
- [Unity Manual](https://docs.unity3d.com/Manual/index.html)
- [BLE Specification](https://www.bluetooth.com/specifications/specs/)
- [ESP32 Docs](https://docs.espressif.com/projects/esp-idf/en/latest/esp32/)

### Comunidades
- [Unity Forum](https://forum.unity.com/)
- [Arduino Forum](https://forum.arduino.cc/)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/bluetooth-lowenergy)
- [Reddit r/Unity3D](https://www.reddit.com/r/Unity3D/)
- [Reddit r/esp32](https://www.reddit.com/r/esp32/)

### Ferramentas
- **nRF Connect** (Android/iOS): App para testar BLE
- **Serial Bluetooth Terminal** (Android): Terminal BLE
- **BLE Scanner** (Android): Ver dispositivos BLE próximos
- **Arduino IDE**: Programar ESP32
- **VS Code + PlatformIO**: Alternativa ao Arduino IDE

---

## Ainda com dúvidas?

1. ? Releia a documentação relevante
2. ? Verifique os logs de erro
3. ? Procure no FAQ acima
4. ? Pergunte ao professor/monitor
5. ? Pesquise no Stack Overflow
6. ? Pergunte no fórum Unity

**Lembre-se**: Toda dúvida é válida! ??

---

**FAQ atualizado em**: 2024  
**Versão do projeto**: 4.0

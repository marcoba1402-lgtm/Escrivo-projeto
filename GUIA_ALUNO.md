# ?? Guia do Aluno - Projeto BLE Unity

## ?? Bem-vindo!

Este guia vai te ajudar a entender e usar o projeto BLE Unity passo a passo.

---

## ?? O que você vai aprender

1. ? Como funciona comunicação Bluetooth Low Energy
2. ? Como conectar Unity com Arduino/ESP32
3. ? Como enviar e receber dados
4. ? Como trabalhar com permissões Android
5. ? Boas práticas de programação Unity

---

## ?? Pré-requisitos

### Conhecimentos Necessários
- ?? C# básico (variáveis, funções, classes)
- ?? Unity básico (GameObjects, Scripts, Inspector)
- ?? Arduino básico (setup, loop)

### Não sabe algo? Sem problemas!
- [Tutorial C#](https://learn.microsoft.com/pt-br/dotnet/csharp/tour-of-csharp/tutorials/)
- [Tutorial Unity](https://learn.unity.com/tutorial/introducao-ao-unity)
- [Tutorial Arduino](https://www.arduino.cc/en/Tutorial/HomePage)

---

## ?? Primeiros Passos

### Passo 1: Entenda o Fluxo do App

```
[Iniciar App]
    ?
[Verificar Permissões] ? PermissaoBluetooth.cs
    ?
[Scan de Dispositivos] ? ExampleBleInteractor.cs
    ?
[Conectar ao Dispositivo] ? ConnectToDevice
    ?
[Inscrever para Notificações] ? SubscribeToCharacteristic
    ?
[Enviar/Receber Dados] ? GerenciarComunicacao.cs
```

### Passo 2: Abra o Projeto

1. Abra o Unity Hub
2. Clique em **"Open"**
3. Selecione a pasta do projeto
4. Aguarde a importação (pode demorar 5-10 minutos)
5. Se pedir para reiniciar, clique em **"Yes"**

### Passo 3: Explore as Cenas

O projeto tem 3 cenas principais:

#### ?? Cena 1: Verificação de Permissões
- **Arquivo**: `InicialVerificaPermissoes.unity`
- **Script**: `PermissaoBluetooth.cs`
- **O que faz**: Pede permissões de Bluetooth ao usuário

#### ?? Cena 2: Conexão BLE
- **Arquivo**: `CenaBluetooth.unity`
- **Script**: `ExampleBleInteractor.cs`
- **O que faz**: Escaneia e conecta ao dispositivo BLE

#### ?? Cena 3: Comunicação
- **Arquivo**: `CenaDois.unity`
- **Script**: `ControlesCenaDois.cs`
- **O que faz**: Envia e recebe dados do dispositivo

---

## ?? Configurando seu ESP32/Arduino

### Hardware Necessário

- **ESP32** (recomendado) ou Arduino com módulo BLE
- **Cabo USB**
- **(Opcional)** Sensores (DHT22, LM35, etc.)
- **(Opcional)** LEDs, relés, servos

### Código Básico ESP32

Abra a Arduino IDE e cole este código:

```cpp
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>

// UUIDs (não mude se quiser usar com o app padrão)
#define SERVICE_UUID      "0000ffe0-0000-1000-8000-00805f9b34fb"
#define CHARACTERISTIC_UUID "0000ffe1-0000-1000-8000-00805f9b34fb"

BLEServer* pServer = NULL;
BLECharacteristic* pCharacteristic = NULL;
bool deviceConnected = false;
bool oldDeviceConnected = false;

// Callbacks de conexão
class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      deviceConnected = true;
      Serial.println("Cliente conectado!");
    };

    void onDisconnect(BLEServer* pServer) {
      deviceConnected = false;
      Serial.println("Cliente desconectado!");
    }
};

// Callback quando recebe dados
class MyCallbacks: public BLECharacteristicCallbacks {
    void onWrite(BLECharacteristic *pCharacteristic) {
      std::string rxValue = pCharacteristic->getValue();

    if (rxValue.length() > 0) {
        Serial.print("Recebido: ");
        for (int i = 0; i < rxValue.length(); i++) {
          Serial.print(rxValue[i]);
        }
        Serial.println();
        
        // Processe o comando aqui
        String comando = String(rxValue.c_str());
        if (comando == "LED:ON") {
          digitalWrite(LED_BUILTIN, HIGH);
  Serial.println("LED ligado!");
      }
        else if (comando == "LED:OFF") {
          digitalWrite(LED_BUILTIN, LOW);
Serial.println("LED desligado!");
        }
   }
    }
};

void setup() {
  Serial.begin(115200);
  pinMode(LED_BUILTIN, OUTPUT);
  
  // ?? IMPORTANTE: Mude este nome para identificar seu dispositivo
  String nomeBLE = "ESP32_ALUNO"; // ? MUDE AQUI
  
  Serial.println("Iniciando BLE...");
  
  // Inicializa BLE
  BLEDevice::init(nomeBLE.c_str());
  
  // Cria servidor BLE
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  
  // Cria serviço BLE
  BLEService *pService = pServer->createService(SERVICE_UUID);
  
  // Cria característica BLE
  pCharacteristic = pService->createCharacteristic(
    CHARACTERISTIC_UUID,
    BLECharacteristic::PROPERTY_READ   |
    BLECharacteristic::PROPERTY_WRITE  |
    BLECharacteristic::PROPERTY_NOTIFY |
  BLECharacteristic::PROPERTY_INDICATE
  );
  
  // Adiciona descritor 2902 (necessário para notificações)
  pCharacteristic->addDescriptor(new BLE2902());
  pCharacteristic->setCallbacks(new MyCallbacks());
  
  // Inicia o serviço
  pService->start();
  
  // Inicia advertising (anúncio)
  BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pAdvertising->setScanResponse(false);
  pAdvertising->setMinPreferred(0x0);
  BLEDevice::startAdvertising();
  
  Serial.println("BLE pronto! Aguardando conexão...");
  Serial.print("Nome do dispositivo: ");
  Serial.println(nomeBLE);
}

void loop() {
  // Se conectado, envia dados a cada 2 segundos
  if (deviceConnected) {
  // Exemplo: enviar temperatura e umidade simuladas
    float temp = random(20, 30) + random(0, 100) / 100.0;
    float umid = random(40, 80) + random(0, 100) / 100.0;
    
    String dados = String(temp, 1) + ";" + String(umid, 1);
    
    // ?? IMPORTANTE: BLE só envia 20 bytes por vez!
    // Este exemplo envia "25.5;60.2" = 9 bytes (OK!)
    pCharacteristic->setValue(dados.c_str());
  pCharacteristic->notify();
  
    Serial.print("Enviado: ");
    Serial.println(dados);
    
    delay(2000);
  }
  
  // Gerencia reconexão
  if (!deviceConnected && oldDeviceConnected) {
    delay(500);
    pServer->startAdvertising();
    Serial.println("Reiniciando advertising...");
    oldDeviceConnected = deviceConnected;
  }
  
  if (deviceConnected && !oldDeviceConnected) {
    oldDeviceConnected = deviceConnected;
  }
}
```

### Como Carregar no ESP32

1. **Instale a biblioteca ESP32**:
   - Arduino IDE > Ferramentas > Gerenciar Bibliotecas
   - Procure por "ESP32 BLE Arduino"
   - Instale

2. **Selecione a placa**:
   - Ferramentas > Placa > ESP32 Dev Module

3. **Selecione a porta**:
   - Ferramentas > Porta > COM X (seu ESP32)

4. **Carregue o código**:
   - Clique em Upload (?)
   - Aguarde "Hard resetting via RTS pin..."

5. **Abra o Serial Monitor**:
   - Ferramentas > Serial Monitor
   - Baud rate: 115200
   - Você deve ver: "BLE pronto! Aguardando conexão..."

---

## ?? Testando no Android

### Build do Projeto

1. **File > Build Settings**
2. Selecione **Android**
3. Clique em **Switch Platform** (se necessário)
4. **Player Settings**:
 - **Company Name**: Seu nome ou "PUCSP"
   - **Product Name**: "BLE Test"
   - **Other Settings**:
     - **Package Name**: `com.pucsp.bletest`
     - **Minimum API Level**: Android 12.0 (API 31)
     - **Target API Level**: 33 ou superior
5. Conecte seu Android via USB
6. Ative **Depuração USB** no celular
7. Clique em **Build And Run**

### Primeiro Teste

1. **App abre** ? Pede permissões ? Clique em **"Permitir"**
2. **Digite o nome do ESP32** (ex: "ESP32_ALUNO")
3. **Clique em "Scan"**
4. **Aguarde a conexão**
5. **Veja os dados chegando!**

---

## ?? Exercícios Práticos

### ?? Nível 1: Básico

#### Exercício 1.1: Mude o Nome do Dispositivo
**Objetivo**: Identificar seu dispositivo único

1. No código Arduino, mude:
```cpp
String nomeBLE = "ESP32_ALUNO"; // ? Mude para seu nome
```

2. No Unity, abra a cena `CenaBluetooth`
3. Teste conexão com o novo nome

#### Exercício 1.2: Adicione um LED Físico
**Objetivo**: Controlar hardware real

1. Conecte um LED ao pino GPIO 2 do ESP32
2. Adicione ao código Arduino:
```cpp
#define LED_PIN 2

void setup() {
  pinMode(LED_PIN, OUTPUT);
  // ... resto do código
}

// Na função onWrite:
if (comando == "LED:ON") {
  digitalWrite(LED_PIN, HIGH);
}
```

3. No Unity, envie o comando "LED:ON"

#### Exercício 1.3: Modifique os Dados Enviados
**Objetivo**: Entender o formato de dados

1. No Arduino, mude:
```cpp
// Antes: "25.5;60.2"
// Depois: "TEMP:25.5|UMID:60.2"
```

2. No Unity (`GerenciarComunicacao.cs`), adapte o Split:
```csharp
string[] dados = texto.Split('|');
```

---

### ?? Nível 2: Intermediário

#### Exercício 2.1: Adicione um Sensor Real
**Objetivo**: Trabalhar com sensores reais

**Sensor DHT22 (Temperatura e Umidade)**

Arduino:
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
    
    if (!isnan(temp) && !isnan(umid)) {
      String dados = String(temp, 1) + ";" + String(umid, 1);
      pCharacteristic->setValue(dados.c_str());
      pCharacteristic->notify();
    }
    
    delay(2000);
  }
}
```

Unity (`ControlesCenaDois.cs`):
```csharp
public void Receber(string[] dados)
{
    if (dados.Length >= 2)
    {
        float temperatura = float.Parse(dados[0]);
        float umidade = float.Parse(dados[1]);
        
        recebidos.text = $"??? {temperatura:F1}°C\n?? {umidade:F1}%";
        
      // Alertas
        if (temperatura > 30)
        {
         Debug.LogWarning("Temperatura alta!");
      }
    }
}
```

#### Exercício 2.2: Crie um Gráfico de Dados
**Objetivo**: Visualizar dados em tempo real

1. Instale o pacote **XCharts** ou use UI nativa
2. Crie um script que armazena histórico:

```csharp
using System.Collections.Generic;

public class GraficoTemperatura : MonoBehaviour
{
    private List<float> historico = new List<float>();
    private const int maxPontos = 50;
    
    public void AdicionarDado(float temperatura)
    {
historico.Add(temperatura);
        
   if (historico.Count > maxPontos)
        {
  historico.RemoveAt(0);
    }
        
        AtualizarGrafico();
    }
    
    private void AtualizarGrafico()
    {
      // Desenhe o gráfico aqui
    }
}
```

#### Exercício 2.3: Sistema de Alertas
**Objetivo**: Reagir a condições específicas

```csharp
public class SistemaAlertas : MonoBehaviour
{
  [SerializeField] private AudioSource alertaSound;
    [SerializeField] private float temperaturaMaxima = 30f;
    
    public void VerificarTemperatura(float temp)
    {
        if (temp > temperaturaMaxima)
        {
         // Toca som
      alertaSound.Play();
          
   // Muda cor da UI
     temperatureText.color = Color.red;
    
  // Envia notificação
      Debug.LogWarning($"ALERTA: Temperatura {temp}°C!");
       
            // Pode enviar comando de volta para ESP32
          EnviarComando("COOLER:ON");
        }
    }
}
```

---

### ?? Nível 3: Avançado

#### Exercício 3.1: Reconexão Automática
**Objetivo**: App não perde conexão

```csharp
public class GerenciadorConexao : MonoBehaviour
{
    private float tempoReconexao = 5f;
    private float timer = 0f;
    private bool tentandoReconectar = false;
    
    private void Update()
    {
        if (!_isConnected && tentandoReconectar)
   {
        timer += Time.deltaTime;
       
      if (timer >= tempoReconexao)
        {
    timer = 0f;
                TentarReconectar();
     }
        }
    }
    
    private void OnDisconnected(string uuid)
    {
  tentandoReconectar = true;
        Debug.Log("Desconectado! Tentando reconectar...");
    }
    
    private void TentarReconectar()
    {
        Debug.Log("Tentando reconectar...");
        Connect();
    }
}
```

#### Exercício 3.2: Log de Dados com Timestamp
**Objetivo**: Salvar histórico de dados

```csharp
using System;
using System.IO;
using System.Text;

public class LogDados : MonoBehaviour
{
    private string caminhoArquivo;
    
    private void Start()
    {
        string pasta = Application.persistentDataPath;
        string nomeArquivo = $"log_{DateTime.Now:yyyyMMdd}.txt";
        caminhoArquivo = Path.Combine(pasta, nomeArquivo);
 
        Debug.Log($"Salvando em: {caminhoArquivo}");
    }
    
    public void SalvarDado(float temperatura, float umidade)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string linha = $"{timestamp},{temperatura:F1},{umidade:F1}\n";
        
        File.AppendAllText(caminhoArquivo, linha, Encoding.UTF8);
    }
    
    public void ExportarCSV()
    {
    // Implementar exportação para compartilhamento
    }
}
```

#### Exercício 3.3: Controle de Múltiplos Dispositivos
**Objetivo**: Conectar a vários ESP32 simultaneamente

**Desafio**:
1. Modifique o `BleManager` para suportar múltiplas conexões
2. Crie uma UI para listar todos os dispositivos
3. Permita alternar entre dispositivos
4. Envie comandos para dispositivo específico

---

## ?? Problemas Comuns e Soluções

### ? "Permission Denied"
**Causa**: Permissões não concedidas  
**Solução**:
1. Desinstale o app
2. Instale novamente
3. Clique em "Permitir" em TODAS as permissões

### ? "Device not found"
**Causa**: ESP32 não está transmitindo  
**Solução**:
1. Abra Serial Monitor do Arduino
2. Verifique se aparece "BLE pronto!"
3. Reinicie o ESP32
4. Verifique se o nome está correto

### ? "Connection failed"
**Causa**: Interferência ou distância  
**Solução**:
1. Aproxime o celular do ESP32
2. Desative outros dispositivos Bluetooth
3. Reinicie ambos os dispositivos

### ? Dados corrompidos/incompletos
**Causa**: Excedeu 20 bytes  
**Solução**:
1. Conte os caracteres: `"25.5;60.2\n"` = 10 bytes ?
2. Reduza casas decimais
3. Use códigos curtos: `"T:25.5;U:60"` em vez de `"TEMP:25.5;UMID:60"`

---

## ?? Próximos Passos

### Aprofunde seus Conhecimentos

1. **Aprenda mais sobre BLE**:
   - [Bluetooth Low Energy](https://learn.adafruit.com/introduction-to-bluetooth-low-energy)
   
2. **Melhore seu código Unity**:
   - [Unity Learn](https://learn.unity.com/)
   - [C# Intermedi ário](https://learn.microsoft.com/pt-br/dotnet/csharp/)

3. **Explore IoT**:
   - MQTT
   - ThingSpeak
   - Firebase

### Projetos Inspiradores

- ?? **Casa Inteligente**: Controle luzes, temperatura, cortinas
- ?? **Robô BLE**: Controle um robô via celular
- ?? **Horta Inteligente**: Monitore solo, temperatura, luz
- ?? **Fitness Tracker**: Monitore exercícios em tempo real
- ?? **Game Controller**: Use ESP32 como controle de jogo

---

## ?? Precisa de Ajuda?

1. **Revise este guia** ??
2. **Consulte o README principal** ??
3. **Verifique o código de exemplo** ??
4. **Pergunte ao professor/monitor** ?????
5. **Colabore com colegas** ??

---

## ? Checklist de Aprendizado

- [ ] Entendi o fluxo de permissões
- [ ] Consegui fazer scan de dispositivos
- [ ] Consegui conectar ao ESP32
- [ ] Consegui receber dados
- [ ] Consegui enviar comandos
- [ ] Modifiquei o código Arduino
- [ ] Criei meu próprio projeto
- [ ] Implementei um sensor real
- [ ] Tratei erros e desconexões
- [ ] Documentei meu código

---

**Parabéns por chegar até aqui! Continue explorando e criando! ??**

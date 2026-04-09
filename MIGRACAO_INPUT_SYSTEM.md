# Migração para o Novo Input System

## Mudanças Realizadas

### 1. Pacote Instalado
- **Adicionado**: `com.unity.inputsystem` versão `1.11.2` ao `Packages/manifest.json`

### 2. Configuração do Projeto
- **ProjectSettings.asset**: Alterado `activeInputHandler` de `0` (antigo) para `1` (apenas novo Input System)
- Isso desabilita completamente o Input System antigo (Input Manager)

## Como Usar o Novo Input System

### Opção 1: PlayerInput Component (Recomendado para UI e jogos simples)
1. Crie um arquivo `.inputactions` (botão direito > Create > Input Actions)
2. Configure suas ações (ex: "Touch", "Submit", "Cancel")
3. Adicione o componente `PlayerInput` ao seu GameObject
4. Atribua o arquivo `.inputactions` criado

### Opção 2: Input Actions via Código
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ExemploNovoInput : MonoBehaviour
{
    // Criar Actions programaticamente
    private InputAction touchAction;
    
    void Awake()
    {
      // Criar uma ação de toque/clique
        touchAction = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/press");
        touchAction.performed += ctx => OnTouch(ctx);
        touchAction.Enable();
    }
    
    void OnTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = Pointer.current.position.ReadValue();
        Debug.Log($"Tocou em: {touchPosition}");
    }
    
    void OnDestroy()
    {
        touchAction.Disable();
  }
}
```

### Opção 3: Input System Direto (Para Android/Mobile)
```csharp
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class ExemploTouch : MonoBehaviour
{
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    
    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    void Update()
    {
foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
   {
        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
    {
    Debug.Log($"Toque começou em: {touch.screenPosition}");
  }
  }
    }
}
```

## Substituições do Input Antigo

### Antes (Input Manager - ANTIGO ?)
```csharp
// Mouse/Touch Position
Vector3 pos = Input.mousePosition;

// Mouse Button
if (Input.GetMouseButtonDown(0)) { }

// Touch
if (Input.touchCount > 0) {
    Touch touch = Input.GetTouch(0);
}

// Keyboard
if (Input.GetKeyDown(KeyCode.Space)) { }

// Axis
float h = Input.GetAxis("Horizontal");
```

### Depois (Input System - NOVO ?)
```csharp
using UnityEngine.InputSystem;

// Mouse/Touch Position
Vector2 pos = Pointer.current.position.ReadValue();

// Mouse/Touch Press
if (Mouse.current.leftButton.wasPressedThisFrame) { }
// ou
if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame) { }

// Touch Enhanced
using UnityEngine.InputSystem.EnhancedTouch;
EnhancedTouchSupport.Enable();
foreach (var touch in Touch.activeTouches) { }

// Keyboard
if (Keyboard.current.spaceKey.wasPressedThisFrame) { }

// Gamepad
if (Gamepad.current.leftStick.ReadValue()) { }
```

## Próximos Passos

1. **Abra o projeto no Unity** - O Unity irá reimportar os pacotes
2. **Aceite a reinicialização** quando solicitado (necessário para ativar o novo Input System)
3. **Crie seu arquivo .inputactions** ou use Input direto via código
4. **Teste no dispositivo Android** para confirmar funcionamento

## Notas Importantes

- ?? O Input Manager antigo (`Input.GetKey`, `Input.GetAxis`, etc.) **NÃO funcionará mais**
- ? Todo código de input deve usar `UnityEngine.InputSystem`
- ?? Para Android, o novo sistema funciona perfeitamente com touch
- ?? Suporte nativo para múltiplos dispositivos (touch, gamepad, keyboard, etc.)

## Documentação Oficial
- [Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/index.html)
- [Input System Migration Guide](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/Migration.html)

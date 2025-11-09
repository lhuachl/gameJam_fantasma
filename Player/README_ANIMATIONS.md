# Sistema de Animaciones del Jugador

## Descripción
Este sistema de animaciones está diseñado para trabajar con el `PlayerController` y controlar todas las animaciones del personaje del jugador basándose en sus acciones y estado.

## Componentes

### 1. PlayerAnimator.cs
Script principal que controla las animaciones del jugador. Se suscribe a los eventos del `PlayerController` y actualiza los parámetros del Animator.

**Parámetros del Animator:**
- `IsRunning` (bool): El jugador se está moviendo horizontalmente
- `IsGrounded` (bool): El jugador está tocando el suelo
- `IsJumping` (bool): El jugador está subiendo en el salto
- `IsFalling` (bool): El jugador está cayendo
- `OnAir` (bool): El jugador está en el aire (opuesto a IsGrounded)
- `OnDashPressed` (bool): Se activó el dash
- `IsAttacking` (bool): El jugador está atacando
- `IsParrying` (bool): El jugador está parrying
- `OnJump` (trigger): Se activó el salto
- `OnLand` (trigger): El jugador aterrizó
- `OnAttack` (trigger): Se activó el ataque
- `OnParry` (trigger): Se activó el parry
- `OnTakeDamage` (trigger): El jugador recibió daño
- `OnDie` (trigger): El jugador murió

### 2. PlayerAnimationConfig.cs
ScriptableObject para configurar las animaciones del jugador.

### 3. PlayerAnimatorSetup.cs (Editor)
Herramienta de editor para configurar fácilmente el Animator Controller.

## Instalación y Configuración

### Paso 1: Asignar el Animator Controller
1. Selecciona el GameObject del jugador en la escena
2. En el componente Animator, asigna el `PlayerAnimatorController.controller`
3. Asegúrate de que el componente `PlayerAnimator` esté añadido al GameObject

### Paso 2: Configurar las Animaciones
1. Crea un `PlayerAnimationConfig` asset (Click derecho en Project → Create → Player → Animation Config)
2. Asigna los clips de animación correspondientes en el inspector
3. Configura los tiempos de transición y eventos de animación

### Paso 3: Usar la Herramienta de Editor (Opcional)
1. Ve a Tools → Player Animator Setup
2. Asigna el prefab del jugador y el Animation Config
3. Usa los botones para:
   - Setup Animator: Asigna el Animator Controller
   - Create New Animator Controller: Crea un nuevo controller
   - Assign Animation Clips: Asigna los clips al controller

## Estados de Animación

### Estados Base
- **Idle**: Jugador quieto en el suelo
- **Run**: Jugador corriendo en el suelo
- **Jump**: Jugador en la fase ascendente del salto
- **Fall**: Jugador cayendo

### Estados de Acción
- **Dash**: Jugador realizando un dash
- **Attack**: Jugador atacando
- **Parry**: Jugador parrying
- **Hurt**: Jugador recibiendo daño

## Transiciones

### Transiciones Automáticas
- Idle ↔ Run: Basado en `IsRunning`
- Jump ↔ Fall: Basado en `IsJumping`/`IsFalling`
- Grounded ↔ Air: Basado en `IsGrounded`

### Transiciones por Eventos
- Dash: Cualquier estado → Dash (trigger por `OnDashPressed`)
- Attack: Cualquier estado → Attack (trigger por `OnAttack`)
- Parry: Cualquier estado → Parry (trigger por `OnParry`)
- Hurt: Cualquier estado → Hurt (trigger por `OnTakeDamage`)

## Eventos de Animación

Puedes añadir eventos a los clips de animación para sincronizar acciones del juego con la animación:

### Attack Animation
- Evento en el frame donde el ataque debería hacer daño
- Usar el método `PlayerController.OnAttackHit()`

### Dash Animation
- Evento al inicio para activar la invulnerabilidad
- Evento al final para desactivar la invulnerabilidad

## Debugging

Si las animaciones no funcionan correctamente:

1. **Verifica el Animator Controller**: Asegúrate de que esté asignado y tenga los parámetros correctos
2. **Revisa los eventos**: Verifica que el `PlayerAnimator` esté suscrito a los eventos del `PlayerController`
3. **Comprueba las capas**: Asegúrate de que el Layer Mask del suelo esté correctamente configurado
4. **Debug Console**: Mira los mensajes de debug en la consola para verificar que los eventos se están disparando

## Ejemplo de Uso

```csharp
// El sistema funciona automáticamente, pero puedes acceder a él si necesitas:
PlayerAnimator playerAnimator = GetComponent<PlayerAnimator>();

// Forzar una animación específica
playerAnimator.SetParryAnimation(true);

// Trigger manual de animación
playerAnimator.TriggerParry();
```

## Notas Importantes

- El sistema usa eventos para comunicarse con el `PlayerController`, no modifica directamente el estado del jugador
- Las animaciones de dash, attack y parry tienen duración fija y se resetean automáticamente
- El sistema detecta automáticamente el aterrizaje después de un salto
- Las animaciones de daño y muerte son triggers instantáneos
# Estilos y convenciones - ARTour Virtual

## Índice
1. [Sistema de diseño](#sistema-de-diseño)
2. [Tipografía](#tipografía)
3. [Paleta de colores](#paleta-de-colores)
4. [Espaciado y dimensiones](#espaciado-y-dimensiones)
5. [Iconografía](#iconografía)
6. [Animaciones y transiciones](#animaciones-y-transiciones)
7. [Convenciones de nomenclatura](#convenciones-de-nomenclatura)
8. [Responsive design](#responsive-design)

---

## Sistema de diseño

ARTour Virtual implementa un sistema de diseño consistente basado en principios de Material Design adaptados para experiencias de realidad aumentada.

### Principios de diseño

1. **Claridad**: La información debe ser siempre legible y comprensible
2. **Consistencia**: Patrones visuales uniformes en toda la aplicación
3. **Retroalimentación**: Respuesta visual inmediata a cada acción del usuario
4. **Jerarquía**: Importancia visual clara de los elementos
5. **Accesibilidad**: Diseño inclusivo para todos los usuarios

### Estructura de tokens de diseño

```css
/* Tokens de diseño global */
:root {
    /* Colores */
    --color-primary: #1E90FF;
    --color-primary-dark: #1565C0;
    --color-primary-light: #64B5F6;
    
    --color-secondary: #FF9800;
    --color-secondary-dark: #F57C00;
    --color-secondary-light: #FFB74D;
    
    --color-background: #F5F5F5;
    --color-surface: #FFFFFF;
    --color-error: #F44336;
    --color-success: #4CAF50;
    
    --color-text-primary: #333333;
    --color-text-secondary: #666666;
    --color-text-disabled: #999999;
    
    /* Espaciado */
    --space-xs: 4px;
    --space-sm: 8px;
    --space-md: 16px;
    --space-lg: 24px;
    --space-xl: 32px;
    --space-xxl: 48px;
    
    /* Tipografía */
    --font-size-xs: 12px;
    --font-size-sm: 14px;
    --font-size-md: 16px;
    --font-size-lg: 18px;
    --font-size-xl: 24px;
    --font-size-xxl: 30px;
    
    /* Bordes y radios */
    --radius-sm: 8px;
    --radius-md: 12px;
    --radius-lg: 20px;
    --radius-full: 9999px;
    
    /* Sombras */
    --shadow-sm: 0 2px 4px rgba(0, 0, 0, 0.1);
    --shadow-md: 0 4px 8px rgba(0, 0, 0, 0.15);
    --shadow-lg: 0 8px 16px rgba(0, 0, 0, 0.2);
    
    /* Transiciones */
    --transition-fast: 150ms;
    --transition-normal: 300ms;
    --transition-slow: 500ms;
}
```

---

## Tipografía

### Familia tipográfica: Outfit

ARTour utiliza la familia tipográfica **Outfit** de Google Fonts por su legibilidad excelente en pantallas móviles y su carácter moderno que complementa la experiencia tecnológica.

```css
/* Importación de fuentes */
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@400;500;600;700&display=swap');
```

### Jerarquía tipográfica

#### Outfit Bold (700)
**Uso**: Títulos principales, encabezados importantes, CTAs destacados

```css
.titulo-principal {
    font-family: 'Outfit', sans-serif;
    font-weight: 700;
    font-size: 30px;
    line-height: 1.2;
    letter-spacing: -0.5px;
    color: var(--color-text-primary);
}

.titulo-secundario {
    font-family: 'Outfit', sans-serif;
    font-weight: 700;
    font-size: 24px;
    line-height: 1.3;
    letter-spacing: -0.3px;
    color: var(--color-text-primary);
}
```

**Ejemplo de uso:**
- Título "ARTour" en header
- Nombres de salas del museo
- Títulos de obras de arte principales

#### Outfit SemiBold (600)
**Uso**: Subtítulos, labels importantes, texto destacado

```css
.subtitulo {
    font-family: 'Outfit', sans-serif;
    font-weight: 600;
    font-size: 20px;
    line-height: 1.4;
    letter-spacing: -0.2px;
    color: var(--color-text-primary);
}
```

**Ejemplo de uso:**
- Nombres de artistas
- Categorías de recorridos
- Headers de secciones

#### Outfit Medium (500)
**Uso**: Botones, labels de formularios, texto de énfasis medio

```css
.texto-medio {
    font-family: 'Outfit', sans-serif;
    font-weight: 500;
    font-size: 16px;
    line-height: 1.5;
    letter-spacing: 0;
    color: var(--color-text-primary);
}

.boton-texto {
    font-family: 'Outfit', sans-serif;
    font-weight: 500;
    font-size: 18px;
    line-height: 1;
    letter-spacing: 0.3px;
    text-transform: none;
}
```

**Ejemplo de uso:**
- Texto de botones principales
- Labels de estado ("Conectando...", "Ubicación actual")
- Navegación y tabs

#### Outfit Regular (400)
**Uso**: Texto de cuerpo, descripciones, contenido general

```css
.texto-cuerpo {
    font-family: 'Outfit', sans-serif;
    font-weight: 400;
    font-size: 16px;
    line-height: 1.6;
    letter-spacing: 0;
    color: var(--color-text-primary);
}

.texto-descripcion {
    font-family: 'Outfit', sans-serif;
    font-weight: 400;
    font-size: 14px;
    line-height: 1.5;
    letter-spacing: 0;
    color: var(--color-text-secondary);
}
```

**Ejemplo de uso:**
- Descripciones de obras de arte
- Instrucciones al usuario
- Información de ayuda
- Textos informativos del footer

### Escalas responsivas

```css
/* Móvil (default) */
.titulo-responsive {
    font-size: 24px;
}

/* Tablet */
@media (min-width: 768px) {
    .titulo-responsive {
        font-size: 28px;
    }
}

/* Desktop (para testing) */
@media (min-width: 1024px) {
    .titulo-responsive {
        font-size: 32px;
    }
}
```

### Ejemplos de implementación en Unity UI Toolkit

```css
/* EstilosGenerales.uss */
.titulo-pantalla {
    -unity-font: url('/Assets/Fonts/Outfit-Bold.ttf');
    font-size: 30px;
    color: rgb(51, 51, 51);
    -unity-text-align: middle-center;
    margin-bottom: 20px;
}

.texto-normal {
    -unity-font: url('/Assets/Fonts/Outfit-Regular.ttf');
    font-size: 16px;
    color: rgb(51, 51, 51);
    white-space: normal;
}

.boton-principal-texto {
    -unity-font: url('/Assets/Fonts/Outfit-Medium.ttf');
    font-size: 18px;
    color: rgb(255, 255, 255);
    -unity-text-align: middle-center;
}
```

---

## Paleta de colores

### Colores principales

#### Azul ARTour (Primary)
**HEX**: `#1E90FF` | **RGB**: `rgb(30, 144, 255)` | **Unity**: `new Color(0.118f, 0.565f, 1f)`

**Uso**: 
- Botones de acción principal
- Enlaces y elementos interactivos
- Indicadores de progreso
- Highlights y selecciones

```css
.color-primary {
    background-color: #1E90FF;
}

.color-primary-hover {
    background-color: #1C7ED6; /* 10% más oscuro */
}

.color-primary-active {
    background-color: #1565C0; /* 20% más oscuro */
}
```

**Variaciones:**
- Primary Light: `#64B5F6` - Para backgrounds sutiles
- Primary Dark: `#1565C0` - Para textos sobre fondos claros
- Primary Ultra Light: `#E3F2FD` - Para highlights y badges

#### Naranja Secundario (Secondary)
**HEX**: `#FF9800` | **RGB**: `rgb(255, 152, 0)` | **Unity**: `new Color(1f, 0.596f, 0f)`

**Uso**:
- Elementos de alerta positiva
- Badges de información importante
- Iconos de acción secundaria

```css
.color-secondary {
    background-color: #FF9800;
}

.color-secondary-hover {
    background-color: #FB8C00;
}
```

### Colores de sistema

#### Gris de fondo
**HEX**: `#F5F5F5` | **RGB**: `rgb(245, 245, 245)`

**Uso**: Background principal de la aplicación

#### Blanco superficie
**HEX**: `#FFFFFF` | **RGB**: `rgb(255, 255, 255)`

**Uso**: Cards, paneles, elementos elevados

#### Error/Peligro
**HEX**: `#F44336` | **RGB**: `rgb(244, 67, 54)`

**Uso**: Mensajes de error, botón de salir, estados críticos

#### Éxito
**HEX**: `#4CAF50` | **RGB**: `rgb(76, 175, 80)`

**Uso**: Confirmaciones, conexiones exitosas, checks

#### Advertencia
**HEX**: `#FFC107` | **RGB**: `rgb(255, 193, 7)`

**Uso**: Alertas, información importante no crítica

### Colores de texto

```css
/* Texto principal - alta legibilidad */
.texto-principal {
    color: #333333; /* 87% opacidad sobre blanco */
}

/* Texto secundario - información complementaria */
.texto-secundario {
    color: #666666; /* 60% opacidad sobre blanco */
}

/* Texto deshabilitado */
.texto-deshabilitado {
    color: #999999; /* 38% opacidad sobre blanco */
}

/* Texto invertido (sobre fondos oscuros) */
.texto-invertido {
    color: #FFFFFF;
}

/* Texto sobre primary */
.texto-sobre-primary {
    color: #FFFFFF;
}
```

### Overlays y transparencias

```css
/* Backdrop oscuro para modales */
.backdrop-oscuro {
    background-color: rgba(0, 0, 0, 0.5);
}

/* Backdrop claro para tooltips */
.backdrop-claro {
    background-color: rgba(255, 255, 255, 0.9);
}

/* Overlay de carga */
.overlay-carga {
    background-color: rgba(0, 0, 0, 0.7);
}

/* Panel semi-transparente sobre AR */
.panel-ar-overlay {
    background-color: rgba(255, 255, 255, 0.85);
    backdrop-filter: blur(10px);
}
```

### Gradientes

```css
/* Gradiente principal para headers especiales */
.gradiente-primary {
    background: linear-gradient(135deg, #1E90FF 0%, #64B5F6 100%);
}

/* Gradiente para elementos destacados */
.gradiente-destacado {
    background: linear-gradient(90deg, #1E90FF 0%, #FF9800 100%);
}

/* Gradiente oscuro para footers */
.gradiente-footer {
    background: linear-gradient(180deg, rgba(0,0,0,0) 0%, rgba(0,0,0,0.5) 100%);
}
```

### Accesibilidad de color

Todos los pares de colores cumplen con WCAG 2.1 nivel AA:

| Combinación | Contraste | Nivel |
|-------------|-----------|-------|
| #333333 sobre #FFFFFF | 12.63:1 | AAA |
| #666666 sobre #FFFFFF | 5.74:1 | AA |
| #FFFFFF sobre #1E90FF | 3.28:1 | AA (texto grande) |
| #333333 sobre #F5F5F5 | 11.84:1 | AAA |
| #FFFFFF sobre #F44336 | 4.84:1 | AA |

---

## Espaciado y dimensiones

### Sistema de espaciado

ARTour utiliza un sistema de espaciado basado en múltiplos de 4px:

```css
/* Sistema base de 4px */
--space-4: 4px;   /* 0.25rem */
--space-8: 8px;   /* 0.5rem */
--space-12: 12px; /* 0.75rem */
--space-16: 16px; /* 1rem */
--space-20: 20px; /* 1.25rem */
--space-24: 24px; /* 1.5rem */
--space-32: 32px; /* 2rem */
--space-40: 40px; /* 2.5rem */
--space-48: 48px; /* 3rem */
--space-64: 64px; /* 4rem */
```

### Nomenclatura semántica

```css
/* Espacios por tamaño */
.padding-xs { padding: 4px; }
.padding-sm { padding: 8px; }
.padding-md { padding: 16px; }
.padding-lg { padding: 24px; }
.padding-xl { padding: 32px; }

.margin-xs { margin: 4px; }
.margin-sm { margin: 8px; }
.margin-md { margin: 16px; }
.margin-lg { margin: 24px; }
.margin-xl { margin: 32px; }

/* Espacios direccionales */
.padding-top-md { padding-top: 16px; }
.padding-bottom-md { padding-bottom: 16px; }
.margin-left-lg { margin-left: 24px; }
.margin-right-lg { margin-right: 24px; }
```

### Tamaños de elementos interactivos

Siguiendo las guías de Material Design y Apple HIG:

```css
/* Botones principales */
.btn-principal {
    min-width: 200px;
    min-height: 60px; /* Mínimo 44pt de Apple HIG */
    padding: 16px 32px;
}

/* Botones secundarios */
.btn-secundario {
    min-width: 120px;
    min-height: 44px;
    padding: 12px 24px;
}

/* Botones de icono */
.btn-icono {
    width: 44px;
    height: 44px;
    padding: 10px;
}

/* Elementos de lista táctiles */
.list-item {
    min-height: 56px;
    padding: 12px 16px;
}
```

### Anchos de contenedores

```css
/* Contenedor de pantalla completa */
.container-full {
    width: 100%;
    max-width: 100vw;
}

/* Contenedor con padding lateral */
.container-padded {
    width: 100%;
    padding: 0 20px;
}

/* Contenedor centrado con max-width */
.container-centered {
    width: 100%;
    max-width: 480px;
    margin: 0 auto;
    padding: 0 20px;
}
```

### Grid system

```css
/* Sistema de 12 columnas */
.row {
    display: flex;
    flex-wrap: wrap;
    margin: 0 -8px;
}

.col {
    flex: 1;
    padding: 0 8px;
}

.col-6 {
    flex: 0 0 50%;
    padding: 0 8px;
}

.col-4 {
    flex: 0 0 33.333%;
    padding: 0 8px;
}
```

### Implementación en Unity

```css
/* EstilosGenerales.uss - Unity UI Toolkit */
.contenedor-padded {
    padding-left: 20px;
    padding-right: 20px;
    padding-top: 16px;
    padding-bottom: 16px;
}

.espaciado-vertical-md {
    margin-top: 16px;
    margin-bottom: 16px;
}

.boton-tactil {
    min-width: 200px;
    min-height: 60px;
    padding-left: 32px;
    padding-right: 32px;
    padding-top: 16px;
    padding-bottom: 16px;
}
```

---

## Iconografía

### Librería de iconos

ARTour utiliza iconos del sistema Material Design Icons y algunos iconos personalizados:

#### Iconos principales

| Icono | Nombre | Uso | Tamaño |
|-------|--------|-----|---------|
| 📍 | location | Ubicación actual | 24px |
| 🔊 | volume | Audio/Voz | 24px |
| ❤️ | favorite | Favoritos | 24px |
| ⚙️ | settings | Configuración | 24px |
| ❓ | help | Ayuda | 24px |
| ✓ | check | Confirmación | 20px |
| × | close | Cerrar | 20px |
| ← | arrow_back | Retroceder | 24px |
| → | arrow_forward | Avanzar | 24px |

### Tamaños de iconos

```css
/* Iconos pequeños - en texto inline */
.icon-xs {
    width: 16px;
    height: 16px;
}

/* Iconos normales - botones y UI */
.icon-md {
    width: 24px;
    height: 24px;
}

/* Iconos grandes - acciones principales */
.icon-lg {
    width: 32px;
    height: 32px;
}

/* Iconos extra grandes - elementos destacados */
.icon-xl {
    width: 48px;
    height: 48px;
}
```

### Implementación en Unity

```csharp
// Clase para gestionar iconos
public class GestorIconos : MonoBehaviour
{
    [SerializeField] private Sprite iconoUbicacion;
    [SerializeField] private Sprite iconoAudio;
    [SerializeField] private Sprite iconoFavorito;
    [SerializeField] private Sprite iconoConfiguracion;
    
    public void AplicarIcono(VisualElement elemento, TipoIcono tipo)
    {
        Sprite icono = ObtenerIcono(tipo);
        elemento.style.backgroundImage = new StyleBackground(icono);
    }
}
```

### Guías de uso de iconos

1. **Siempre con label**: Los iconos deben ir acompañados de texto o tooltip
2. **Consistencia**: Usar el mismo icono para la misma acción en toda la app
3. **Tamaño mínimo táctil**: Iconos interactivos en contenedores de mínimo 44x44pt
4. **Color**: Los iconos heredan el color del texto o usan el color primary

---

## Animaciones y transiciones

### Duraciones estándar

```css
/* Transiciones rápidas - feedback inmediato */
.transition-fast {
    transition-duration: 150ms;
    transition-timing-function: ease-out;
}

/* Transiciones normales - la mayoría de animaciones */
.transition-normal {
    transition-duration: 300ms;
    transition-timing-function: ease-in-out;
}

/* Transiciones lentas - cambios importantes */
.transition-slow {
    transition-duration: 500ms;
    transition-timing-function: ease-in-out;
}
```

### Curvas de animación (Easing)

```css
/* Ease Out - Entrada de elementos */
.ease-out {
    transition-timing-function: cubic-bezier(0, 0, 0.2, 1);
}

/* Ease In - Salida de elementos */
.ease-in {
    transition-timing-function: cubic-bezier(0.4, 0, 1, 1);
}

/* Ease In Out - Movimientos continuos */
.ease-in-out {
    transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

/* Sharp - Cambios rápidos y decisivos */
.sharp {
    transition-timing-function: cubic-bezier(0.4, 0, 0.6, 1);
}
```

### Animaciones comunes

#### Fade In/Out

```css
@keyframes fadeIn {
    from {
        opacity: 0;
    }
    to {
        opacity: 1;
    }
}

@keyframes fadeOut {
    from {
        opacity: 1;
    }
    to {
        opacity: 0;
    }
}

.fade-in {
    animation: fadeIn 300ms ease-out forwards;
}

.fade-out {
    animation: fadeOut 300ms ease-in forwards;
}
```

#### Slide In/Out

```css
@keyframes slideInFromRight {
    from {
        transform: translateX(100%);
        opacity: 0;
    }
    to {
        transform: translateX(0);
        opacity: 1;
    }
}

@keyframes slideOutToLeft {
    from {
        transform: translateX(0);
        opacity: 1;
    }
    to {
        transform: translateX(-100%);
        opacity: 0;
    }
}

.slide-in-right {
    animation: slideInFromRight 300ms ease-out forwards;
}

.slide-out-left {
    animation: slideOutToLeft 300ms ease-in forwards;
}
```

#### Scale (Zoom)

```css
@keyframes scaleIn {
    from {
        transform: scale(0.8);
        opacity: 0;
    }
    to {
        transform: scale(1);
        opacity: 1;
    }
}

.scale-in {
    animation: scaleIn 300ms ease-out forwards;
}
```

#### Pulse (Latido)

```css
@keyframes pulse {
    0%, 100% {
        transform: scale(1);
    }
    50% {
        transform: scale(1.05);
    }
}

.pulse {
    animation: pulse 2s ease-in-out infinite;
}
```

### Implementación en Unity con LeanTween

```csharp
public class AnimacionesUI : MonoBehaviour
{
    public static AnimacionesUI Instance { get; private set; }
    
    // Fade In
    public void FadeIn(VisualElement elemento, float duracion = 0.3f)
    {
        elemento.style.opacity = 0;
        LeanTween.value(gameObject, 0f, 1f, duracion)
            .setOnUpdate((float val) => {
                elemento.style.opacity = val;
            })
            .setEase(LeanTweenType.easeOutQuad);
    }
    
    // Fade Out
    public void FadeOut(VisualElement elemento, float duracion = 0.3f)
    {
        LeanTween.value(gameObject, 1f, 0f, duracion)
            .setOnUpdate((float val) => {
                elemento.style.opacity = val;
            })
            .setEase(LeanTweenType.easeInQuad);
    }
    
    // Slide In desde la derecha
    public void SlideInFromRight(VisualElement elemento, float duracion = 0.3f)
    {
        float anchoInicial = elemento.resolvedStyle.width;
        elemento.style.left = anchoInicial;
        
        LeanTween.value(gameObject, anchoInicial, 0f, duracion)
            .setOnUpdate((float val) => {
                elemento.style.left = val;
            })
            .setEase(LeanTweenType.easeOutQuad);
    }
    
    // Scale con bounce
    public void ScaleIn(GameObject obj, float duracion = 0.5f)
    {
        obj.transform.localScale = Vector3.zero;
        LeanTween.scale(obj, Vector3.one, duracion)
            .setEase(LeanTweenType.easeOutBack);
    }
}
```

### Guías de uso de animaciones

1. **No abusar**: Usar animaciones solo cuando aporten valor
2. **Consistencia**: Mismas duraciones para acciones similares
3. **Rendimiento**: Evitar animar demasiados elementos simultáneamente
4. **Interrupción**: Permitir que el usuario interrumpa animaciones largas
5. **Accesibilidad**: Respetar preferencias de "reducir movimiento"

```csharp
// Respetar preferencias de accesibilidad
private bool DebeReducirMovimiento()
{
    return PlayerPrefs.GetInt("ReducirMovimiento", 0) == 1;
}

public void AnimarElemento(VisualElement elemento)
{
    if (DebeReducirMovimiento())
    {
        // Mostrar sin animación
        elemento.style.opacity = 1;
    }
    else
    {
        // Animación normal
        FadeIn(elemento);
    }
}
```

---

## Convenciones de nomenclatura

### Nombres de elementos UI

Seguir el patrón: `[prefijo][Descripcion][Tipo]`

#### Prefijos por tipo de elemento

| Prefijo | Tipo | Ejemplo |
|---------|------|---------|
| btn | Button | `btnIniciarRecorrido` |
| lbl | Label | `lblTitulo` |
| txt | TextField | `txtBusqueda` |
| container | VisualElement contenedor | `containerSensores` |
| panel | VisualElement panel | `panelUbicacion` |
| scroll | ScrollView | `scrollRutas` |
| list | ListView | `listaSensores` |
| img | VisualElement imagen | `imgLogo` |
| toggle | Toggle | `toggleNotificaciones` |
| slider | Slider | `sliderVolumen` |

#### Ejemplos correctos

```csharp
// ✅ Correcto
Button btnIniciarRecorrido;
Label lblSalaActual;
VisualElement containerHeader;
ScrollView scrollListaObras;

// ❌ Incorrecto
Button botonIniciar; // Falta prefijo 'btn'
Label Titulo; // Falta prefijo, PascalCase incorrecto
VisualElement sensores_container; // snake_case incorrecto
```

### Nombres de clases CSS

Usar kebab-case para clases CSS:

```css
/* ✅ Correcto */
.boton-principal { }
.texto-destacado { }
.contenedor-principal { }
.panel-informacion { }

/* ❌ Incorrecto */
.botonPrincipal { } /* camelCase */
.TextoDestacado { } /* PascalCase */
.contenedor_principal { } /* snake_case */
```

### Nombres de archivos

#### UXML (UI Documents)

Patrón: `UIDocument[NombrePantalla].uxml`

```
UIDocumentInicio.uxml
UIDocumentEscaneo.uxml
UIDocumentARTour.uxml
UIDocumentConfiguracion.uxml
```

#### USS (Stylesheets)

Patrón: `[Descripcion].uss` o `[NombrePantalla].uss`

```
EstilosGenerales.uss
PantallaInicio.uss
PantallaEscaneo.uss
PantallaARTour.uss
ComponentesComunes.uss
```

#### Scripts C#

Patrón: `[Nombre][Tipo].cs`

```csharp
// Controladores
CambiadorDePantallas.cs
ControlUI.cs
GestorEstados.cs

// Managers
VozSinteticaManager.cs
TrilateracionManager.cs

// Componentes
AnimacionesUI.cs
PanelUbicacionController.cs

// Utilidades
EstilosBoton.cs
AdaptadorTexto.cs
```

### Scripts terminados según función

| Sufijo | Propósito | Ejemplo |
|--------|-----------|---------|
| Manager | Gestión global de sistemas | `AudioManager.cs` |
| Controller | Control de componentes específicos | `PanelController.cs` |
| Handler | Manejo de eventos | `InputHandler.cs` |
| UI | Scripts específicos de interfaz | `AnimacionesUI.cs` |
| Util / Helper | Funciones auxiliares | `MathUtil.cs` |

---

## Responsive design

### Breakpoints

Aunque ARTour es principalmente una app móvil, se consideran estos breakpoints para testing:

```css
/* Móvil pequeño (hasta 375px) */
@media (max-width: 375px) {
    .titulo-principal {
        font-size: 24px;
    }
    .btn-principal {
        width: 100%;
    }
}

/* Móvil estándar (376px - 767px) - Default */
/* No requiere media query */

/* Tablet (768px - 1023px) */
@media (min-width: 768px) {
    .container-centered {
        max-width: 600px;
    }
    .titulo-principal {
        font-size: 32px;
    }
}

/* Desktop (1024px+) - Solo para testing */
@media (min-width: 1024px) {
    .container-centered {
        max-width: 480px;
    }
}
```

### Orientación

```css
/* Portrait (vertical) - Default */
.contenedor-orientacion {
    flex-direction: column;
}

/* Landscape (horizontal) */
@media (orientation: landscape) {
    .contenedor-orientacion {
        flex-direction: row;
    }
    
    .panel-lateral {
        width: 50%;
    }
}
```

### Safe areas

Consideración para notch de iPhone y otros dispositivos:

```csharp
// Script para manejar safe areas
public class SafeAreaHandler : MonoBehaviour
{
    private void Start()
    {
        ApplySafeArea();
    }
    
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        UIDocument document = GetComponent<UIDocument>();
        VisualElement root = document.rootVisualElement;
        
        // Aplicar padding basado en safe area
        root.style.paddingTop = Screen.height - safeArea.yMax;
        root.style.paddingBottom = safeArea.yMin;
        root.style.paddingLeft = safeArea.xMin;
        root.style.paddingRight = Screen.width - safeArea.xMax;
    }
}
```

---

## Guía de implementación rápida

### Checklist de estilo para nuevos elementos

Al crear un nuevo elemento UI, verificar:

- [ ] Fuente correcta aplicada (Outfit)
- [ ] Tamaño de fuente según jerarquía
- [ ] Colores de la paleta oficial
- [ ] Espaciado múltiplo de 4px
- [ ] Tamaño mínimo táctil (44x44pt)
- [ ] Transiciones definidas
- [ ] Nombre con convención correcta
- [ ] Contraste de color accesible (WCAG AA)
- [ ] Responsive si aplica

### Template de botón completo

```xml
<!-- UXML -->
<ui:Button name="btnEjemplo" class="btn-principal btn-ejemplo" text="Acción" />
```

```css
/* USS */
#btnEjemplo {
    width: 280px;
    height: 60px;
    background-color: #1E90FF;
    color: #FFFFFF;
    border-radius: 30px;
    border-width: 0;
    -unity-font: url('/Assets/Fonts/Outfit-Medium.ttf');
    font-size: 18px;
    -unity-text-align: middle-center;
    transition-duration: 0.3s;
}

#btnEjemplo:hover {
    scale: 1.05;
    background-color: #1C7ED6;
}

#btnEjemplo:active {
    scale: 0.95;
    background-color: #1565C0;
}

#btnEjemplo:disabled {
    background-color: #CCCCCC;
    color: #999999;
}
```

```csharp
// C# - Código de implementación
public class ConfiguradorBoton : MonoBehaviour
{
    private void Start()
    {
        var btnEjemplo = root.Q<Button>("btnEjemplo");
        btnEjemplo.clicked += OnBtnEjemploClicked;
    }
    
    private void OnBtnEjemploClicked()
    {
        // Lógica del botón
    }
}
```

---

## Recursos adicionales

### Herramientas recomendadas

1. **Figma**: Para diseño y prototipado
2. **Coolors.co**: Para generación de paletas
3. **WebAIM Contrast Checker**: Para verificar accesibilidad
4. **Google Fonts**: Para preview de tipografías
5. **LeanTween**: Para animaciones en Unity

### Referencias de diseño

- Material Design 3: https://m3.material.io/
- Apple Human Interface Guidelines: https://developer.apple.com/design/
- Unity UI Toolkit: https://docs.unity3d.com/Manual/UIElements.html

### Actualizaciones del sistema de diseño

Este documento debe actualizarse cuando:
- Se agreguen nuevos colores a la paleta
- Se modifiquen tamaños o espaciados base
- Se implementen nuevas animaciones estándar
- Se agreguen nuevas convenciones de nomenclatura

**Última actualización**: Octubre 2025
**Versión**: 1.0
**Responsable**: Equipo UI/UX ARTour
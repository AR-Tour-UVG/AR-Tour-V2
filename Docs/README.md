# ARTour Virtual (AR Tour App V2) 🏛️📱
Aplicación de recorridos virtuales con **Realidad Aumentada (AR)**, desarrollada en Unity y diseñada para la **Universidad del Valle de Guatemala**.  
> [!NOTE]
> Esta nueva versión integra mejoras en navegación con sensores UWB, accesibilidad y usabilidad.

---

## 🚀 Objetivo del Proyecto
Brindar una experiencia interactiva que combine recorridos físicos con información digital aumentada, integrando:
- Realidad Aumentada.
- Sensores UWB Beacon para posicionamiento.
- Interfaz intuitiva y responsive en iOS (y multiplataforma en fases futuras).
- Funcionalidades de gamificación con minijuegos.

---

## ✨ Características principales
- 🗺️ **Navegación en interiores** con beacons UWB de Estimote.  
- 🧭 **Pathfinding en tiempo real** usando NavMesh de Unity.  
- 🎙️ **Asistente de voz** como guía virtual (Google TTS).  
- 📱 **Interfaz responsiva** con UI Toolkit.  
- 🎮 **Minijuegos integrados** para gamificación de la experiencia.  

---

## 📂 Estructura del Proyecto
```
ARTour/
│── Assets/
│   ├── UI/             # Pantallas .uxml y estilos .uss
│   ├── Scripts/        # Lógica en C#
│   ├── Minijuegos/     # Juegos heredados de versiones anteriores
│   └── Resources/      # Imágenes, fuentes y materiales
│
│── Docs/               # Documentación extendida en Markdown
│── Packages/           # Dependencias de Unity
│── ProjectSettings/    # Configuración del proyecto
│── README.md          # Este archivo
```

---

## 🔄 Control de Versiones
Este proyecto utiliza **Unity Version Control** integrado con GitHub:
- El repositorio de GitHub y Unity Version Control están **sincronizados automáticamente**.
- Los commits realizados en Unity Version Control se reflejan automáticamente en GitHub.
- Esto permite un flujo de trabajo colaborativo eficiente entre desarrolladores.

---

## 🛠️ Requisitos
- **Unity** 6.000.0.55f1 o superior.  
- **SDK Estimote UWB Beacons**.  
- **Xcode** (para compilaciones móviles).  
- **.NET Framework** compatible con Unity.  

---

## ⚙️ Instalación y Ejecución
1. Clonar el repositorio:
   ```bash
   git clone https://github.com/AR-Tour-UVG/AR-Tour-V2.git
   ```
2. Abrir el proyecto en Unity Hub.
3. Seleccionar la escena principal:
   ```
   Assets/Scenes/PantallaPrincipal.unity
   ```
4. Ejecutar en Play Mode o compilar para iOS.

---

## 📱 Pantallas Principales
- **PantallaInicio** → Menú principal.
- **PantallaEscaneo** → Conexión a sensores UWB Beacon (con bypass de prueba).
- **PantallaARTour** → Vista AR con información de ubicación, nivel, ruta e instrucciones.
- **PantallaMinijuegos** → Acceso a juegos interactivos como Breakout, FlappyJack y Trivia.

---

## 🧩 Arquitectura
- **UI Toolkit** (UXML + USS) para interfaz.
- **CambiadorDePantallas.cs** gestiona navegación entre pantallas.
- **Módulo UWB Beacons** (en desarrollo) para posicionamiento en interiores.
- **Módulo Minijuegos** importado y adaptado de versiones anteriores.

---

## 👥 Integrantes
- Diego Leiva
- Gustavo Gonzalez
- Marta Ramírez
- Eduardo Ramírez

---

## 📖 Documentación Extendida
La documentación completa (manual de usuario, manual técnico y guías de instalación) se encuentra en la carpeta `/Docs`.

---

## 🔮 Futuro del Proyecto
- Migración completa a multiplataforma (Android + iOS).
- Integración real de sensores UWB Beacon.
- Optimización de minijuegos.
- Implementación de panel de administración para gestión de rutas.

---

## 📜 Licencia
Este proyecto es de carácter académico. Uso libre para fines educativos.

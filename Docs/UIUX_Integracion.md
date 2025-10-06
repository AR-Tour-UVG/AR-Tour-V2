# Integración con otros módulos - ARTour Virtual

## Conexión con Navegación y voz sintética
- ARTour recibe la ubicación actual y las rutas generadas por Navegación.
- Botones y eventos pueden disparar mensajes de voz sintética.

## Conexión con Trilateración y sensores
- Módulo UI/UX muestra datos de sensores (estado de conexión, ubicación detectada).
- Eventos de sensores disparan animaciones y actualizaciones de la interfaz.

## Secuencia de eventos
1. Usuario inicia recorrido → Pantalla Escaneo
2. Simulación de conexión a sensores
3. Pantalla ARTour muestra ruta y ubicación
4. Usuario interactúa con elementos → envía eventos a módulos de navegación y voz
> Imagen sugerida: `images/flujo_integracion.png`
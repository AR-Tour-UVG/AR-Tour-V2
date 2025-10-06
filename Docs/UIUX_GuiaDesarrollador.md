# Guía de Desarrollador - Módulo UI/UX ARTour

## Cómo agregar una nueva pantalla
1. Crear un `UIDocumentNuevo.uxml` dentro de `UIDocuments/`
2. Crear un archivo `.uss` para estilos específicos si aplica
3. Agregar prefabs o elementos necesarios a `Prefabs/`
4. Registrar el nuevo `UIDocument` en `CambiadorDePantallas.cs`

## Pruebas y simulación
- Simular sensores usando el bypass temporal
- Revisar navegación entre pantallas
- Verificar consistencia de estilos y fuentes

## Tips de mantenimiento
- Mantener nombres de objetos consistentes
- Usar `VisualElement.Q<Button>("nombre")` para acceder a botones
- Siempre actualizar los estilos generales en `EstilosGenerales.uss`
- Revisar animaciones antes de mergear cambios
> Imagen sugerida: `images/guia_desarrollador.png`
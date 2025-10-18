using UnityEngine;

public class ModelGimbalVisualizer : MonoBehaviour
{
    public Transform model;
    public float outerRadius = 0.6f;
    public float middleRadius = 0.45f;
    public float innerRadius = 0.3f;
    public int segments = 64;
    
    private LineRenderer outerRing;
    private LineRenderer middleRing;
    private LineRenderer innerRing;
    
    void Start()
    {
        outerRing = CreateLineRenderer("OuterRing", Color.red);
        middleRing = CreateLineRenderer("MiddleRing", Color.green);
        innerRing = CreateLineRenderer("InnerRing", Color.yellow);
        
        Debug.Log("[Gimbal] Created line renderers");
        Debug.Log($"[Gimbal] Model position: {model.position}");
        Debug.Log($"[Gimbal] Outer radius: {outerRadius}");
    }
    
    void Update()
    {
        if (model == null) 
        {
            Debug.LogWarning("[Gimbal] Model is null!");
            return;
        }
        
        Vector3 pos = model.position;
        Vector3 euler = model.rotation.eulerAngles;
        
        float yaw = euler.y;
        float pitch = euler.x;
        
        Quaternion outerRot = Quaternion.Euler(0, yaw, 0);
        Quaternion middleRot = Quaternion.Euler(pitch, yaw, 0);
        
        Vector3 middleAxis = outerRot * Vector3.right;
        Vector3 innerAxis = middleRot * Vector3.forward;
        
        DrawRingRuntime(outerRing, pos, Vector3.up, outerRot * Vector3.forward, outerRadius);
        DrawRingRuntime(middleRing, pos, middleAxis, middleRot * Vector3.up, middleRadius);
        DrawRingRuntime(innerRing, pos, innerAxis, model.rotation * Vector3.up, innerRadius);
        
        // Debug every 60 frames
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[Gimbal] Position: {pos}, Yaw: {yaw:F1}, Pitch: {pitch:F1}");
            Debug.Log($"[Gimbal] OuterRing enabled: {outerRing.enabled}, positions: {outerRing.positionCount}");
            Debug.Log($"[Gimbal] First point: {outerRing.GetPosition(0)}, Last point: {outerRing.GetPosition(segments)}");
        }
    }
    
    LineRenderer CreateLineRenderer(string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = transform;
        obj.transform.position = Vector3.zero;
        
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        
        // Use Unlit/Color shader
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = color;
        
        lr.material = mat;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f; // VERY thick for testing
        lr.endWidth = 0.1f;
        lr.positionCount = segments + 1;
        lr.useWorldSpace = true;
        lr.enabled = true; // Explicitly enable
        
        // Make sure it renders
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        
        Debug.Log($"[Gimbal] Created {name}: enabled={lr.enabled}, material={lr.material.name}");
        
        return lr;
    }
    
    void DrawRingRuntime(LineRenderer lr, Vector3 center, Vector3 axis, Vector3 up, float radius)
    {
        if (lr == null)
        {
            Debug.LogError("[Gimbal] LineRenderer is null!");
            return;
        }
        
        for (int i = 0; i <= segments; i++)
        {
            float angle = 360f * i / segments;
            Vector3 point = center + Quaternion.AngleAxis(angle, axis) * up * radius;
            lr.SetPosition(i, point);
        }
    }
}
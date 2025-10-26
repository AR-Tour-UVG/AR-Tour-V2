using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class KeyboardPositioning : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Speed in meters/second")]
    [SerializeField]
    private float moveSpeed = 2f;

    [Header("Target")]
    [Tooltip("The player object to move")]
    [SerializeField]
    private Transform target;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Log("[KeyboardPositioning] Enabled in non-Editor build.");
        enabled = false;
#else
        if (target == null)
            target = transform;
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        gameObject.tag = "Player";
#endif
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        int h = 0;
        int v = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            h -= 1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            h += 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            v -= 1;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            v += 1;

        if (h == 0 && v == 0)
            return;

        Vector3 dir = new Vector3(h, 0f, v).normalized;
        target.position += moveSpeed * Time.deltaTime * dir;
    }
}

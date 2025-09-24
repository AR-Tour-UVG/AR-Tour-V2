using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class KeyboardAgent : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 180f;
    public bool snapToNavMesh = true;
    public float snapRadius = 0.3f;

    NavMeshAgent agent;

    void Awake()
    {
        Debug.Log("[KeyboardAgent] Awakened");
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!enabled) return;                // enabled is toggled by the bootstrap
        if (agent.enabled) agent.enabled = false;

        Vector2 v = Vector2.zero;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v.y += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v.y -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) v.x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  v.x -= 1f;
        }
        var gp = Gamepad.current;
        if (gp != null) v += gp.leftStick.ReadValue();

        if (v.sqrMagnitude == 0f) return;
        if (v.sqrMagnitude > 1f) v.Normalize();

        Vector3 move = new Vector3(v.x, 0f, v.y);
        Vector3 next = transform.position + move * moveSpeed * Time.deltaTime;

        if (snapToNavMesh && NavMesh.SamplePosition(next, out var hit, snapRadius, NavMesh.AllAreas))
            next = hit.position;

        transform.position = next;

        var targetRot = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}

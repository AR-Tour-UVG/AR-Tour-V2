using UnityEngine.UIElements;

public static class UIPickingUtils
{
    // Make the whole subtree pass-through
    public static void ConfigureTreePickingMode(VisualElement root, PickingMode mode)
    {
        if (root == null)
            return;
        root.pickingMode = mode;
        root.Query<VisualElement>().ForEach(e => e.pickingMode = mode);
    }

    public static void SetPassThrough(this VisualElement ve)
    {
        if (ve != null)
            ve.pickingMode = PickingMode.Ignore;
    }

    public static void SetPickable(this VisualElement ve)
    {
        if (ve != null)
            ve.pickingMode = PickingMode.Position;
    }
}

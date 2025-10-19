using UnityEngine;

[CreateAssetMenu(fileName = "NewActionData", menuName = "AR GUI/Action Pop Up Data")]
public class ActionData : ScriptableObject
{
    [SerializeField]
    private string actionTitle;

    [SerializeField]
    private string actionDescription;

    [SerializeField]
    private Sprite actionJack;

    [SerializeField]
    private string actionBtnText;

    public string ActionTitle => actionTitle;
    public string ActionDescription => actionDescription;
    public Sprite ActionJack => actionJack;
    public string ActionBtnText => actionBtnText;
}

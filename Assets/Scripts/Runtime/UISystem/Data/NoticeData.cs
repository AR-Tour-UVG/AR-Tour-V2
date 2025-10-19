using UnityEngine;

[CreateAssetMenu(fileName = "NewNoticeData", menuName = "AR GUI/Notice Pop Up Data")]
public class NoticeData : ScriptableObject
{
    [SerializeField]
    private string noticeTitle;

    [SerializeField]
    private string noticeMessage;

    [SerializeField]
    private Sprite noticeJack;
    public string NoticeTitle => noticeTitle;
    public string NoticeMessage => noticeMessage;
    public Sprite NoticeJack => noticeJack;
}

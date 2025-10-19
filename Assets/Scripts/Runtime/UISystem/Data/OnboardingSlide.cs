using UnityEngine;

[CreateAssetMenu(fileName = "NewOnboardingSlide", menuName = "AR GUI/Onboarding Slide")]
public class OnboardingSlide : ScriptableObject
{
    [SerializeField]
    private string title;

    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite art;
    private string buttonText;

    public string Title => title;
    public string Description => description;
    public Sprite Art => art;
    public string ButtonText => buttonText;
}

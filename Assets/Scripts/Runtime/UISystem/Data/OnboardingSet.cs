using UnityEngine;

[CreateAssetMenu(fileName = "NewOnboardingSet", menuName = "AR GUI/Onboarding Set")]
public class OnboardingSet : ScriptableObject
{
    [SerializeField]
    private OnboardingSlide[] slides;

    [SerializeField]
    private int showEveryNDays;

    public OnboardingSlide[] Slides => slides;
    public int ShowEveryNDays => showEveryNDays;
}

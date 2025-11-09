using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimationConfig", menuName = "Player/Animation Config")]
public class PlayerAnimationConfig : ScriptableObject
{
    [Header("Animation Clips")]
    public AnimationClip idleAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip fallAnimation;
    public AnimationClip dashAnimation;
    public AnimationClip attackAnimation;
    public AnimationClip parryAnimation;
    public AnimationClip hurtAnimation;
    public AnimationClip deathAnimation;
    
    [Header("Animator Controller")]
    public RuntimeAnimatorController animatorController;
    
    [Header("Animation Settings")]
    public float transitionDuration = 0.1f;
    public bool applyRootMotion = false;
    public bool stabilizeFeet = false;
    
    [Header("Animation Events")]
    public float attackEventTime = 0.3f;
    public float dashEventTime = 0.1f;
    public float parryActiveTime = 0.2f;
    public float parryEndTime = 0.4f;
}
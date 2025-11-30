using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationManagement : MonoBehaviour
{
    [SerializeField] private string isHit = "Is Hit";

    private MageHitVFX mageHit;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        mageHit = GetComponentInParent<MageHitVFX>();
    }
    //public void NotifyEndImmunity() { }
    public void EndHitAnimation()
    {
        animator.SetBool(isHit, false);
    }

    public void ShowGameOverScreen()
    {
        LevelManager.Instance.ActivateGameOver();
    }
}

using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    //private PlayerController playerController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        float moveMagnitude = Main.PlayerControl.CurMovementInput.magnitude;

        // walk 애니메이션
        if (moveMagnitude > 0.01)
        {
            animator.SetFloat("Speed", moveMagnitude);
        }
        // idle 애니메이션
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }
}

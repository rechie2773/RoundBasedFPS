using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnims : MonoBehaviour
{
    private Animator currentAnimator; 

    public void SetAnimator(Animator animator)
    {
        currentAnimator = animator;
    }

    public void PlayShootAnimation()
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("Shoot", true);
        }
    }

    public void StopShootAnimation()
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetBool("Shoot", false);
        }
    }

    public void PlayReloadAnimation()
    {
        if (currentAnimator != null)
        {
            currentAnimator.SetTrigger("Reload");
        }
    }
}

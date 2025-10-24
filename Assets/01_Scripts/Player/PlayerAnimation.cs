using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    PlayerContext plc;
    PlayerController player;
    PlayerAnimationId animId;

    private void Awake()
    {
        plc = GetComponent<PlayerContext>();
        player = GetComponent<PlayerController>();
        animId = GetComponent<PlayerAnimationId>();
    }

    private void Update()
    {
        PlayAnimation();
    }

    void PlayAnimation()
    {
        plc.Anim.SetFloat(animId.GetAnimationHash(AnimationMap.MoveX), Mathf.Abs(plc.Velocity.x));
        plc.Anim.SetFloat(animId.GetAnimationHash(AnimationMap.MoveY), Mathf.Abs(plc.Velocity.y));
    }
}

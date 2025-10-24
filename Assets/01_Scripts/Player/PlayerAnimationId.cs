using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationMap
{
    MoveX,
    MoveY,
}

public class PlayerAnimationId : MonoBehaviour
{
    private static Dictionary<AnimationMap, int> hashMap = new();

    void Awake()
    {
        foreach(var an in Enum.GetValues(typeof(AnimationMap)))
        {
            if (!hashMap.ContainsKey((AnimationMap)an))
            {
                hashMap.Add((AnimationMap)an, Animator.StringToHash(an.ToString()));
            }
        }
    }

    public int GetAnimationHash(AnimationMap anim) => hashMap[anim];
}
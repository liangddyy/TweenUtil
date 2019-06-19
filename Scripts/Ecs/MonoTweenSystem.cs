using System;
using System.Collections;
using System.Collections.Generic;
using Tween;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MonoTweenSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TweenerData data,ref Translation translation) =>
        {
            if (data.Type == AnimType.Position)
            {
                var deltaTime = Time.deltaTime;
                translation = new Translation()
                {
                    Value = new float3(translation.Value.x, translation.Value.y + deltaTime, translation.Value.z)
                };
            }
        });
    }
}

[Serializable]
public struct TweenerData : IComponentData
{
    public AnimType Type;
}

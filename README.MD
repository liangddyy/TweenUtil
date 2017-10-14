# TweenUtil

一个简单、完整、易用的Unity插值动画插件。

## 功能

- 一行代码实现一个动画
- 移动、缩放、旋转、Color、Alpha、Shader等所有需要插值的地方都能便捷使用。
- 支持所有缓动函数。
- 支持匀速曲线运动、带缓动函数的曲线运动。
- 支持UGUI。
- 多种循环方式、自动回收脚本/脚本复用。
- 支持忽略Timescale。
- 更便捷的停止/暂停/反转所有动画等。
- 支持自定义插值。
- 支持AnimationCurve
- 。。。

## 使用

直接放到项目中即可。

## 示例

```
// 直线移动。从当前位置移动到原点。时间2s。
transform.TnMove(null, Vector3.zero, 2f);

// 直线移动。从当前位置移动到原点。并且循环。运动一次2s。缓动函数OutBounce。
transform.TnMove(null, Vector3.zero, 2f).SetLoopType(LoopType.PingPang).SetEase(Ease.OutBounce);

// 曲线移动。从当前位置开始，依次通过 vector3[] nodePoints
transform.TnPathMove(null, pathTest.nodePoints, 2);

Img.TnUguiColor(Color.white, Color.blue, 2f).SetLoopType(LoopType.PingPang);
transform.TnRotate();
transform.TnScale();
...
```

> 如果你想看完整的代码和示例，可以在[Unity商店](https://www.assetstore.unity3d.com/cn/#!/content/97821) 下载。
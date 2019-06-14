# TweenUtil

一个简单、完整、易用的Unity插值动画插件。

可以通过代码类似Dotween的形式调用；也可以挂在脚本到物体上调整动画，同时可以直接在编辑器预览，方便美术等非开发人员使用。

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

直接放到Unity项目中即可。

### 方式1（脚本方式）

```
// 直线移动。从当前位置移动到原点。时间2s。
transform.TnMove(Vector3.zero, 2f);

// 直线移动。从当前位置移动到原点。并且循环。运动一次2s。缓动函数OutBounce。
transform.TnMove(Vector3.zero, 2f).SetLoopType(LoopType.PingPang).SetEase(Ease.OutBounce);

// 曲线移动。从当前位置开始，依次通过 vector3[] nodePoints
transform.TnPathMove(pathTest.nodePoints, 2);

Img.TnUguiColor(Color.white, Color.blue, 2f).SetLoopType(LoopType.PingPang);
transform.TnRotate();
transform.TnScale();
...
```

### 方式2（Mono）

挂在 MonoTweener到物体上，按需添加动画。



> 如果你想看使用示例，可以在[Unity商店](https://www.assetstore.unity3d.com/cn/#!/content/97821) 下载 (可能不是最新)
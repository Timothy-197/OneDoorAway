# Portal2d
--------------------------------------------------------------------
### Layer设置标准

* 每个场景中的 GameObject 都应该有一个 layer
* 新建 layer 的名称指代对应 GameObject 的**类型**，而不是**属性**
  * Good Example: `Player`, `Portal Obj`, `Gravity Ball`, `Prop`
  * Bad Example: `Portal Transferable`, `Player Collectable`


--------------------------------------------------------------------
### Character Controller
Everything realting to the player control should be placed under the ```Player Control``` folder

#### Keyboard Control

* a, d / left, right: horizontal move along the ground
* w / up: jump

#### Player Controller parameters
* ...speed: 各种速度
* Gravity: 重力大小，请输入**正数**(magnitude)
* Head Bounce Speed: player可能会头撞墙，此速度为头撞墙后弹回速度
* Ground Layer: ground物体的layer,用于ground detection

#### Get and set player velocity
* ```public float GetSpeed()```
 得到速度大小(magnitude)

* ```public void SetVelocity(Vector3 newVelo)```
 设置速度大小: 传入参数 -> 速度(**vector**)

* 如何调用这些函数？
 直接如此调用：

  ```c#
 BasicMove.Instance.SetVelocity(new Vector3(0, 0.5f, 0));
  ```

 

#### 注意
* 因为playercontroller未用unity物理，所以速度数值偏小，但只要有关player速度只调用```basic move```里的函数就不必担心这个问题

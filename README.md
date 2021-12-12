# One Door Away

### Layer设置标准

* 每个场景中的 GameObject 都应该有一个 layer
* 新建 layer 的名称指代对应 GameObject 的**类型**，而不是**属性**
  * Good Example: `Player`, `Portal Obj`, `Gravity Ball`, `Prop`
  * Bad Example: `Portal Transferable`, `Player Collectable`

### Layers

* Ground: 地面物体，用于地面检测，放置Portal
* gravity block: 玩家可以捡起的物体


---
### Character Controller
Everything realting to the player control should be placed under the `Player Control` folder

#### Keyboard Control

* a, d / left, right: horizontal move along the ground
* w / up: jump
* s / down: pick and place moveable objects

#### Player Controller parameters
* ...speed: 各种速度

* Gravity: 重力大小，请输入**正数**(magnitude), 默认值为9.8

* Head Bounce Speed: player可能会头撞墙，此速度为头撞墙后弹回速度

* Ground Layer: ground物体的layer,用于ground detection

* Note:

  > 1. the move speeds in the inspector is not actual speed in the game. However, the max speed should be the actual speed in the game.

#### Get and set player velocity
* `public float GetSpeed()`
 得到速度大小(magnitude)

* `public void SetVelocity(Vector3 newVelo)`
 设置速度大小: 传入参数 -> 速度(**vector**)

 你也可以输入一个Vector2的argument，毕竟这是个2d游戏

* 如何调用这些函数？
 直接如此调用：

  ```c#
   BasicMove.Instance.SetVelocity(new Vector3(0, 0.5f, 0));
  ```

 

#### 注意
* playercontroller未用unity物理，但只要有关player速度只调用```basic move```里的函数就不必担心这个问题

-----------------------------

### Triggers and doors

对于简单的门和触发器，我做了prefab，可以在scene中直接用，不过你需要设置几个参数。对于复杂的门，我认为用动画实现会更好。

#### Doors

* Slide Door：若玩家触发开关，此门会平移，若玩家离开开关，此门会回到原位。
* Rotate Door：若玩家出发开关，此门会旋转。

#### Trigger

* Type1: Press-down trigger: 有重物压在其上视为触发开关
* Type2: Toggle trigger: player can trigger the toggle on or off

#### How to set up?

1. Trigger:

   1.1. Door trigger / Door toggle

   **set trigger index** (which is **unique** for this particular trigger)

   add layers that can be seen as gravity objects to trigger the trigger.

2. Door

   2.1. Slide door


   **set door index** (this index should **equals to the corresponding trigger of the door**)

   set the parameter to control the move of the door (which is straightforward)

   2.2. Rotate door

   **set door index** (this index should **equals to the corresponding trigger of the door**)

   set the parameter to control the rotationv (note: **direction 1: counter-clockwise, -1: clockwise**)

3. Note

   * You can control several doors by one trigger by setting the door indexes equal to the trigger index.
   * you can change the transdorm of the the doors as you like, but pay attention to that you need to change the sprite child under the "Slide Door" and "Rotate Door" gameobjects.

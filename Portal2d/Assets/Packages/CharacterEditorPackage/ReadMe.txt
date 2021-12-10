This document explains how to set up and work with the extendable 2D character controller

------------------------------------------------
Setup
------------------------------------------------
To make the physics work, two changes need to be made to the project.
The first is that an additional layer needs to be added. This layer is "Player". In the package, layer 31 (the last editable layer) is used.
WARNING: if that layer number is already in use, and you decide to use another layer, there are two things that need to be changed.
1: On the character object, change the object layer to your new layer name. A Unity pop-up will ask if you want to change child objects as well.
It doesn't matter either way to the character controller; only the base object needs to be a specific layer.
2: On the character object, find the ControlledCapsuleCollider script. Under "General collision settings", change the LayerMask property to uncheck your new layer.
Alternatively, in the character editor window, change the CollisionMask property to uncheck your new layer.

The second change needs to be made in Edit->Project settings->Physics.
Here, uncheck the 'Queries hit triggers' box. If this box is left checked, the player will treat triggers as solid objects.

Be aware that the character controller uses 3D colliders (in order to use the capsule shape), and can break using concave colliders.
------------------------------------------------
Creating a character
------------------------------------------------
There are two ways to create a character. The first is to copy and modify one of the supplied prefabs: 
ClassicCharacter.prefab
DefaultCharacter.prefab
HeroCubeCharacter.prefab
all of which can be found in the CharacterEditorPackage/Prefabs/CharacterControllers folder.
They can also be found in the provided scenes, where they can be tried out.

The second way to create a character is through the Character Editor Window. This window can be found in Window->Character Editor.
While this window is open, select a gameobject that you wish to turn into a character controller.
The CharacterEditorWindow will then display: "Character script could not be found on this object. Create it?".
There will also be a button: "Create character on this object".

By pressing this button, the gameobject will get all components that it needs to function as a character controller.
Be warned that this does not include anything renderable, so the character will be invisible until a mesh renderer and mesh filter are added (or a sprite for example).

------------------------------------------------
Character Editor Window
------------------------------------------------
To ease up the editing of the character controller, a custom editor window is provided.
This gives easy access to some of the variables you might want to tweak in a character controller.
In the Unity editor, go to Window->Character Editor to open this package's character editor.
By default, no character is selected and the window will be empty.
Select a character in the editor, or a character prefab in the project folder to make it show up in the character editor.

There are four tabs in the editor window: Character, Abilities, Input and MovingColliders
Be aware that changes made here during play mode will not persist when play mode is exited.
-----------------
Character Tab
-----------------
The Character tab allows for editing of core behavior. This includes running speed, jump speed, air control as well as capsule size.
Playing around with these settings will most noticeably change the character's feel. Hovering over each variable will provide a short explanation of its effects.

-----------------
Abilities Tab
-----------------
The Abilities tab allows for the addition of Ability Modules (explained in the AbilityModules section later).
These modules are separate prefabs and are not parented to the character controller. Adding Ability Module prefabs to this list will add them to the character's behavior.
This can include things like Crouch, WallJump and DoubleJump. Removing them from this list will disable the character from using them.
Pressing the "Edit" button (after folding out the list item) will select the Ability Module Prefab in the Project view. This will allow you to edit that Ability Module's behavior in the Inspector view.
Currently, the Character Editor Window does not allow for editing of abilities, so the Inspector view must be used for this.

To remove an ability from the character, rightclick on the list item and click "Delete Array Element".
The Ability Module prefab will not be destroyed, but it will be removed from this character.

To add an ability to the character, right click one of the list items and click "Duplicate Array Element".
Alternatively, increase the "Size" field at the top by 1. This will duplicate the last item in the list.
Click on the foldout arrow next to the newly duplicated element. This will show a field called "Prefab"
Drag the Ability Module prefab that you wish to add onto this field. The name of the list item will change to the name of the ability that you just added.

-----------------
Input Tab
-----------------
The Input tab defines the inputs that a character can use. It is a layer between Unity's Input manager and the character controller.
For good functionality, it is important that Unity's Input manager (under Edit->Project Settings->Input) has the "Jump", "Horizontal" and "Vertical" input axes set up, as the character controller relies on these for Jumping and Moving.
Any Ability Module that needs its own input (such as a crouch button) will need to have that input defined in the Input tab.
For the core character behavior, the Input list will need a "Move" and "Jump" item defined. These are created by default, and the Character Editor Window will warn for missing components.
Currently, there are two types of inputs: Direction and Button. By changing the "Input Type" enum, a list item will be defined as being one of these input types.

Direction Input is defined by two Unity axes. In the case of "Move" they are "Horizontal" and "Vertical".
These two together describe a circle, such as an analogue stick on a gamepad, or the combined arrow keys on a keyboard.
The third value, "Direction Threshold", defines how far a stick needs to be pushed before it registers as a direction (such as pressing down to enter a crouch)

Button Input is defined by either a Unity Button, Unity Axis, or both. This is to allow for mapping button input to gamepad triggers (which Unity recognizes as axes instead of buttons).
This way, a "Sprint" input can be bound to the Shift key on keyboard and a trigger on a gamepad.
Selecting one of these options can be done in the Unity Input Type dropdown.
If "Axis" or "Button And Axis" are selected, another dropdown will show for Unity Axis Recognition.
What this controls is when an axis will register as a button press; when it has a positive value, a negative value, or if it has a non-zero value.
Lastly the Button Name is the name of the Unity Input Manager button name or axis name.

-----------------
MovingColliders Tab
-----------------
The MovingColliders tab details how to handle moving colliders. This includes:
Whether the character should fall off platforms that move away too fast (such as an elevator that suddenly drops)
Whether the character should inherit velocity from moving platforms (such as jumping from a moving car)

------------------------------------------------
Ability Modules
------------------------------------------------
Ability Modules are separate prefabs from the character controller. They each define a movement ability (sprinting, crouching and walljumping for instance).
Ability Module prefabs have a single script (derived from GroundedAbilityModule) which defines the ability, including its requirements (which buttons to press for example) and its effects.
It also has a priority which allows it to override other abilities (if multiple can be active at the same time)
If an AbilityModule is added to the character, via the Abilities tab in the Character Editor window, the character will be able to use this ability.

An example: The Sprint module
The sprint is a character ability that works on the ground. It is active when the sprint button is held down.
This means that a "Sprint" button needs to be added to the character via the Input tab of the Character Editor window.
By default, the ClassicCharacter and DefaultCharacter prefab have this button set up.
The effect of a Sprint is that it speeds up the character's movement while it is held down.
To that end, it has a couple of variables (acceleration, max speed, friction) which define the new speed settings.
These variables can be edited in the Inspector view when the Sprint module prefab is selected

The Abilities Tab section above explains how to add and remove Ability Module prefabs from a character.

The following abilities are provided:
DoubleJump - Allows the character controller to jump in the air (once or multiple times).
Sprint - Speeds up motion for the character controller while on the ground. Requires a "Sprint" button.
WallJump - Allows the character controller to jump from a wall it is next to.
WallSlide - Allows the character controller to slide across a wall it is next to.
WallRun - Allows the character controller to briefly run up a wall it is next to.
Crouch - Shrinks the character controller, lowers movement speed. Requires a "Crouch" button, or pressing down on analogue stick/down arrow key
Slide - Shrinks the character controller, slides across ground. Character needs to be moving faster than a certain threshold for the ability to work. Requires a "Crouch" button, or pressing down on analogue stick/down arrow key
LedgeHang - Lets the character controller hang from ledges it is next to. 
EdgeClimb - Allows the character controller to climb up on an edge it is hanging from. Will require a Ledgehang ability module on the same character to work properly.
EdgeJump - Allows the character controller to jump up from an edge it is hanging from. Will require a Ledgehang ability module on the same character to work properly.
Dash - Launches the character controller in a certain direction. Requires an "Aim" directional input (such as an analogue stick) and a "Dash" button input to trigger the dash
DropDown - Allows the character to pass through OneWayCollider objects (Objects with a OneWayPlatform script and collider/triggers set up).
Swim- Allows the character to move freely in Water objects (Objects in the "Water" layer with a trigger component).
[DEPRECATED - to be replaced by NetClim] LadderClimb - Allows the character to climb Ladder objects (Objects with a Ladder script and trigger component set up)
NetClimb - Allows the character to climb Net objects (Objects with a Net script and trigger set up).
WallStick - Allows the character to hang on to a wall.
WallClimb - Allows the character to hang on to a wall and climb up and down along it.
Jetpack - Allows a character to boost itself upwards while in the air.
Zipline - Allows the character to use the Zipline and CurvedZipline objects (as demonstrated in ZiplineScene)

Different character prefabs have different combinations of modules attached to demonstrate them
Opening and playing one of the DemoScene scenes will allow you to view them in action.
By copying one of these scripts, or by creating a new script which derives from GroundedAbilityModule, it is possible to create more abilities.
These new abilities can be added in the same way as the above ones.

------------------------------------------------
Fixing a character
------------------------------------------------
To help prevent issues with characters, the editor provides a way to fix common issues. These could range from accidentally deleted components to missing inputs.
The Character Editor Window attempts to detect these things and offers a way to fix it.
If the Character Editor notices an issue, the Character Editor Window will add a section at the top.
It will state that: "Character is missing components, or links to components. Fix it?"
and then add a button with: "Fix character"
Pressing this button will let the editor find missing components on the character and link them up. In the case of missing components, it will create them (with default values) before linking them up.
This should get the character back in a functional state.

To help with initial setup, the creation of components (and using the Reset context menu option in the inspector) will revert a component back to default variables.


------------------------------------------------
Issues:
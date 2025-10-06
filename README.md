# godot-PhysicalBone2D-Workaround

Originally created by StrikeForceZero in GDScript, a workaround for an ongoing issue with PhysicalBone2D in Godot 4.0.3+: https://github.com/godotengine/godot/issues/78367

I created this because I like keeping my Godot solutions strict to a single language and imagine others prefer this as well.

**IMPORTANT:** Unlike the original implementation, this is designed to be attached directly to a Skeleton2D rather than the scene root, as to avoid the use of unique names or taking up the script field in the scene root.

# Changes:
- In C# (duh)
- Inherits from Skeleton2D instead of Node2D
- As callables work differently in C#, they are instantiated as local variables before being called rather than being instantiated during the call itself, and the node path is given as an argument rather than the node object itself
- Iterates through the modification stack instead of taking the first modifier when searching for SkeletonModification2DPhysicsBones, in case your PhysicalBones modifier is not at index 0
- Error handling if there is no existing SkeletonModification2DPhysicalBones modifier on the modification stack
- The warning if a PhysicalBone2D does not have "Simulate Physics" enabled makes use of Godot's PushWarning() method for debugging purposes

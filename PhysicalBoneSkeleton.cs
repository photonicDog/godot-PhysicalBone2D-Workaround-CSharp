using Godot;
using System;

namespace ProcedralAnimation.scripts.NodeBehaviour
{
    public partial class PhysicalBoneSkeleton : Skeleton2D
    {
        //Credit to StrikeForceZero on GitHub for the original GD implementation
        //See: https://github.com/StrikeForceZero/godot-PhysicalBone2D-Workaround
        public override void _Ready()
        {
            base._Ready();

            var modStack = GetModificationStack();
            // Make sure the stack is enabled at runtime for better editor interaction
            modStack.Enabled = true;

            SkeletonModification2DPhysicalBones modStackPhysicalBones = null;
            for (int i = 0; i < modStack.ModificationCount; i++)
            {
                if (modStack.GetModification(i) is SkeletonModification2DPhysicalBones)
                {
                    modStackPhysicalBones = modStack.GetModification(i) as SkeletonModification2DPhysicalBones;
                }
            }

            if (modStackPhysicalBones != null)
            {
                modStackPhysicalBones.Enabled = true;
                FixSkeleton();

                //Re-fetch all physical bones post-update
                modStackPhysicalBones.FetchPhysicalBones();
                modStackPhysicalBones.StartSimulation();

                //If StopSimulation() is called, FixSkeleton() needs to be ran again before stuff will work again
                modStackPhysicalBones.StopSimulation();
                modStackPhysicalBones.StartSimulation();

                FixSkeleton();

            } else
            {
                throw new Exception($"Could not find SkeletonModification2DPhysicalBones in the SkeletonModificationStack2D. Please check your Skeleton2D's modification stack in the editor properties.");
            }
        }

        public void FixSkeleton()
        {
            foreach(var child in GetChildren())
            {
                if (child is PhysicalBone2D)
                {
                    var callable = new Callable(this, MethodName.UpdateBone);
                    CallChildBone(child as PhysicalBone2D);
                }
            }
        }

        public void CallChildBone(PhysicalBone2D bone)
        {
            var callable = new Callable(this, MethodName.UpdateBone);
            callable.Call(bone.GetPath());
            foreach (var child in bone.GetChildren())
            {
                if (child is PhysicalBone2D)
                {
                    CallChildBone(child as PhysicalBone2D);
                }
            }
        }

        public void UpdateBone(Variant bonePath)
        {
            var bone = GetNode(bonePath.ToString()) as PhysicalBone2D;
            if (!bone.SimulatePhysics)
            {
                //SimulatePhysics needs to be checked in the editor to make sure the bone's result position is properly updated
                //Check the position in editor is correct if this still occurs
                GD.PushWarning($"{bone.Name} does not have Simulate Physics checked! Make sure this is enabled in the editor properties.");
            }

            //Undo the CPP constructor
            bone.Freeze = true;
            bone.Freeze = false;
        }
    }
}

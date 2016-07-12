/* VRPhysicsDisableAllCollisions
 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;

using MiddleVR_Unity3D;

[AddComponentMenu("MiddleVR/Physics/Disable All Collisions")]
public class VRPhysicsDisableAllCollisions : MonoBehaviour {

    #region Member Variables

    private bool mApplied = false;

    #endregion

    #region MonoBehaviour Member Functions

    protected void Start()
    {
        if (MiddleVR.VRPhysicsMgr == null)
        {
            MiddleVRTools.Log(0, "[X] PhysicsDisableAllCollisions: No PhysicsManager found.");
            enabled = false;

            return;
        }
    }

    protected void Update()
    {
        if (!mApplied)
        {
            vrPhysicsEngine physicsEngine = MiddleVR.VRPhysicsMgr.GetPhysicsEngine();

            if (physicsEngine == null)
            {
                MiddleVRTools.Log(0, "[X] PhysicsDisableAllCollisions: No PhysicsEngine found.");
                enabled = false;

                return;
            }

            if (physicsEngine.IsStarted())
            {
                bool disabled = physicsEngine.EnableCollisions(false);

                if (disabled)
                {
                    MiddleVRTools.Log(2, "[ ] PhysicsDisableAllCollisions: all collisions disabled.");
                }

                mApplied = true;
            }
        }
    }

    #endregion
}

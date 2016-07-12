/* VRManagerPostFrame
 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

[AddComponentMenu("")]
public class VRManagerPostFrame : MonoBehaviour {
    public vrKernel kernel = null;
    public vrDeviceManager deviceMgr = null;
    public vrClusterManager clusterMgr = null;

    private bool LoggedNoKeyboard = false;

    private void InitManagers()
    {
        if (kernel == null)
        {
            kernel = MiddleVR.VRKernel;
        }

        if (deviceMgr == null)
        {
            deviceMgr = MiddleVR.VRDeviceMgr;
        }

        if (clusterMgr == null)
        {
            clusterMgr = MiddleVR.VRClusterMgr;
        }
    }

    private IEnumerator PostFrameUpdate()
    {
        yield return new WaitForEndOfFrame();

        VRManagerScript mgr = GetComponent<VRManagerScript>();
        if (mgr != null && MiddleVR.VRKernel == null)
        {
            Debug.LogWarning("[ ] If you have an error mentioning 'DLLNotFoundException: MiddleVR_CSharp', please restart Unity. If this does not fix the problem, please make sure MiddleVR is in the PATH environment variable.");
            mgr.GetComponent<GUIText>().text = "[ ] Check the console window to check if you have an error mentioning 'DLLNotFoundException: MiddleVR_CSharp', please restart Unity. If this does not fix the problem, please make sure MiddleVR is in the PATH environment variable.";
        }

        MVRTools.Log(4, "[>] Unity: Start of VR PostFrameUpdate.");

        if (kernel == null || deviceMgr == null || clusterMgr == null)
        {
            InitManagers();
        }

        if (deviceMgr != null )
        {
            vrKeyboard keyboard = deviceMgr.GetKeyboard();
            if (keyboard != null)
            {
                if (mgr != null && mgr.QuitOnEsc && keyboard.IsKeyPressed((uint)MiddleVR.VRK_ESCAPE))
                {
                    mgr.QuitApplication();
                }
            }
            else
            {
                if (!LoggedNoKeyboard)
                {
                    MVRTools.Log("[X] No VR keyboard.");
                    LoggedNoKeyboard = true;
                }
            }
        }

        if (kernel != null)
        {
            kernel.PostFrameUpdate();
        }

        MVRTools.Log(4, "[<] Unity: End of VR PostFrameUpdate.");

        if (kernel != null && kernel.GetFrame() == 2 && !Application.isEditor)
        {
            MVRTools.Log(2, "[ ] If the application is stuck here and you're using Quad-buffer active stereoscopy, make sure that in the Player Settings of Unity, the option 'Run in Background' is checked.");
        }
    }

    private void Update()
    {
        MVRTools.Log(4, "[>] Unity: VR EndFrame Update!");

        MiddleVR.VRKernel.EndFrameUpdate();

        MVRTools.Log(4, "[ ] Unity: StartCoRoutine PostFrameUpdate");
        StartCoroutine(PostFrameUpdate());

        MVRTools.Log(4, "[<] Unity: End of VR EndFrame Update!");
    }
}

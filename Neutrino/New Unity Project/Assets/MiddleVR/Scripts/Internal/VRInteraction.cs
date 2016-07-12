/* VRInteraction
 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using MiddleVR_Unity3D;

[AddComponentMenu("")]
public class VRInteraction : MonoBehaviour
{
    private vrInteraction   m_Interaction = null;
    private vrEventListener m_Listener = null;

    private bool m_IsActive = false;

    private void OnDestroy()
    {
        MiddleVR.DisposeObject(ref m_Listener);
        MiddleVR.DisposeObject(ref m_Interaction);
    }

    private bool EventListener(vrEvent iEvent)
    {
        vrInteractionEvent evt = vrInteractionEvent.Cast(iEvent);
        if (evt == null)
        {
            return false;
        }

        vrInteraction evtInteraction = evt.GetInteraction();

        if (m_Interaction != null && evtInteraction != null && evt != null &&
            evtInteraction.GetId() == m_Interaction.GetId())
        {
            var eventType = evt.GetEventType();

            if (eventType == (int)VRInteractionEventEnum.VRInteractionEvent_Activated)
            {
                Activate();
            }
            else if (eventType == (int)VRInteractionEventEnum.VRInteractionEvent_Deactivated)
            {
                Deactivate();
            }
        }

        return true;
    }

    public void Activate()
    {
        if (!m_IsActive)
        {
            m_IsActive = true;
            MiddleVR.VRInteractionMgr.Activate(m_Interaction);

            OnActivate();
        }
    }

    public void Deactivate()
    {
        if (m_IsActive)
        {
            m_IsActive = false;
            MiddleVR.VRInteractionMgr.Deactivate(m_Interaction);

            OnDeactivate();
        }
    }

    protected virtual void OnActivate()
    {
        MVRTools.Log(3, "[ ] VRInteraction: Activating '" + m_Interaction.GetName() + "'.");
    }

    protected virtual void OnDeactivate()
    {
        MVRTools.Log(3, "[ ] VRInteraction: Deactivating '" + m_Interaction.GetName() + "'.");
    }

    public bool IsActive()
    {
        return m_IsActive;
    }

    public void InitializeBaseInteraction()
    {
        m_Listener = new vrEventListener(EventListener);
        MiddleVR.VRInteractionMgr.AddEventListener(m_Listener);
    }

    public vrInteraction CreateInteraction(string iName)
    {
        if (m_Interaction == null)
        {
            // Create the requested interaction
            m_Interaction = new vrInteraction(iName);
            MiddleVR.VRInteractionMgr.AddInteraction(m_Interaction);
        }
        else
        {
            // Interaction already existing, rename it
            m_Interaction.SetName(iName);
        }

        return m_Interaction;
    }

    public void SetInteraction(vrInteraction iInteraction)
    {
        m_Interaction = iInteraction;
    }

    public vrInteraction GetInteraction()
    {
        return m_Interaction;
    }
}

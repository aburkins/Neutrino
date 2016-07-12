/* VRInteractionManipulationHomer
 * MiddleVR
 * (c) MiddleVR
 *
 * Note: Made to be attached to the Wand
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;
using System;

[AddComponentMenu("")]
public class VRInteractionManipulationHomer : VRInteraction {

    public string Name               = "InteractionManipulationHomer";
    public string HandNode           = "HandNode";
    public uint   WandGrabButton     = 0;
    public float  TranslationScale   = 5.0f;
    public float  RotationScale      = 2.0f;

    private vrInteractionManipulationHomer m_it   = null;

    private VRManagerScript   m_VRMgr             = null;
    private vrNode3D          m_HandNode          = null;
    private VRWand            m_Wand              = null;

    private GameObject m_CurrentSelectedObject    = null;
    private GameObject m_CurrentManipulatedObject = null;

    private Vector3    m_ManipulatedObjectInitialLocalPosition;
    private Quaternion m_ManipulatedObjectInitialLocalRotation;
    private bool       m_ManipulatedObjectInitialIsKinematic;

    private MVRNodesMapper.ENodesSyncDirection m_ObjectPreviousSyncDir = MVRNodesMapper.ENodesSyncDirection.NoSynchronization;

    private vrInteraction m_PausedSelection = null;


    private void Start()
    {
        // Make sure the base interaction is started
        InitializeBaseInteraction();

        m_VRMgr = GameObject.Find("VRManager").GetComponent<VRManagerScript>();

        m_it = new vrInteractionManipulationHomer(Name);
        // Must tell base class about our interaction
        SetInteraction(m_it);

        MiddleVR.VRInteractionMgr.AddInteraction(m_it);
        MiddleVR.VRInteractionMgr.Activate(m_it);

        m_HandNode = MiddleVR.VRDisplayMgr.GetNode( HandNode );

        if ( m_HandNode != null )
        {
            m_it.SetGrabWandButton( WandGrabButton );
            m_it.SetTranslationScale( TranslationScale );
            m_it.SetRotationScale( RotationScale );
            m_it.SetManipulatorNode(m_HandNode);
        }
        else
        {
            MiddleVR.VRLog( 2, "[X] VRInteractionManipulationHomer: One or several nodes are missing." );
        }

        m_Wand = this.GetComponent<VRWand>();
    }

    void Update ()
    {
        if (IsActive())
        {
            // Retrieve selection result
            VRSelection selection = m_Wand.GetSelection();

            if (selection == null || !selection.SelectedObject.GetComponent<VRActor>().Grabable)
            {
                return;
            }

            m_CurrentSelectedObject = selection.SelectedObject;

            // Manipulation
            if (m_it.HasManipulationStarted())
            {
                // Try to grab
                Grab(m_CurrentSelectedObject);
            }
            else if (m_it.IsManipulationRunning() && m_CurrentManipulatedObject != null)
            {
                // Nothing to do here
            }
            else if (m_it.IsManipulationStopped())
            {
                Ungrab();
            }
        }
    }

    void OnEnable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionManipulationHomer: enabled" );
        if( m_it != null )
        {
            MiddleVR.VRInteractionMgr.Activate( m_it );
        }
    }

    void OnDisable()
    {
        MiddleVR.VRLog( 3, "[ ] VRInteractionManipulationHomer: disabled" );

        if( m_it != null && MiddleVR.VRInteractionMgr != null )
        {
            MiddleVR.VRInteractionMgr.Deactivate( m_it );
        }
    }

    private void Grab( GameObject iGrabbedObject )
    {
        if( iGrabbedObject == null )
        {
            return;
        }

        m_CurrentManipulatedObject = iGrabbedObject;

        VRActor vrActorScript = m_CurrentManipulatedObject.GetComponent<VRActor>();
        m_ObjectPreviousSyncDir = vrActorScript.SyncDirection;
        vrActorScript.SyncDirection = MVRNodesMapper.ENodesSyncDirection.MiddleVRToUnity;
        vrNode3D middleVRNode = vrActorScript.GetMiddleVRNode();
        m_it.SetManipulatedNode(middleVRNode);
        m_it.SetPivotPositionVirtualWorld(MVRTools.FromUnity(m_CurrentManipulatedObject.GetComponent<Collider>().bounds.center));

        // Save initial position
        m_ManipulatedObjectInitialLocalPosition = m_CurrentManipulatedObject.transform.localPosition;
        m_ManipulatedObjectInitialLocalRotation = m_CurrentManipulatedObject.transform.localRotation;

        // Pause rigidbody acceleration 
        Rigidbody manipulatedRigidbody = iGrabbedObject.GetComponent<Rigidbody>();
        if (manipulatedRigidbody != null)
        {
            m_ManipulatedObjectInitialIsKinematic = manipulatedRigidbody.isKinematic;
            manipulatedRigidbody.isKinematic = true;
        }

        // Deactivate selection during the manipulation
        vrInteraction selection = MiddleVR.VRInteractionMgr.GetActiveInteractionByTag("ContinuousSelection");
        if (selection != null)
        {
            m_PausedSelection = selection;
            MiddleVR.VRInteractionMgr.Deactivate(m_PausedSelection);
        }

        // Hide Wand
        m_VRMgr.ShowWandGeometry(false);
    }

    private void Ungrab()
    {
        if( m_CurrentManipulatedObject == null )
        {
            return;
        }

        // Give to return objects script
        VRInteractionManipulationReturnObjects returningObjectScript = this.GetComponent<VRInteractionManipulationReturnObjects>();
        if( returningObjectScript != null )
        {
            if (returningObjectScript.enabled)
            {
                returningObjectScript.AddReturningObject(m_CurrentManipulatedObject, m_ManipulatedObjectInitialLocalPosition,
                                                         m_ManipulatedObjectInitialLocalRotation, false);
            }
        }

        // Reset
        m_it.SetManipulatedNode(null);

        VRActor vrActorScript = m_CurrentManipulatedObject.GetComponent<VRActor>();
        vrActorScript.SyncDirection = m_ObjectPreviousSyncDir;

        // Show Wand
        m_VRMgr.ShowWandGeometry(true);

        // Unpause rigidbody acceleration 
        Rigidbody manipulatedRigidbody = m_CurrentManipulatedObject.GetComponent<Rigidbody>();
        if (manipulatedRigidbody != null)
        {
            manipulatedRigidbody.isKinematic = m_ManipulatedObjectInitialIsKinematic;
        }

        // Reactivate selection after the manipulation
        if (m_PausedSelection != null)
        {
            MiddleVR.VRInteractionMgr.Activate(m_PausedSelection);
            m_PausedSelection = null;
        }

        m_CurrentManipulatedObject = null;
    }
}

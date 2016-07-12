/* VRPhysicsBody
 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MiddleVR_Unity3D;

[AddComponentMenu("MiddleVR/Physics/Body")]
[RequireComponent(typeof(VRClusterObject))]
public class VRPhysicsBody : MonoBehaviour
{
    #region Member Variables

    [SerializeField]
    private bool m_Static = false;
    [SerializeField]
    private float m_Mass = 0.0f;
    [SerializeField]
    private double m_Margin = 0.0;
    [SerializeField]
    private double m_RotationDamping = 0.0;
    [SerializeField]
    private double m_TranslationDamping = 0.0;

    private vrPhysicsGeometry m_Geometry = null;
    private vrPhysicsBody m_PhysicsBody = null;
    private string m_PhysicsBodyName = "";

    private vrEventListener m_MVREventListener = null;

    #endregion

    #region Member Properties

    public float Mass
    {
        get
        {
            return m_Mass;
        }

        set
        {
            m_Mass = value;
        }
    }

    public vrPhysicsBody PhysicsBody
    {
        get
        {
            return m_PhysicsBody;
        }
    }

    public string PhysicsBodyName
    {
        get
        {
            return m_PhysicsBodyName;
        }
    }

    #endregion

    #region MonoBehaviour Member Functions

    protected void Start()
    {
        if (MiddleVR.VRClusterMgr.IsCluster() && !MiddleVR.VRClusterMgr.IsServer())
        {
            enabled = false;
            return;
        }

        if (MiddleVR.VRPhysicsMgr == null)
        {
            MiddleVRTools.Log(0, "[X] PhysicsBody: No PhysicsManager found.");
            enabled = false;

            return;
        }

        vrPhysicsEngine physicsEngine = MiddleVR.VRPhysicsMgr.GetPhysicsEngine();

        if (physicsEngine == null)
        {
            MiddleVRTools.Log(0, "[X] PhysicsBody: Failed to access a physics engine for body '" + name + "'.");
            enabled = false;

            return;
        }

        if (m_PhysicsBody == null)
        {
            m_PhysicsBody = physicsEngine.CreateBodyWithUniqueName(name);

            if (m_PhysicsBody == null)
            {
                MiddleVRTools.Log(0, "[X] PhysicsBody: Failed to create a physics body for '" + name + "'.");
                enabled = false;

                return;
            }
            else
            {
                GC.SuppressFinalize(m_PhysicsBody);

                m_MVREventListener = new vrEventListener(OnMVRNodeDestroy);
                m_PhysicsBody.AddEventListener(m_MVREventListener);

                var nodesMapper = MVRNodesMapper.Instance;

                nodesMapper.AddMapping(
                    gameObject, m_PhysicsBody,
                    MVRNodesMapper.ENodesSyncDirection.MiddleVRToUnity,
                    MVRNodesMapper.ENodesInitValueOrigin.FromUnity);

                m_PhysicsBodyName = m_PhysicsBody.GetName();

                string geometryName = m_PhysicsBodyName + ".Geometry";

                MiddleVRTools.Log(4,
                    "[>] PhysicsBody: Creation of the physics geometry '" +
                    geometryName + "'.");

                m_Geometry = new vrPhysicsGeometry(geometryName);
                GC.SuppressFinalize(m_Geometry);

                Mesh mesh = null;

                MeshCollider meshCollider = GetComponent<MeshCollider>();

                if (meshCollider != null)
                {
                    mesh = meshCollider.sharedMesh;

                    if (mesh != null)
                    {
                        MiddleVRTools.Log(2, "[ ] PhysicsBody: the physics geometry '" + geometryName +
                            "' uses the mesh of its MeshCollider.");
                    }
                }

                // No mesh from collider was found so let's try from the mesh filter.
                if (mesh == null)
                {
                    var meshFilter = GetComponent<MeshFilter>();

                    if (meshFilter != null)
                    {
                        mesh = meshFilter.sharedMesh;

                        if (mesh != null)
                        {
                            MiddleVRTools.Log(2, "[ ] PhysicsBody: the physics geometry '" + geometryName +
                                "' uses the mesh of its MeshFilter.");
                        }
                    }
                }

                if (mesh != null)
                {
                    ConvertGeometry(mesh);
                    MiddleVRTools.Log(4, "[ ] PhysicsBody: Physics geometry created.");
                }
                else
                {
                    MiddleVRTools.Log(
                        0,
                        "[X] PhysicsBody: Failed to create the physics geometry '" +
                        geometryName + "'.");
                }

                MiddleVRTools.Log(4,
                    "[<] PhysicsBody: Creation of the physics geometry '" +
                    geometryName + "' ended.");

                m_PhysicsBody.SetGeometry(m_Geometry);

                m_PhysicsBody.SetStatic(m_Static);
                m_PhysicsBody.SetMass(m_Mass);
                m_PhysicsBody.SetRotationDamping(m_RotationDamping);
                m_PhysicsBody.SetTranslationDamping(m_TranslationDamping);

                m_PhysicsBody.SetMargin(m_Margin);

                if (physicsEngine.AddBody(m_PhysicsBody))
                {
                    MiddleVRTools.Log(3, "[ ] PhysicsBody: The physics body '" + m_PhysicsBodyName +
                        "' was added to the physics simulation.");
                }
                else
                {
                    MiddleVRTools.Log(3, "[X] PhysicsBody: Failed to add the body '" +
                         m_PhysicsBodyName + "' to the physics simulation.");
                }
            }
        }
    }

    protected void Update()
    {
        if (m_Geometry != null && m_PhysicsBody.IsInSimulation())
        {
            // The geometry was used for creation so it can be deleted.

            // Clear now content to avoid a delayed memory deallocation.
            m_Geometry.Clear();
            m_Geometry.Dispose();

            m_Geometry = null;
        }
    }

    protected void OnDestroy()
    {
        if (m_PhysicsBody != null)
        {
            if (MVRNodesMapper.HasInstance())
            {
                var nodesMapper = MVRNodesMapper.Instance;
                nodesMapper.RemoveMapping(gameObject);
            }

            if (MiddleVR.VRPhysicsMgr != null)
            {
                vrPhysicsEngine physicsEngine = MiddleVR.VRPhysicsMgr.GetPhysicsEngine();
                if (physicsEngine != null)
                {
                    physicsEngine.DestroyBody(m_PhysicsBodyName);
                }
            }

            m_PhysicsBody.Dispose();

            m_PhysicsBody = null;
        }

        m_PhysicsBodyName = "";

        if (m_MVREventListener != null)
        {
            m_MVREventListener.Dispose();
        }
    }

    #endregion

    #region VRPhysicsBody Functions

    private bool OnMVRNodeDestroy(vrEvent iBaseEvt)
    {
        vrObjectEvent e = vrObjectEvent.Cast(iBaseEvt);
        if (e != null)
        {
            if (e.ComesFrom(m_PhysicsBody))
            {
                if (e.eventType == (int)VRObjectEventEnum.VRObjectEvent_Destroy)
                {
                    // Killed in MiddleVR so delete it in C#.
                    m_PhysicsBody.Dispose();
                }
            }
        }

        return true;
    }

    private void ConvertGeometry(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        MiddleVRTools.Log(3, "PhysicsBody: Number of vertices: " + vertices.Length);
        MiddleVRTools.Log(3, "PhysicsBody: Number of Triangles: " + triangles.Length);

        // We will reuse the same vectors to avoid many memory allocations.
        Vector3 vertexPos = new Vector3();
        vrVec3 vPos = new vrVec3();

        // We compute a matrix to scale vertices according to their world
        // coordinates, so this scale depends on the scales of the GameObject
        // parents. Matrices 4x4 are used because matrices 3x3 aren't available.
        vrMatrix worldMatrix = MVRTools.RawFromUnity(transform.localToWorldMatrix);
        worldMatrix.SetCol(3, new vrVec4(0.0f, 0.0f, 0.0f, 0.0f));
        worldMatrix.SetRow(3, new vrVec4(0.0f, 0.0f, 0.0f, 0.0f));

        vrQuat invRotWorlQ = worldMatrix.GetRotation();
        invRotWorlQ = invRotWorlQ.Normalize().GetInverse();

        vrMatrix invRotWorldMatrix = new vrMatrix();
        invRotWorldMatrix.SetRotation(invRotWorlQ);
        invRotWorldMatrix.SetCol(3, new vrVec4(0.0f, 0.0f, 0.0f, 0.0f));
        invRotWorldMatrix.SetRow(3, new vrVec4(0.0f, 0.0f, 0.0f, 0.0f));

        vrMatrix scaleWorldMatrix = invRotWorldMatrix.Mult(worldMatrix);
        Matrix4x4 scaleWorldMatrixUnity = MiddleVRTools.RawToUnity(scaleWorldMatrix);

        foreach (Vector3 vertex in vertices)
        {
            vertexPos = scaleWorldMatrixUnity.MultiplyPoint3x4(vertex);

            MiddleVRTools.FromUnity(vertexPos, ref vPos);

            m_Geometry.AddVertex(vPos);

            MiddleVRTools.Log(6, "PhysicsBody: Adding a vertex at position (" +
                vPos.x() + ", " + vPos.y() + ", " + vPos.z() + ").");
        }

        for (int i = 0, iEnd = triangles.Length; i < iEnd; i += 3)
        {
            uint index0 = (uint)triangles[i];
            uint index1 = (uint)triangles[i + 1];
            uint index2 = (uint)triangles[i + 2];

            m_Geometry.AddTriangle(index0, index1, index2);

            MiddleVRTools.Log(6, "PhysicsBody: Adding a triangle with vertex indexes (" +
                index0 + ", " + index1 + ", " + index2 + ").");
        }
    }

    #endregion
}

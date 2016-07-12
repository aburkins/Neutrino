/* VRMenu
 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using System.Collections.Generic;
using MiddleVR_Unity3D;

[AddComponentMenu("")]
public class VRMenu : MonoBehaviour
{
    private VRManagerScript m_VRManager;

    private vrGUIRendererWeb m_GUIRendererWeb = null;
    private vrWidgetMenu     m_Menu = null;

    public vrGUIRendererWeb guiRendererWeb
    {
        get
        {
            return m_GUIRendererWeb;
        }
    }

    public vrWidgetMenu menu
    {
        get
        {
            return m_Menu;
        }
    }

    // General
    private vrWidgetMenu         m_ResetButtonMenu;
    private vrWidgetButton       m_ResetCurrentButton;
    private vrWidgetButton       m_ResetZeroButton;
    private vrWidgetMenu         m_ExitButtonMenu;
    private vrWidgetButton       m_ExitButton;
    private vrWidgetMenu         m_GeneralOptions;
    private vrWidgetToggleButton m_FramerateCheckbox;
    private vrWidgetToggleButton m_ProxiWarningCheckbox;
    private vrWidgetSeparator    m_GeneralSeparator;

    // Navigation
    private vrWidgetMenu         m_NavigationOptions;
    private vrWidgetSeparator    m_NavigationSeparator;
    private vrWidgetToggleButton m_FlyCheckbox;
    private vrWidgetToggleButton m_CollisionsCheckbox;

    // Manipulation
    private vrWidgetMenu         m_ManipulationOptions;
    private vrWidgetSeparator    m_ManipulationSeparator;
    private vrWidgetToggleButton m_ReturnObjectsCheckbox;

    // Virtual Hand
    private vrWidgetMenu        m_VirtualHandOptions;

    // General
    private vrCommand m_ResetCurrentButtonCommand;
    private vrCommand m_ResetZeroButtonCommand;
    private vrCommand m_ExitButtonCommand;
    private vrCommand m_FramerateCheckboxCommand;
    private vrCommand m_ProxiWarningCheckboxCommand;

    // Navigation
    private vrCommand m_NavigationModeRadioCommand;
    private vrCommand m_FlyCheckboxCommand;
    private vrCommand m_CollisionsCheckboxCommand;

    // Manipulation
    private vrCommand m_ManipulationModeRadioCommand;
    private vrCommand m_ReturnObjectsCheckboxCommand;

    // Virtual Hand
    private vrCommand m_VirtualHandModeRadioCommand;

    // Bind with Interaction events
    private vrEventListener m_Listener;
    private Dictionary<string, vrCommand> m_Commands = new Dictionary<string, vrCommand>();
    private Dictionary<string, vrWidgetToggleButton> m_Buttons = new Dictionary<string, vrWidgetToggleButton>();

    private bool EventListener(vrEvent iEvent)
    {
        // Catch interaction events
        vrInteractionEvent interactionEvt = vrInteractionEvent.Cast(iEvent);
        if (interactionEvt != null)
        {
            vrInteraction interaction = interactionEvt.GetInteraction();

            bool needLabelRefresh = false;

            // Identify interaction
            // If existing in the Menu, update the menu
            if (interactionEvt.GetEventType() == (int)VRInteractionEventEnum.VRInteractionEvent_Activated)
            {
                vrWidgetToggleButton interactionButton;

                if (m_Buttons.TryGetValue(interaction.GetName(), out interactionButton))
                {
                    interactionButton.SetChecked(true);
                }

                needLabelRefresh = true;
            }
            else if (interactionEvt.GetEventType() == (int)VRInteractionEventEnum.VRInteractionEvent_Deactivated)
            {
                vrWidgetToggleButton interactionButton;

                if (m_Buttons.TryGetValue(interaction.GetName(), out interactionButton))
                {
                    interactionButton.SetChecked(false);
                }

                needLabelRefresh = true;
            }

            // Refresh interaction menu label if activated or deactivated
            if (needLabelRefresh)
            {
                if (interaction.TagsContain("ContinuousNavigation"))
                {
                    _RefreshNavigationMenuName();
                }
                else if (interaction.TagsContain("ContinuousManipulation"))
                {
                    _RefreshManipulationMenuName();
                }
                else if (interaction.TagsContain("VirtualHand"))
                {
                    _RefreshVirtualHandMenuName();
                }
            }
        }

        return true;
    }

    // General
    private vrValue ResetCurrentButtonHandler(vrValue iValue)
    {
        MVRTools.Log("[ ] Reload current level.");
        Application.LoadLevel(Application.loadedLevel);
        return null;
    }

    private vrValue ResetZeroButtonHandler(vrValue iValue)
    {
        MVRTools.Log("[ ] Reload level zero.");
        Application.LoadLevel(0);
        return null;
    }

    private vrValue ExitButtonHandler(vrValue iValue)
    {
        MVRTools.Log("[ ] Exit simulation.");
        m_VRManager.QuitApplication();
        return null;
    }

    private vrValue FramerateCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ShowFPS = iValue.GetBool();
        MVRTools.Log("[ ] Show Frame Rate: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue ProxiWarningCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ShowScreenProximityWarnings = iValue.GetBool();
        MVRTools.Log("[ ] Show proximity warnings: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue NavigationJoystickHandler(vrValue iValue)
    {
        // Activate Joystick Navigation
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationWandJoystick");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Navigation Joystick activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Navigation Joystick deactivated.");
        }

        return null;
    }

    private vrValue NavigationElasticHandler(vrValue iValue)
    {
        // Activate Elastic Navigation
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationElastic");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Navigation Elastic activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Navigation Elastic deactivated.");
        }

        return null;
    }

    private vrValue NavigationGrabWorldHandler(vrValue iValue)
    {
        // Activate Grab World Navigation
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationGrabWorld");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Navigation Grab World activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Navigation Grab World deactivated.");
        }

        return null;
    }

    private vrValue FlyCheckboxHandler(vrValue iValue)
    {
        m_VRManager.Fly = iValue.GetBool();
        MVRTools.Log("[ ] Fly mode: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue CollisionsCheckboxHandler(vrValue iValue)
    {
        m_VRManager.NavigationCollisions = iValue.GetBool();
        MVRTools.Log("[ ] Navigation Collisions: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue ManipulationRayHandler(vrValue iValue)
    {
        // Activate Ray Manipulation
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionManipulationRay");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Manipulation Ray activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Manipulation Ray deactivated.");
        }

        return null;
    }

    private vrValue ManipulationHomerHandler(vrValue iValue)
    {
        // Activate Homer Manipulation
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionManipulationHomer");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Manipulation Homer activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Manipulation Homer deactivated.");
        }

        return null;
    }

    private vrValue ReturnObjectsCheckboxHandler(vrValue iValue)
    {
        m_VRManager.ManipulationReturnObjects = iValue.GetBool();
        MVRTools.Log("[ ] Manipulation return objects: " + iValue.GetBool().ToString());
        return null;
    }

    private vrValue VirtualHandGogoButtonHandler(vrValue iValue)
    {
        // Activate Gogo Virtual Hand
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetInteraction("InteractionVirtualHandGogo");

        bool activate = iValue.GetBool();
        if (activate)
        {
            MiddleVR.VRInteractionMgr.Activate(interaction);
            MVRTools.Log("[ ] Virtual Hand Gogo activated.");
        }
        else
        {
            MiddleVR.VRInteractionMgr.Deactivate(interaction);
            MVRTools.Log("[ ] Virtual Hand Gogo deactivated.");
        }

        return null;
    }

    protected void Start ()
    {
        // Retrieve the VRManager
        VRManagerScript[] foundVRManager = FindObjectsOfType(typeof(VRManagerScript)) as VRManagerScript[];
        if (foundVRManager.Length != 0)
        {
            m_VRManager = foundVRManager[0];
        }
        else
        {
            MVRTools.Log("[X] VRMenu: impossible to retrieve the VRManager.");
            return;
        }

        // Start listening to MiddleVR events
        m_Listener = new vrEventListener(EventListener);
        MiddleVR.VRInteractionMgr.AddEventListener(m_Listener);

        // Create commands

        // General
        m_ResetCurrentButtonCommand   = new vrCommand("VRMenu.ResetCurrentButtonCommand", ResetCurrentButtonHandler);
        m_ResetZeroButtonCommand      = new vrCommand("VRMenu.ResetZeroButtonCommand", ResetZeroButtonHandler);
        m_ExitButtonCommand           = new vrCommand("VRMenu.ExitButtonCommand", ExitButtonHandler);
        m_FramerateCheckboxCommand    = new vrCommand("VRMenu.FramerateCheckboxCommand", FramerateCheckboxHandler);
        m_ProxiWarningCheckboxCommand = new vrCommand("VRMenu.ProxiWarningCheckboxCommand", ProxiWarningCheckboxHandler);

        // Navigation
        m_FlyCheckboxCommand          = new vrCommand("VRMenu.FlyCheckboxCommand", FlyCheckboxHandler);
        m_CollisionsCheckboxCommand   = new vrCommand("VRMenu.CollisionsCheckboxCommand", CollisionsCheckboxHandler);

        // Manipulation
        m_ReturnObjectsCheckboxCommand = new vrCommand("VRMenu.ReturnObjectsCheckboxCommand", ReturnObjectsCheckboxHandler);

        // Create GUI
        m_GUIRendererWeb = null;

        VRWebView webViewScript = GetComponent<VRWebView>();

        if (webViewScript == null)
        {
            MVRTools.Log(1, "[X] VRMenu does not have a WebView.");
            return;
        }

        m_GUIRendererWeb = new vrGUIRendererWeb("", webViewScript.webView);

        m_Menu = new vrWidgetMenu("VRMenu.VRManagerMenu", m_GUIRendererWeb);

        // Navigation
        m_NavigationOptions = new vrWidgetMenu("VRMenu.NavigationOptions", m_Menu, "Navigation");

        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationWandJoystick"), "Joystick", m_NavigationOptions, NavigationJoystickHandler);
        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationElastic"), "Elastic", m_NavigationOptions, NavigationElasticHandler);
        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionNavigationGrabWorld"), "Grab The World", m_NavigationOptions, NavigationGrabWorldHandler);

        m_NavigationSeparator = new vrWidgetSeparator("VRMenu.NavigationSeparator", m_NavigationOptions);
        m_FlyCheckbox         = new vrWidgetToggleButton("VRMenu.FlyCheckbox", m_NavigationOptions, "Fly", m_FlyCheckboxCommand, m_VRManager.Fly);
        m_CollisionsCheckbox  = new vrWidgetToggleButton("VRMenu.CollisionsCheckbox", m_NavigationOptions, "Navigation Collisions", m_CollisionsCheckboxCommand, m_VRManager.NavigationCollisions);

        // Manipulation
        m_ManipulationOptions = new vrWidgetMenu("VRMenu.ManipulationOptions", m_Menu, "Manipulation");

        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionManipulationRay"), "Ray", m_ManipulationOptions, ManipulationRayHandler);
        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionManipulationHomer"), "Homer", m_ManipulationOptions, ManipulationHomerHandler);

        m_ManipulationSeparator = new vrWidgetSeparator("VRMenu.ManipulationSeparator", m_ManipulationOptions);
        m_ReturnObjectsCheckbox = new vrWidgetToggleButton("VRMenu.ReturnObjectsCheckbox", m_ManipulationOptions, "Return Objects", m_ReturnObjectsCheckboxCommand, m_VRManager.ManipulationReturnObjects);

        // Virtual Hand
        m_VirtualHandOptions = new vrWidgetMenu("VRMenu.VirtualHandOptions", m_Menu, "Virtual Hand");

        CreateInteractionToggleButton(MiddleVR.VRInteractionMgr.GetInteraction("InteractionVirtualHandGogo"), "Gogo", m_VirtualHandOptions, VirtualHandGogoButtonHandler);

        // General
        m_GeneralSeparator = new vrWidgetSeparator("VRMenu.GeneralSeparator", m_Menu);
        m_GeneralOptions   = new vrWidgetMenu("VRMenu.GeneralOptions", m_Menu, "General Options");

        m_FramerateCheckbox    = new vrWidgetToggleButton("VRMenu.FramerateCheckbox", m_GeneralOptions, "Show Frame Rate", m_FramerateCheckboxCommand, m_VRManager.ShowFPS);
        m_ProxiWarningCheckbox = new vrWidgetToggleButton("VRMenu.ProxiWarningCheckbox", m_GeneralOptions, "Show Proximity Warning", m_ProxiWarningCheckboxCommand, m_VRManager.ShowScreenProximityWarnings);

        // Reset and Exit
        m_ResetButtonMenu    = new vrWidgetMenu("VRMenu.ResetButtonMenu", m_Menu, "Reset Simulation");
        m_ResetCurrentButton = new vrWidgetButton("VRMenu.ResetCurrentButton", m_ResetButtonMenu, "Reload current level", m_ResetCurrentButtonCommand);
        m_ResetZeroButton    = new vrWidgetButton("VRMenu.ResetZeroButton", m_ResetButtonMenu, "Reload level zero", m_ResetZeroButtonCommand);

        m_ExitButtonMenu = new vrWidgetMenu("VRMenu.ExitButtonMenu", m_Menu, "Exit Simulation");
        m_ExitButton     = new vrWidgetButton("VRMenu.ExitButton", m_ExitButtonMenu, "Yes, Exit Simulation", m_ExitButtonCommand);
    }

    public void CreateInteractionToggleButton(vrInteraction iInteraction, string iButtonName, vrWidgetMenu iParentMenu, vrCommand.Delegate iButtonHandler)
    {
        string itName = iInteraction.GetName();

        vrCommand newCommand = new vrCommand("VRMenu." + itName + "ToggleCommand", iButtonHandler);
        m_Commands.Add(itName, newCommand);

        vrWidgetToggleButton button = new vrWidgetToggleButton("VRMenu." + itName + "ToggleButton", iParentMenu, iButtonName, newCommand, iInteraction.IsActive());
        m_Buttons.Add(itName, button);
    }

    private void _RefreshNavigationMenuName()
    {
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetActiveInteractionByTag("ContinuousNavigation");
        if (interaction != null)
        {
            switch (interaction.GetName())
            {
                case "InteractionNavigationWandJoystick":
                    {
                        m_NavigationOptions.SetLabel("Navigation (Joystick)");
                        break;
                    }
                case "InteractionNavigationElastic":
                    {
                        m_NavigationOptions.SetLabel("Navigation (Elastic)");
                        break;
                    }
                case "InteractionNavigationGrabWorld":
                    {
                        m_NavigationOptions.SetLabel("Navigation (Grab The World)");
                        break;
                    }
                default:
                    {
                        m_NavigationOptions.SetLabel("Navigation (" + interaction.GetName() + ")");
                        break;
                    }
            }
        }
        else
        {
            m_NavigationOptions.SetLabel("Navigation");
        }
    }

    private void _RefreshManipulationMenuName()
    {
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetActiveInteractionByTag("ContinuousManipulation");
        if (interaction != null)
        {
            switch (interaction.GetName())
            {
                case "InteractionManipulationRay":
                    {
                        m_ManipulationOptions.SetLabel("Manipulation (Ray)");
                        break;
                    }
                case "InteractionManipulationHomer":
                    {
                        m_ManipulationOptions.SetLabel("Manipulation (Homer)");
                        break;
                    }
                default:
                    {
                        m_ManipulationOptions.SetLabel("Manipulation (" + interaction.GetName() + ")");
                        break;
                    }
            }
        }
        else
        {
            m_ManipulationOptions.SetLabel("Manipulation");
        }
    }

    private void _RefreshVirtualHandMenuName()
    {
        vrInteraction interaction = MiddleVR.VRInteractionMgr.GetActiveInteractionByTag("VirtualHand");
        if (interaction != null)
        {
            switch (interaction.GetName())
            {
                case "InteractionVirtualHandGogo":
                    {
                        m_VirtualHandOptions.SetLabel("Virtual Hand (Gogo)");
                        break;
                    }
                default:
                    {
                        m_VirtualHandOptions.SetLabel("Virtual Hand (" + interaction.GetName() + ")");
                        break;
                    }
            }
        }
        else
        {
            m_VirtualHandOptions.SetLabel("Virtual Hand");
        }
    }

    private void OnDestroy()
    {
        // Listener
        MiddleVR.DisposeObject(ref m_Listener);

        // Commands
        foreach (var item in m_Commands)
        {
            MiddleVR.DisposeObject(item.Value);
        }
        m_Commands.Clear();

        MiddleVR.DisposeObject(ref m_ResetCurrentButtonCommand);
        MiddleVR.DisposeObject(ref m_ResetZeroButtonCommand);
        MiddleVR.DisposeObject(ref m_ExitButtonCommand);
        MiddleVR.DisposeObject(ref m_FramerateCheckboxCommand);
        MiddleVR.DisposeObject(ref m_ProxiWarningCheckboxCommand);
        MiddleVR.DisposeObject(ref m_NavigationModeRadioCommand);
        MiddleVR.DisposeObject(ref m_FlyCheckboxCommand);
        MiddleVR.DisposeObject(ref m_CollisionsCheckboxCommand);
        MiddleVR.DisposeObject(ref m_ManipulationModeRadioCommand);
        MiddleVR.DisposeObject(ref m_ReturnObjectsCheckboxCommand);
        MiddleVR.DisposeObject(ref m_VirtualHandModeRadioCommand);

        // Widgets
        foreach (var item in m_Buttons)
        {
            MiddleVR.DisposeObject(item.Value);
        }
        m_Buttons.Clear();

        MiddleVR.DisposeObject(ref m_ResetButtonMenu);
        MiddleVR.DisposeObject(ref m_ResetCurrentButton);
        MiddleVR.DisposeObject(ref m_ResetZeroButton);
        MiddleVR.DisposeObject(ref m_ExitButtonMenu);
        MiddleVR.DisposeObject(ref m_ExitButton);
        MiddleVR.DisposeObject(ref m_GeneralOptions);
        MiddleVR.DisposeObject(ref m_FramerateCheckbox);
        MiddleVR.DisposeObject(ref m_ProxiWarningCheckbox);
        MiddleVR.DisposeObject(ref m_GeneralSeparator);
        MiddleVR.DisposeObject(ref m_NavigationOptions);
        MiddleVR.DisposeObject(ref m_NavigationSeparator);
        MiddleVR.DisposeObject(ref m_FlyCheckbox);
        MiddleVR.DisposeObject(ref m_CollisionsCheckbox);
        MiddleVR.DisposeObject(ref m_ManipulationOptions);
        MiddleVR.DisposeObject(ref m_ManipulationSeparator);
        MiddleVR.DisposeObject(ref m_ReturnObjectsCheckbox);
        MiddleVR.DisposeObject(ref m_VirtualHandOptions);
        MiddleVR.DisposeObject(ref m_Menu);

        // GUIRenderer
        MiddleVR.DisposeObject(ref m_GUIRendererWeb);
    }
}

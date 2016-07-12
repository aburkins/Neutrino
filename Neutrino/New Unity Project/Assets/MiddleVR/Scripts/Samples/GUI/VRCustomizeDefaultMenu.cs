/* VRCustomizeDefaultMenu

 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using System.Collections;
using MiddleVR_Unity3D;

[AddComponentMenu("MiddleVR/Samples/GUI/Customize Default Menu")]
public class VRCustomizeDefaultMenu : MonoBehaviour
{
    // Start waits on VRMenu creation with a coroutine
    IEnumerator Start()
    {
        VRMenu MiddleVRMenu = null;
        while (MiddleVRMenu == null || MiddleVRMenu.menu == null)
        {
            // Wait for VRMenu to be created
            yield return null;
            MiddleVRMenu = FindObjectOfType(typeof(VRMenu)) as VRMenu;
        }

        AddButton(MiddleVRMenu);
        RemoveItem(MiddleVRMenu);
        MoveItems(MiddleVRMenu);

        // End coroutine
        yield break;
    }
    private void OnDestroy()
    {
        MiddleVR.DisposeObject(ref m_MyItemCommand);
    }

    private vrCommand m_MyItemCommand = null;
    vrValue MyItemCommandHandler(vrValue iValue)
    {
        print("My menu item has been clicked");
        return null;
    }

    private void AddButton(VRMenu iVRMenu)
    {
        // Add a button at the start of the menu
        m_MyItemCommand = new vrCommand("VRMenu.MyCustomButtonCommand", MyItemCommandHandler);

        vrWidgetButton button = new vrWidgetButton("VRMenu.MyCustomButton", iVRMenu.menu, "My Menu Item", m_MyItemCommand);
        iVRMenu.menu.SetChildIndex(button, 0);

        // Add a separator below it
        vrWidgetSeparator separator = new vrWidgetSeparator("VRMenu.MyCustomSeparator", iVRMenu.menu);
        iVRMenu.menu.SetChildIndex(separator, 1);
    }

    private void RemoveItem(VRMenu iVRMenu)
    {
        // Remove "Reset" submenu
        for (uint i = 0; i < iVRMenu.menu.GetChildrenNb(); ++i)
        {
            vrWidget widget = iVRMenu.menu.GetChild(i);
            if( widget.GetLabel().Contains("Reset"))
            {
                iVRMenu.menu.RemoveChild(widget);
                break;
            }
        }   
    }

    private void MoveItems(VRMenu iVRMmenu)
    {
        // Move every menu item under a sub menu
        vrWidgetMenu subMenu = new vrWidgetMenu("VRMenu.MyNewSubMenu", null, "MiddleVR Menu");

        while (iVRMmenu.menu.GetChildrenNb() > 0)
        {
            vrWidget widget = iVRMmenu.menu.GetChild(0);
            widget.SetParent(subMenu);
        }

        subMenu.SetParent(iVRMmenu.menu);
    }
}

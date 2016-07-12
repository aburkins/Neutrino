/* VRGUIMenuSample

 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;
using MiddleVR_Unity3D;

[AddComponentMenu("MiddleVR/Samples/GUI/Menu")]
public class VRGUIMenuSample : MonoBehaviour {

    private vrGUIRendererWeb m_GUIRendererWeb;
    private vrWidgetMenu m_Menu;
    private vrWidgetButton m_Button1;
    private vrWidgetToggleButton m_Checkbox;
    private vrWidgetMenu m_Submenu;
    private vrWidgetRadioButton m_Radio1;
    private vrWidgetRadioButton m_Radio2;
    private vrWidgetRadioButton m_Radio3;
    private vrWidgetColorPicker m_Picker;
    private vrWidgetSlider m_Slider;
    private vrWidgetList m_List;

    private vrCommand m_ButtonCommand;
    private vrCommand m_CheckboxCommand;
    private vrCommand m_RadioCommand;
    private vrCommand m_ColorPickerCommand;
    private vrCommand m_SliderCommand;
    private vrCommand m_ListCommand;

    private vrValue ButtonHandler(vrValue iValue)
    {
        m_Checkbox.SetChecked( ! m_Checkbox.IsChecked() );
        print("ButtonHandler() called");
        return null;
    }

    private vrValue CheckboxHandler(vrValue iValue)
    {
        print("Checkbox value : " + iValue.GetBool().ToString());
        return null;
    }
    
    private vrValue RadioHandler(vrValue iValue)
    {
        print("Radio value : " + iValue.GetString());
        return null;
    }
    
    private vrValue ColorPickerHandler(vrValue iValue)
    {
        vrVec4 color = iValue.GetVec4();
        print("Selected color : " + color.x().ToString() + " " + color.y().ToString() + " " + color.z().ToString());
        return null;
    }
    
    private vrValue SliderHandler(vrValue iValue)
    {
        print("Slider value : " + iValue.GetFloat().ToString());
        return null;
    }
    
    private vrValue ListHandler(vrValue iValue)
    {
        print( "List Selected Index : " + iValue.GetInt() );
        return null;
    }

    private void Start()
    {
        // Create commands

        m_ButtonCommand = new vrCommand("GUIMenuSample.ButtonCommand", ButtonHandler);
        m_CheckboxCommand = new vrCommand("GUIMenuSample.CheckboxCommand", CheckboxHandler);
        m_RadioCommand = new vrCommand("GUIMenuSample.RadioCommand", RadioHandler);
        m_ColorPickerCommand = new vrCommand("GUIMenuSample.ColorPickerCommand", ColorPickerHandler);
        m_SliderCommand = new vrCommand("GUIMenuSample.SliderCommand", SliderHandler);
        m_ListCommand = new vrCommand("GUIMenuSample.ListCommand", ListHandler);

        // Create GUI

        m_GUIRendererWeb = null;

        VRWebView webViewScript = GetComponent<VRWebView>();

        if (webViewScript == null)
        {
            MVRTools.Log(1, "[X] VRGUIMenuSample does not have a WebView.");
            return;
        }

        m_GUIRendererWeb = new vrGUIRendererWeb("", webViewScript.webView);

        m_Menu = new vrWidgetMenu("GUIMenuSample.MainMenu", m_GUIRendererWeb);

        m_Button1 = new vrWidgetButton("GUIMenuSample.Button1", m_Menu, "Button", m_ButtonCommand);

        new vrWidgetSeparator("GUIMenuSample.Separator1", m_Menu);

        m_Checkbox = new vrWidgetToggleButton("GUIMenuSample.Checkbox", m_Menu, "Toggle Button", m_CheckboxCommand, true);

        m_Submenu = new vrWidgetMenu("GUIMenuSample.SubMenu", m_Menu, "Sub Menu");
        m_Submenu.SetVisible(true);

        m_Radio1 = new vrWidgetRadioButton("GUIMenuSample.Radio1", m_Submenu, "Huey", m_RadioCommand, "Huey");
        m_Radio2 = new vrWidgetRadioButton("GUIMenuSample.Radio2", m_Submenu, "Dewey", m_RadioCommand, "Dewey");
        m_Radio3 = new vrWidgetRadioButton("GUIMenuSample.Radio3", m_Submenu, "Louie", m_RadioCommand, "Louie");

        m_Picker = new vrWidgetColorPicker("GUIMenuSample.ColorPicker", m_Menu, "Color Picker", m_ColorPickerCommand, new vrVec4(0, 0, 0, 0));

        m_Slider = new vrWidgetSlider("GUIMenuSample.Slider", m_Menu, "Slider", m_SliderCommand, 50.0f, 0.0f, 100.0f, 1.0f);
        
        vrValue listContents = vrValue.CreateList();
        listContents.AddListItem( "Item 1" );
        listContents.AddListItem( "Item 2" );

        m_List = new vrWidgetList("GUIMenuSample.List", m_Menu, "List", m_ListCommand, listContents, 0);
    }

    private void OnDestroy()
    {
        MiddleVR.DisposeObject(ref m_ButtonCommand);
        MiddleVR.DisposeObject(ref m_CheckboxCommand);
        MiddleVR.DisposeObject(ref m_RadioCommand);
        MiddleVR.DisposeObject(ref m_ColorPickerCommand);
        MiddleVR.DisposeObject(ref m_SliderCommand);
        MiddleVR.DisposeObject(ref m_ListCommand);

        MiddleVR.DisposeObject(ref m_Button1);
        MiddleVR.DisposeObject(ref m_Checkbox);
        MiddleVR.DisposeObject(ref m_Submenu);
        MiddleVR.DisposeObject(ref m_Radio1);
        MiddleVR.DisposeObject(ref m_Radio2);
        MiddleVR.DisposeObject(ref m_Radio3);
        MiddleVR.DisposeObject(ref m_Picker);
        MiddleVR.DisposeObject(ref m_Slider);
        MiddleVR.DisposeObject(ref m_List);

        MiddleVR.DisposeObject(ref m_Menu);

        MiddleVR.DisposeObject(ref m_GUIRendererWeb);
    }
}

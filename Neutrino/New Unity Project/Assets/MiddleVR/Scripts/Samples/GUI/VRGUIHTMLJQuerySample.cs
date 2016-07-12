/* VRGUIHTMLJQuerySample

 * MiddleVR
 * (c) MiddleVR
 */

/*
 * 
 * Use with data/GUI/HTMLJQuerySample/index.html in MiddleVR install directory
 * 
 */

using UnityEngine;

[AddComponentMenu("MiddleVR/Samples/GUI/HTML JQuery")]
public class VRGUIHTMLJQuerySample : MonoBehaviour
{
	private vrCommand m_ButtonCommand;
	private vrCommand m_RadioCommand;
	private vrCommand m_SliderCommand;

	private int m_Progress = 0;

	private vrValue ButtonHandler(vrValue iValue)
	{
		m_Progress += 1;

		GetComponent<VRWebView>().webView.ExecuteJavascript(
			"$('#progressbar').progressbar('value'," + m_Progress.ToString() + ");");

		return null;
	}

	private vrValue RadioHandler(vrValue iValue)
	{
		if(iValue.IsString())
		{
			Debug.Log("Radio value = " + iValue.GetString() );
		}
		return null;
	}

	private vrValue SliderHandler(vrValue iValue)
	{
		if (iValue.IsNumber())
		{
			Debug.Log("Slider value as Number = " + iValue.GetNumber() );
		}
		return null;
	}
	
	private void Start()
	{
		m_ButtonCommand = new vrCommand("ButtonCommand", ButtonHandler);
		m_RadioCommand = new vrCommand("RadioCommand", RadioHandler);
		m_SliderCommand = new vrCommand("SliderCommand", SliderHandler);
	}

    private void OnDestroy()
    {
        MiddleVR.DisposeObject(ref m_ButtonCommand);
        MiddleVR.DisposeObject(ref m_RadioCommand);
        MiddleVR.DisposeObject(ref m_SliderCommand);
    }
}

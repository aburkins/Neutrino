/* VRGUIHTMLBasicSample

 * MiddleVR
 * (c) MiddleVR
 */

using UnityEngine;

/*
 * 
 * Use with data/GUI/HTMLBasicSample/index.html in MiddleVR install directory
 * 
 */
[AddComponentMenu("MiddleVR/Samples/GUI/HTML Basic")]
public class VRGUIHTMLBasicSample : MonoBehaviour
{
    private vrCommand m_MyCommand = null;

    private vrValue CommandHandler(vrValue iValue)
    {
        print("HTML Button was clicked");

        // Uncomment the following lines to have modify the HTML page in response !
        //vrWebView webView = GetComponent<VRWebView>().webView;
        //webView.ExecuteJavascript("AddResult('Button was clicked !')");

        return null;
    }

    private void Start()
    {
        m_MyCommand = new vrCommand("MyCommand", CommandHandler);
    }

    private void OnDestroy()
    {
        MiddleVR.DisposeObject(ref m_MyCommand);
    }
}

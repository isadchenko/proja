using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections ;

namespace TouchworkSDK
{
	public sealed class Touchwork
	{
        //Constructor
        public Touchwork()
        {
            TouchMessage.InitializeTouchMessages(); 
        }
        /// <summary>
        /// Allows a given ITouchable control to recieve touch events.
        /// </summary>
        /// <param name="touchableControl"></param>
		static public void NotifyOnTouch(ITouchable touchableControl)
		{
			WndProcHooker.HookWndProc(touchableControl);
            
            TouchMessage.InitializeTouchMessages(); 
            
		}

        /// <summary>
        /// Allows a given ITouchable control to recieve touch events.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="callback"></param>
        static public void NotifyOnTouch(Control control, ITouchable callback)
        {
            WndProcHooker.HookWndProc(control, callback);
            TouchMessage.InitializeTouchMessages(); 
        }

        /// <summary>
        /// UnsubscribeToTouchEvents prevent a control from receiving touch events.
        /// </summary>
        /// <param name="ctrl"></param>
		static public  void UnsubscribeToTouchEvents(Control ctrl)
		{
			WndProcHooker.UnhookWndProc(ctrl,false);
		}

        /// <summary>
        /// This function is used to raise the touch events in any application using this dll
        /// </summary>
        /// <param name="touchable"></param>
        /// <param name="ctrl"></param>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <param name="mTouches"></param>
        /// <returns></returns>
		static public int TouchworkWindowProc(
			ITouchable touchable, Control ctrl,IntPtr hwnd, uint msg, uint wParam, uint lParam, ref bool handled, List<Touch> mTouches)
		{
			TouchEventArgs e; // This event should be filled
            Touch touch;
            
            if (msg == TouchMessage.WM_IS_TOUCHABLE)
            {
                return 5; 
            }
            
            if (msg == TouchMessage.WM_TOUCH_DOWN)
            {
                e = new TouchEventArgs(lParam, wParam);
                touchable.OnTouchDown(ctrl, e);
                handled = e.handled;
                touch = new Touch(e.ID, e.point);
                mTouches.Add(touch); 
                
            }
            else if (msg == TouchMessage.WM_TOUCH_MOVE)
            {
                e = new TouchEventArgs(lParam, wParam);
                touchable.OnTouchMove(ctrl, e);
                handled = e.handled;
                for (int i = 0; i < mTouches.Count; i++)
                {
                    if (mTouches[i].mID == e.ID)
                    {
                        mTouches[i].mOldPosition = mTouches[i].mPosition;
                        mTouches[i].mPosition = e.point; 
                        break;
                    }
                }
                
            }
            else if (msg == TouchMessage.WM_TOUCH_UP)
            {
                e = new TouchEventArgs(lParam, wParam);
                touchable.OnTouchUp(ctrl, e);
                handled = e.handled;
                int i;
                for(i=0 ; i< mTouches.Count ; i++ )
                {
                    if (mTouches[i].mID == e.ID)
                        break; 
                }
                mTouches.RemoveAt(i); 

            }
			return 0;

		}

        /// <summary>
        /// Get a Control and return a List of the touches on that control
        /// </summary>
        /// <param name="ctrl"></param>
        /// <returns></returns>
        static public List<Touch> GetTouchesOnControl(Control ctrl)
        {
            return ((WndProcHooker.HookedProcInformation)WndProcHooker.GetInfoForControl(ctrl)).mTouches; 
        }
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace touch_simulator
{

	public enum TWMessagesType :uint
    {
		WM_NONE = 0,
		WM_TOUCH_FIRST,
        WM_TOUCHUP = 0x8000 ,
        WM_TOUCHDOWN , 
        WM_TOUCHMOVE ,
		WM_TOUCH_LAST,
    }
	public class Messages
	{
		
		public static void SendToNextWindow(Form sender,Control pointContainer ,Blob blob)
		{
			if (blob.type >= TWMessagesType.WM_TOUCH_FIRST && blob.type <= TWMessagesType.WM_TOUCH_LAST)
			{
				//find the window to send to .. mostly obsecured by the simulator
				//here i can hide the simulator .. query for window at point .. send .. then unhide the simulator
				IntPtr HWnd = GetWindow(sender.Handle, (uint)GetWindow_Cmd.GW_HWNDNEXT);

				//map the points
				POINT p = new POINT(blob.center.X, blob.center.Y);
				MapWindowPoints(sender.Handle, HWnd, ref p, 1);

				uint Msg = (uint)blob.type;
				uint LParam = (uint)((p.Y << 16) | (p.X & 0x0000FFFF));
				uint WParam = (uint)((blob.Pressure << 16) | (blob.id & 0x0000FFFF));
				PostMessage(HWnd, Msg, WParam, LParam);
			}
		}

		public static void SendToChildWindows(Control sender, Blob blob)
		{
			if (blob.type >= TWMessagesType.WM_TOUCH_FIRST && blob.type <= TWMessagesType.WM_TOUCH_LAST)
			{
				//find the window to send to .. mostly obsecured by the simulator
				//here i can hide the simulator 
				//--done by the caller
				//.. query for window at point .. send .. 
				POINT ScreenPos = new POINT(blob.center.X, blob.center.Y);
				ClientToScreen(sender.Handle ,ref ScreenPos);
				IntPtr HWnd = WindowFromPoint(ScreenPos);
				StringBuilder strBuilder = new StringBuilder();
				GetWindowText(HWnd, strBuilder, 100);
				Form1.s_txtMonitor.AppendText(HWnd + " - "+ strBuilder.ToString() +"\r\n");
				//then unhide the simulator
				//--done by the caller
				//map the points
				POINT p = new POINT(blob.center.X, blob.center.Y);
				MapWindowPoints(sender.Handle, HWnd, ref p, 1);

				uint Msg = (uint)blob.type;
				uint LParam = (uint)((p.Y << 16) | (p.X & 0x0000FFFF));
				uint WParam = (uint)((blob.Pressure << 16) | (blob.id & 0x0000FFFF));
				PostMessage(HWnd, Msg, WParam, LParam);
			}
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		
		[DllImport("user32.dll", SetLastError = true)]
		public extern static int SendMessage(
			IntPtr hwnd, uint msg, uint wParam, uint lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam,
		  uint lParam);


		[DllImport("user32.dll", SetLastError = true)]
		public extern static IntPtr GetNextWindow(
			IntPtr hwnd, uint wCmd);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll")]
		static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		enum GetWindow_Cmd : uint
		{
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			public static implicit operator System.Drawing.Point(POINT p)
			{
				return new System.Drawing.Point(p.X, p.Y);
			}

			public static implicit operator POINT(System.Drawing.Point p)
			{
				return new POINT(p.X, p.Y);
			}
		}


	}
}

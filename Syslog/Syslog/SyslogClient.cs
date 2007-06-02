/*
Syslog Client
Copyright (C)2007 Adrian O' Neill

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aonaware.Syslog
{
	/// <summary>
	/// Sends syslog messages to a Syslog server
	/// </summary>
	public class SyslogClient : IDisposable
	{
		public SyslogClient(IPAddress serverAddress)
		{
			_sendPoint = new IPEndPoint(serverAddress, DefaultPort);
		}

		public SyslogClient(IPAddress serverAddress, int port)
		{
			_sendPoint = new IPEndPoint(serverAddress, port);
		}

		public void Connect()
		{
			if (_disposed)
				throw new ObjectDisposedException("SyslogMessage");

			if (_udpClient != null)
				return;

			_udpClient = new UdpClient();
			_udpClient.Connect(_sendPoint);
		}

		public void Close()
		{
			if (_udpClient == null)
				return;

			_udpClient.Close();
			_udpClient = null;
		}

		public bool Connected
		{
			get
			{
				return (_udpClient != null);
			}
		}

		public void Send(SyslogMessage msg)
		{
			if (_disposed)
				throw new ObjectDisposedException("SyslogMessage");
			
			if (_udpClient == null)
				throw new Exception("Cannot send data, connection not established");

			if (msg == null)
				throw new ArgumentNullException("msg", "SyslogMessage paramter null");

			byte[] data = _encoding.GetBytes(msg.ToString());
			_udpClient.Send(data, data.Length);
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!_disposed)
			{
				if(disposing)
				{
					Close();
				}
				_disposed = true;
			}
		}

		~SyslogClient()      
		{
			Dispose(false);
		}

		private bool _disposed = false;
		private readonly IPEndPoint _sendPoint;
		private UdpClient _udpClient = null;
		private static ASCIIEncoding _encoding = new ASCIIEncoding();

		public const int DefaultPort = SyslogServer.DefaultPort;

	}
}

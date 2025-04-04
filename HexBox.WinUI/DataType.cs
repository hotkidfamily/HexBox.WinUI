﻿namespace HexBox.WinUI
{
    /// <summary>
    /// Enumerates how the data (bytes read from the buffer) is to be interpreted when displayed.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Display the data as integral (integer) values.
        /// </summary>
        Int_1 = 1,
        /// <summary>
        /// Display the data as integral (integer) values.
        /// </summary>
        Int_2 = 2,
        /// <summary>
        /// Display the data as integral (integer) values.
        /// </summary>
        Int_4 = 4,
        /// <summary>
        /// Display the data as integral (integer) values.
        /// </summary>
        Int_8 = 8,
        /// <summary>
        /// Display the data as floating point values.
        /// </summary>
        Float_32 = 32,
        /// <summary>
        /// Display the data as floating point values.
        /// </summary>
        Float_64 = 64,
    }
}

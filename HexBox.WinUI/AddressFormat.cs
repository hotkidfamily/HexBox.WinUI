﻿namespace HexBox.WinUI
{
    /// <summary>
    /// Enumerates the address column formatting options.
    /// </summary>
    public enum AddressFormat
    {
        /// <summary>
        /// 16 bit HEX address "0000".
        /// </summary>
        Address16,

        /// <summary>
        /// 24 bit HEX address "00:0000".
        /// </summary>
        Address24,

        /// <summary>
        /// 32 bit HEX address "0000:0000".
        /// </summary>
        Address32,

        /// <summary>
        /// 48 bit HEX address "0000:00000000".
        /// </summary>
        Address48,

        /// <summary>
        /// 64 bit HEX address "00000000:00000000".
        /// </summary>
        Address64,
    }
}

﻿namespace OggVorbisEncoder.Setup.Templates.FloorBooks;

public class Line128X11_1Sub1 : IStaticCodeBook
{
    public int Dimensions { get; } = 1;

    public byte[] LengthList { get; } = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        5, 3, 5, 3, 6, 4, 6, 4, 7, 4, 7, 4, 7, 4, 8, 4,
        8, 4, 9, 5, 9, 5, 9, 5, 9, 6, 10, 6, 10, 6, 11, 7,
        10, 7, 10, 8, 11, 9, 11, 9, 11, 10, 11, 11, 12, 11, 11, 12,
        15, 15, 12, 14, 11, 14, 12, 14, 11, 14, 13, 14, 12, 14, 11, 14,
        11, 14, 12, 14, 11, 14, 11, 14, 13, 13, 14, 14, 14, 14, 14, 14,
        14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14
    };

    public CodeBookMapType MapType { get; } = CodeBookMapType.None;
    public int QuantMin { get; } = 0;
    public int QuantDelta { get; } = 0;
    public int Quant { get; } = 0;
    public int QuantSequenceP { get; } = 0;
    public int[] QuantList { get; } = null;
}

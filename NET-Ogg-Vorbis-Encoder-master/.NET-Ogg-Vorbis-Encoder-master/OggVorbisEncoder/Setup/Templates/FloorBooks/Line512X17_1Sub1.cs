﻿namespace OggVorbisEncoder.Setup.Templates.FloorBooks;

public class Line512X17_1Sub1 : IStaticCodeBook
{
    public int Dimensions { get; } = 1;

    public byte[] LengthList { get; } = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        4, 3, 5, 3, 5, 4, 5, 4, 5, 4, 5, 5, 5, 5, 6, 5,
        6, 5, 7, 5, 8, 6, 8, 6, 8, 6, 8, 6, 8, 7, 9, 7,
        9, 7, 11, 9, 11, 11, 12, 11, 14, 12, 14, 16, 14, 16, 13, 16,
        14, 16, 12, 15, 13, 16, 14, 16, 13, 14, 12, 15, 13, 15, 13, 13,
        13, 15, 12, 14, 14, 15, 13, 15, 12, 15, 15, 15, 15, 15, 15, 15,
        15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15
    };

    public CodeBookMapType MapType { get; } = CodeBookMapType.None;
    public int QuantMin { get; } = 0;
    public int QuantDelta { get; } = 0;
    public int Quant { get; } = 0;
    public int QuantSequenceP { get; } = 0;
    public int[] QuantList { get; } = null;
}

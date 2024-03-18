﻿namespace OggVorbisEncoder.Setup.Templates.FloorBooks;

public class Line2048X27Class3 : IStaticCodeBook
{
    public int Dimensions { get; } = 1;

    public byte[] LengthList { get; } = {
        3, 3, 6, 16, 5, 5, 7, 16, 9, 8, 11, 16, 16, 16, 16, 16,
        5, 5, 8, 16, 5, 5, 7, 16, 8, 7, 9, 16, 16, 16, 16, 16,
        9, 9, 12, 16, 6, 8, 11, 16, 9, 10, 11, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 13, 16, 16, 16, 15, 16, 16, 16, 16, 16, 16, 16,
        5, 4, 7, 16, 6, 5, 8, 16, 9, 8, 10, 16, 16, 16, 16, 16,
        5, 5, 7, 15, 5, 4, 6, 15, 7, 6, 8, 16, 16, 16, 16, 16,
        9, 9, 11, 15, 7, 7, 9, 16, 8, 8, 9, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 15, 15, 15, 16, 15, 15, 14, 16, 16, 16, 16, 16,
        8, 8, 11, 16, 8, 9, 10, 16, 11, 10, 14, 16, 16, 16, 16, 16,
        6, 8, 10, 16, 6, 7, 10, 16, 8, 8, 11, 16, 14, 16, 16, 16,
        10, 11, 14, 16, 9, 9, 11, 16, 10, 10, 11, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 15, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
        12, 16, 15, 16, 12, 14, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16,
        16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16
    };

    public CodeBookMapType MapType { get; } = CodeBookMapType.None;
    public int QuantMin { get; } = 0;
    public int QuantDelta { get; } = 0;
    public int Quant { get; } = 0;
    public int QuantSequenceP { get; } = 0;
    public int[] QuantList { get; } = null;
}

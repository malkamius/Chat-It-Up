namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Coupled.Chapter1;

public class Page9_2 : IStaticCodeBook
{
    public int Dimensions { get; } = 2;

    public byte[] LengthList { get; } = {
         1, 4, 4, 6, 6, 7, 7, 8, 7, 8, 8, 9, 9, 9, 9,10,
        10,10, 9,10,10,11,12,12, 8, 8, 8, 8, 9, 9, 9, 9,
        10,10,10,10,10,11,11,10,12,11,11,13,11, 7, 7, 8,
         8, 8, 8, 9, 9, 9,10,10,10,10, 9,10,10,11,11,12,
        11,11, 8, 8, 8, 8, 9, 9,10,10,10,10,11,11,11,11,
        11,11,11,12,11,12,12, 8, 8, 9, 9, 9, 9, 9,10,10,
        10,10,10,10,11,11,11,11,11,11,12,11, 9, 9, 9, 9,
        10,10,10,10,11,10,11,11,11,11,11,11,12,12,12,12,
        11, 9, 9, 9, 9,10,10,10,10,11,11,11,11,11,11,11,
        11,11,12,12,12,13, 9,10,10, 9,11,10,10,10,10,11,
        11,11,11,11,10,11,12,11,12,12,11,12,11,10, 9,10,
        10,11,10,11,11,11,11,11,11,11,11,11,12,12,11,12,
        12,12,10,10,10,11,10,11,11,11,11,11,11,11,11,11,
        11,11,12,13,12,12,11, 9,10,10,11,11,10,11,11,11,
        12,11,11,11,11,11,12,12,13,13,12,13,10,10,12,10,
        11,11,11,11,11,11,11,11,11,12,12,11,13,12,12,12,
        12,13,12,11,11,11,11,11,11,12,11,12,11,11,11,11,
        12,12,13,12,11,12,12,11,11,11,11,11,12,11,11,11,
        11,12,11,11,12,11,12,13,13,12,12,12,12,11,11,11,
        11,11,12,11,11,12,11,12,11,11,11,11,13,12,12,12,
        12,13,11,11,11,12,12,11,11,11,12,11,12,12,12,11,
        12,13,12,11,11,12,12,11,12,11,11,11,12,12,11,12,
        11,11,11,12,12,12,12,13,12,13,12,12,12,12,11,11,
        12,11,11,11,11,11,11,12,12,12,13,12,11,13,13,12,
        12,11,12,10,11,11,11,11,12,11,12,12,11,12,12,13,
        12,12,13,12,12,12,12,12,11,12,12,12,11,12,11,11,
        11,12,13,12,13,13,13,13,13,12,13,13,12,12,13,11,
        11,11,11,11,12,11,11,12,11,
    };

    public CodeBookMapType MapType { get; } = (CodeBookMapType)1;
    public int QuantMin { get; } = -529268736;
    public int QuantDelta { get; } = 1611661312;
    public int Quant { get; } = 5;
    public int QuantSequenceP { get; } = 0;

    public int[] QuantList { get; } = {
        10,
        9,
        11,
        8,
        12,
        7,
        13,
        6,
        14,
        5,
        15,
        4,
        16,
        3,
        17,
        2,
        18,
        1,
        19,
        0,
        20,
    };
}

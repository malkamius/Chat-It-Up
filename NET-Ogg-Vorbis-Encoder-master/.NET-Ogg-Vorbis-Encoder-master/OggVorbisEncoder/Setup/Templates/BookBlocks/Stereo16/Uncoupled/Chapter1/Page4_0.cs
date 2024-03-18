namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Uncoupled.Chapter1;

public class Page4_0 : IStaticCodeBook
{
    public int Dimensions { get; } = 4;

    public byte[] LengthList { get; } = {
         4, 5, 5, 8, 8, 6, 6, 7, 9, 9, 6, 6, 6, 9, 9, 9,
        10, 9,11,11, 9, 9,10,11,11, 6, 7, 7,10, 9, 7, 7,
         8, 9,10, 7, 7, 8,10,10,10,10,10,10,12, 9, 9,10,
        11,12, 6, 7, 7, 9, 9, 7, 8, 7,10,10, 7, 8, 7,10,
        10, 9,10, 9,12,11,10,10, 9,12,10, 9,10,10,12,11,
        10,10,10,12,12, 9,10,10,12,12,12,11,12,13,13,11,
        11,12,12,13, 9,10,10,11,12, 9,10,10,12,12,10,10,
        10,12,12,11,12,11,14,13,11,12,12,14,13, 5, 7, 7,
        10,10, 7, 8, 8,10,10, 7, 8, 7,10,10,10,10,10,12,
        12,10,10,10,12,12, 6, 8, 7,10,10, 7, 7, 9,10,11,
         8, 9, 9,11,10,10,10,11,11,13,10,10,11,12,13, 6,
         8, 8,10,10, 7, 9, 8,11,10, 8, 9, 9,10,11,10,11,
        10,13,11,10,11,10,12,12,10,11,10,12,11,10,10,10,
        12,13,10,11,11,13,12,11,11,13,11,14,12,12,13,14,
        14, 9,10,10,12,13,10,11,10,13,12,10,11,11,12,13,
        11,12,11,14,12,12,13,13,15,14, 5, 7, 7,10,10, 7,
         7, 8,10,10, 7, 8, 8,10,10,10,10,10,11,12,10,10,
        10,12,12, 7, 8, 8,10,10, 8, 9, 8,11,10, 7, 8, 9,
        10,11,10,11,11,12,12,10,10,11,11,13, 7, 7, 8,10,
        10, 8, 8, 9,10,11, 7, 9, 7,11,10,10,11,11,13,12,
        11,11,10,13,11, 9,10,10,12,12,10,11,11,13,12,10,
        10,11,12,12,12,13,13,14,14,11,11,12,12,14,10,10,
        11,12,12,10,11,11,12,13,10,10,10,13,12,12,13,13,
        15,14,12,13,10,14,11, 8,10,10,12,12,10,11,10,13,
        13, 9,10,10,12,12,12,13,13,15,14,11,12,12,13,13,
         9,10,10,13,12,10,10,11,13,13,10,11,10,13,12,12,
        12,13,14,15,12,13,12,15,13, 9,10,10,12,13,10,11,
        10,13,12,10,10,11,12,13,12,14,12,15,13,12,12,13,
        14,15,11,12,11,14,13,11,11,12,14,15,12,13,12,15,
        14,13,11,15,11,16,13,14,14,16,15,11,12,12,14,14,
        11,12,11,14,13,12,12,13,14,15,13,14,12,16,12,14,
        14,14,15,15, 8,10,10,12,12, 9,10,10,12,12,10,10,
        11,13,13,11,12,12,13,13,12,13,13,14,15, 9,10,10,
        13,12,10,11,11,13,12,10,10,11,13,13,12,13,12,15,
        14,12,12,13,13,16, 9, 9,10,12,13,10,10,11,12,13,
        10,11,10,13,13,12,12,13,13,15,13,13,12,15,13,11,
        12,12,14,14,12,13,12,15,14,11,11,12,13,14,14,14,
        14,16,15,13,12,15,12,16,11,11,12,13,14,12,13,13,
        14,15,10,12,11,14,13,14,15,14,16,16,13,14,11,15,
        11,
    };

    public CodeBookMapType MapType { get; } = (CodeBookMapType)1;
    public int QuantMin { get; } = -533725184;
    public int QuantDelta { get; } = 1611661312;
    public int Quant { get; } = 3;
    public int QuantSequenceP { get; } = 0;

    public int[] QuantList { get; } = {
        2,
        1,
        3,
        0,
        4,
    };
}

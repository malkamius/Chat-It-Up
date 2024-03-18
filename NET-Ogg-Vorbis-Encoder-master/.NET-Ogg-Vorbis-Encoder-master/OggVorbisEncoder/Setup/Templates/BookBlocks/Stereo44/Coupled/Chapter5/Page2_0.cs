namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5;

public class Page2_0 : IStaticCodeBook
{
    public int Dimensions { get; } = 4;

    public byte[] LengthList { get; } = {
         2, 4, 4, 0, 0, 0, 5, 5, 0, 0, 0, 5, 5, 0, 0, 0,
         8, 7, 0, 0, 0, 0, 0, 0, 0, 4, 6, 6, 0, 0, 0, 8,
         8, 0, 0, 0, 8, 7, 0, 0, 0,10,10, 0, 0, 0, 0, 0,
         0, 0, 4, 6, 6, 0, 0, 0, 8, 8, 0, 0, 0, 7, 8, 0,
         0, 0,10,10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 5, 8, 7, 0, 0, 0, 8, 8, 0, 0,
         0, 8, 8, 0, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0, 5,
         7, 8, 0, 0, 0, 8, 8, 0, 0, 0, 8, 8, 0, 0, 0,10,
        10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 5, 8, 8, 0, 0, 0, 8, 8, 0, 0, 0, 8, 8,
         0, 0, 0,10,10, 0, 0, 0, 0, 0, 0, 0, 5, 8, 8, 0,
         0, 0, 8, 8, 0, 0, 0, 8, 8, 0, 0, 0,10,10, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         8,10,10, 0, 0, 0,10,10, 0, 0, 0, 9,10, 0, 0, 0,
        11,10, 0, 0, 0, 0, 0, 0, 0, 8,10,10, 0, 0, 0,10,
        10, 0, 0, 0,10,10, 0, 0, 0,10,11, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         0,
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

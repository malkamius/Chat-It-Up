namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4;

public class Chapter4Short : IStaticCodeBook
{
    public int Dimensions { get; } = 2;

    public byte[] LengthList { get; } = {
         4, 7,14,10,15,10,12,15,16,15, 4, 2,11, 5,10, 6,
         8,11,14,14,14,10, 7,11, 6, 8,10,11,13,15, 9, 4,
        11, 5, 9, 6, 9,12,14,15,14, 9, 6, 9, 4, 5, 7,10,
        12,13, 9, 5, 7, 6, 5, 5, 7,10,13,13,10, 8, 9, 8,
         7, 6, 8,10,14,14,13,11,10,10, 7, 7, 8,11,14,15,
        13,12, 9, 9, 6, 5, 7,10,14,17,15,13,11,10, 6, 6,
         7, 9,12,17,
    };

    public CodeBookMapType MapType { get; } = (CodeBookMapType)0;
    public int QuantMin { get; } = 0;
    public int QuantDelta { get; } = 0;
    public int Quant { get; } = 0;
    public int QuantSequenceP { get; } = 0;

    public int[] QuantList { get; } = null;
}

namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Coupled.Chapter1;

public class Page7_0 : IStaticCodeBook
{
    public int Dimensions { get; } = 4;

    public byte[] LengthList { get; } = {
         1, 4, 4, 6, 6, 6, 7, 6, 6, 4, 7, 7,10, 9,10,10,
        10, 9, 4, 7, 7,10,10,10,11,10,10, 6,10,10,11,11,
        11,11,10,10, 6,10, 9,11,11,11,11,10,10, 6,10,10,
        11,11,11,11,10,10, 7,11,11,11,11,11,12,12,11, 6,
        10,10,11,10,10,11,11,11, 6,10,10,10,11,10,11,11,
        11,
    };

    public CodeBookMapType MapType { get; } = (CodeBookMapType)1;
    public int QuantMin { get; } = -529137664;
    public int QuantDelta { get; } = 1618345984;
    public int Quant { get; } = 2;
    public int QuantSequenceP { get; } = 0;

    public int[] QuantList { get; } = {
        1,
        0,
        2,
    };
}

//using OggVorbisEncoder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatItUp.webm
{
    public static class ParseAudio
    {
        static readonly int WriteBufferSize = 512;
        public static byte[] ParseWebMChunk(byte[] data) => new byte[] {};

    //    public static byte[] ParseWebMChunk(byte[] data)
    //    {
    //        using (var outstream = new MemoryStream())
    //        using (var stream = new MemoryStream(data))
    //        {
    //            long pos = 0;
    //            while (stream.Position < stream.Length)
    //            {

    //                long length = 0;
    //                var size = ReadUInt(stream, pos, ref length);

    //                pos += length;
    //                var tracknumber = ReadUInt(stream, pos, ref length);
    //                pos += length;
    //                pos += 2;
    //                stream.Position = pos;

    //                var marker = 0;

    //                if (stream.Position < stream.Length && data[stream.Position] == 128)
    //                    marker = stream.ReadByte();
    //                if (size <= 4 || stream.Position + size > stream.Length)
    //                {
    //                    pos = stream.Position + 1;
    //                    continue;
    //                }
    //                var buffer = new byte[size - 4];
    //                var read = stream.Read(buffer, 0, buffer.Length);


    //                pos = stream.Position + 1;


    //                var totalbuffer = buffer;
    //                using (var decoder = FragLabs.Audio.Codecs.OpusDecoder.Create(48000, 1))
    //                {
    //                    decoder.MaxDataBytes = 48000;

    //                    var newbuffer = new byte[0];
    //                    try
    //                    {
    //                        if (tracknumber == 1)
    //                        {
    //                            var decoded = decoder.Decode(totalbuffer, totalbuffer.Length, out var decodedlength);
    //                            outstream.Write(decoded, 0, decodedlength);
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        Console.WriteLine(ex.ToString());
    //                    }
    //                }
    //            }

    //            var oggBytes = ConvertRawPCMFile(48000, 1, outstream.ToArray(), 2, 48000, 1);
    //            return oggBytes;
    //        }
    //    }

    //    static long ReadUInt(Stream stream, long pos, ref long len)
    //    {
    //        if (stream == null || pos < 0)
    //            return -1;

    //        len = 1;
    //        stream.Position = pos;
    //        var b = stream.ReadByte();


    //        if (b == 0)  // we can't handle u-int values larger than 8 bytes
    //            return -1;

    //        byte m = 0x80;

    //        while ((b & m) == 0)
    //        {
    //            m >>= 1;
    //            ++len;
    //        }

    //        long result = b & (~m);
    //        ++pos;

    //        for (int i = 1; i < len; ++i)
    //        {
    //            b = stream.ReadByte();

    //            result <<= 8;
    //            result |= b;

    //            ++pos;
    //        }
    //        return result;
    //    }

    //    static byte[] ConvertRawPCMFile(int outputSampleRate, int outputChannels, byte[] pcmSamples, short pcmSampleSize, int pcmSampleRate, int pcmChannels)
    //    {
    //        int numPcmSamples = (pcmSamples.Length / (int)pcmSampleSize / pcmChannels);
    //        float pcmDuraton = numPcmSamples / (float)pcmSampleRate;

    //        int numOutputSamples = (int)(pcmDuraton * outputSampleRate);
    //        //Ensure that samble buffer is aligned to write chunk size
    //        numOutputSamples = (numOutputSamples / WriteBufferSize) * WriteBufferSize;

    //        float[][] outSamples = new float[outputChannels][];

    //        for (int ch = 0; ch < outputChannels; ch++)
    //        {
    //            outSamples[ch] = new float[numOutputSamples];
    //        }

    //        for (int sampleNumber = 0; sampleNumber < numOutputSamples; sampleNumber++)
    //        {
    //            float rawSample = 0.0f;

    //            for (int ch = 0; ch < outputChannels; ch++)
    //            {
    //                int sampleIndex = (sampleNumber * pcmChannels) * (int)pcmSampleSize;

    //                if (ch < pcmChannels) sampleIndex += (ch * (int)pcmSampleSize);

    //                switch (pcmSampleSize)
    //                {
    //                    case 1:
    //                        rawSample = ByteToSample(pcmSamples[sampleIndex]);
    //                        break;
    //                    case 2:
    //                        rawSample = ShortToSample((short)(pcmSamples[sampleIndex + 1] << 8 | pcmSamples[sampleIndex]));
    //                        break;
    //                    case 4: // Handling 32-bit samples
    //                        int intSample = (int)(
    //                            (pcmSamples[sampleIndex + 3] << 24) |
    //                            (pcmSamples[sampleIndex + 2] << 16) |
    //                            (pcmSamples[sampleIndex + 1] << 8) |
    //                            pcmSamples[sampleIndex]
    //                        );
    //                        rawSample = IntToSample(intSample);
    //                        break;
    //                }

    //                outSamples[ch][sampleNumber] = rawSample;
    //            }
    //        }

    //        return GenerateFile(outSamples, outputSampleRate, outputChannels);
    //    }
    //    static float IntToSample(int intSample)
    //    {
    //        return intSample / (float)Int32.MaxValue;
    //    }

    //    static byte[] GenerateFile(float[][] floatSamples, int sampleRate, int channels)
    //    {
    //        using MemoryStream outputData = new MemoryStream();

    //        // Stores all the static vorbis bitstream settings
    //        var info = VorbisInfo.InitVariableBitRate(channels, sampleRate, 1f);

    //        // set up our packet->stream encoder
    //        var serial = new Random().Next();
    //        var oggStream = new OggStream(serial);

    //        // =========================================================
    //        // HEADER
    //        // =========================================================
    //        // Vorbis streams begin with three headers; the initial header (with
    //        // most of the codec setup parameters) which is mandated by the Ogg
    //        // bitstream spec.  The second header holds any comment fields.  The
    //        // third header holds the bitstream codebook.

    //        var comments = new Comments();
    //        comments.AddTag("ARTIST", "TEST");

    //        var infoPacket = HeaderPacketBuilder.BuildInfoPacket(info);
    //        var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);
    //        var booksPacket = HeaderPacketBuilder.BuildBooksPacket(info);

    //        oggStream.PacketIn(infoPacket);
    //        oggStream.PacketIn(commentsPacket);
    //        oggStream.PacketIn(booksPacket);

    //        // Flush to force audio data onto its own page per the spec
    //        FlushPages(oggStream, outputData, true);

    //        // =========================================================
    //        // BODY (Audio Data)
    //        // =========================================================
    //        var processingState = ProcessingState.Create(info);

    //        for (int readIndex = 0; readIndex <= floatSamples[0].Length; readIndex += WriteBufferSize)
    //        {
    //            if (readIndex == floatSamples[0].Length)
    //            {
    //                processingState.WriteEndOfStream();
    //            }
    //            else
    //            {
    //                processingState.WriteData(floatSamples, WriteBufferSize, readIndex);
    //            }

    //            while (!oggStream.Finished && processingState.PacketOut(out OggPacket packet))
    //            {
    //                oggStream.PacketIn(packet);

    //                FlushPages(oggStream, outputData, false);
    //            }
    //        }

    //        FlushPages(oggStream, outputData, true);

    //        return outputData.ToArray();
    //    }

    //    static void FlushPages(OggStream oggStream, Stream output, bool force)
    //    {
    //        while (oggStream.PageOut(out OggPage page, force))
    //        {
    //            output.Write(page.Header, 0, page.Header.Length);
    //            output.Write(page.Body, 0, page.Body.Length);
    //        }
    //    }

    //    static float ByteToSample(short pcmValue)
    //    {
    //        return pcmValue / 128f;
    //    }

    //    static float ShortToSample(short pcmValue)
    //    {
    //        return pcmValue / 32768f;
    //    }
    }
}

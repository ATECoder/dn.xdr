using cc.isr.XDR.Codecs;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class CodecStreamsTests
    {

        /// <summary>   Assert codec should process string. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessString( string arg1 )
        {
            StringXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            StringXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process string. </summary>
        [TestMethod]
        public void CodecShouldProcessString()
        {
            AssertCodecShouldProcessString( "XDR serialized string" );
        }
    }
}

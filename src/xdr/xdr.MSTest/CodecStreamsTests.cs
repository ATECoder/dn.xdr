using cc.isr.XDR.Codecs;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class CodecStreamsTests
    {

        /// <summary>   Assert codec should process string. </summary>
        /// <remarks>   2022-12-31. </remarks>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessString( string arg1 )
        {
            StringXdrCodec args = new( arg1 );
            XdrBufferEncodingStream xdr = new ( 1024 );
            args.Encode( xdr );

            StringXdrCodec result = new();
            XdrDecodingStreamBase xdrDecoder = new XdrBufferDecodingStream( xdr.GetXdrData(), xdr.GetXdrLength() );
            xdrDecoder.BeginDecoding();
            result.Decode( xdrDecoder );
            xdrDecoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process string. </summary>
        /// <remarks>   2022-12-31. </remarks>
        [TestMethod]
        public void CodecShouldProcessString()
        {
            AssertCodecShouldProcessString( "XDR serialized string" );
        }
    }
}

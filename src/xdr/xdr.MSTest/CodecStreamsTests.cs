using System.Text;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class CodecStreamsTests
    {


        /// <summary>   Assert codec should process Boolean. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessBoolean( bool arg1 )
        {
            BooleanXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            BooleanXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Boolean. </summary>
        [TestMethod]
        public void CodecShouldProcessBoolean()
        {
            AssertCodecShouldProcessBoolean( true );
            AssertCodecShouldProcessBoolean( false );
        }

        /// <summary>   Assert codec should process <see langword="byte"/>. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessByte( byte arg1 )
        {
            ByteXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            ByteXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="byte"/>. </summary>
        [TestMethod]
        public void CodecShouldProcessByte()
        {
            AssertCodecShouldProcessByte( byte.MinValue );
            AssertCodecShouldProcessByte( byte.MaxValue );
            AssertCodecShouldProcessByte( ( byte ) 0 );
        }

        /// <summary>   Assert codec should process <see langword="byte"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessBytes( byte[] arg1 )
        {
            BytesXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            BytesXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( args.GetValue(), result.GetValue() ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="byte"/>s. </summary>
        [TestMethod]
        public void CodecShouldProcessBytes()
        {
            AssertCodecShouldProcessBytes( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Character. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessCharacter( char arg1 )
        {
            CharXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            CharXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Character. </summary>
        [TestMethod]
        public void CodecShouldProcessCharacter()
        {
            for ( int i = 0;i < byte.MaxValue ; i++ )
            {
                AssertCodecShouldProcessCharacter( Encoding.ASCII.GetString( new byte[] { (byte) i } )[0] );
            }
        }

        /// <summary>   Assert codec should process Double. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessDouble( double arg1 )
        {
            DoubleXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            DoubleXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Double. </summary>
        [TestMethod]
        public void CodecShouldProcessDouble()
        {
            AssertCodecShouldProcessDouble( double.MinValue );
            AssertCodecShouldProcessDouble( double.MaxValue );
            AssertCodecShouldProcessDouble( ( double ) 0 );
        }

        /// <summary>   Assert codec should process DynamicOpaque. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessDynamicOpaque( byte[] arg1 )
        {
            DynamicOpaqueXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            DynamicOpaqueXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( args.GetValue(), result.GetValue() ) );
        }

        /// <summary>   (Unit Test Method) codec should process DynamicOpaque. </summary>
        [TestMethod]
        public void CodecShouldProcessDynamicOpaque()
        {
            AssertCodecShouldProcessDynamicOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Float. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessFloat( float arg1 )
        {
            FloatXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            FloatXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Float. </summary>
        [TestMethod]
        public void CodecShouldProcessFloat()
        {
            AssertCodecShouldProcessFloat( float.MinValue );
            AssertCodecShouldProcessFloat( float.MaxValue );
            AssertCodecShouldProcessFloat( ( float ) 0 );
        }

        /// <summary>   Assert codec should process Integer. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessInteger( int arg1 )
        {
            IntXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            IntXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Integer. </summary>
        [TestMethod]
        public void CodecShouldProcessInteger()
        {
            AssertCodecShouldProcessInteger( int.MinValue );
            AssertCodecShouldProcessInteger( int.MaxValue );
            AssertCodecShouldProcessInteger( ( int ) 0 );
        }

        /// <summary>   Assert codec should process Long. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessLong( long arg1 )
        {
            LongXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            LongXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Long. </summary>
        [TestMethod]
        public void CodecShouldProcessLong()
        {
            AssertCodecShouldProcessLong( long.MinValue );
            AssertCodecShouldProcessLong( long.MaxValue );
            AssertCodecShouldProcessLong( ( long ) 0 );
        }

        /// <summary>   Assert codec should process Opaque. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessOpaque( byte[] arg1 )
        {
            OpaqueXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            OpaqueXdrCodec result = new( encoder.GetEncodedData() );
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 ); // encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            byte[] expected = args.GetValue();
            byte[] actual = new byte[expected.Length];
            Array.Copy( result.GetValue(), actual, expected.Length );

            Assert.IsTrue( Enumerable.SequenceEqual( expected, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process Opaque. </summary>
        [TestMethod]
        public void CodecShouldProcessOpaque()
        {
            AssertCodecShouldProcessOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }


        /// <summary>   Assert codec should process Short. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertCodecShouldProcessShort( short arg1 )
        {
            ShortXdrCodec args = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            args.Encode( encoder );

            ShortXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( args.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Short. </summary>
        [TestMethod]
        public void CodecShouldProcessShort()
        {
            AssertCodecShouldProcessShort( short.MinValue );
            AssertCodecShouldProcessShort( short.MaxValue );
            AssertCodecShouldProcessShort( ( short ) 0 );
        }


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

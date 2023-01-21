using System.Text;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class CodecStreamsTests
    {


        /// <summary>   Assert codec should process <see cref="bool"/>. </summary>
        /// <param name="value">    parameter of type <see cref="bool"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessBoolean( bool value )
        {
            BooleanXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            BooleanXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Boolean. </summary>
        [TestMethod]
        public void CodecShouldProcessBoolean()
        {
            AssertCodecShouldProcessBoolean( true );
            AssertCodecShouldProcessBoolean( false );
        }

        /// <summary>   Assert codec should process <see cref="byte"/>. </summary>
        /// <param name="value">    parameter of type <see cref="byte"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessByte( byte value )
        {
            ByteXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            ByteXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="byte"/>. </summary>
        [TestMethod]
        public void CodecShouldProcessByte()
        {
            AssertCodecShouldProcessByte( byte.MinValue );
            AssertCodecShouldProcessByte( byte.MaxValue );
            AssertCodecShouldProcessByte( ( byte ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="byte"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessBytes( byte[] value )
        {
            BytesXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            BytesXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( request.GetValue(), result.GetValue() ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="byte"/>s. </summary>
        [TestMethod]
        public void CodecShouldProcessBytes()
        {
            AssertCodecShouldProcessBytes( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Character. </summary>
        /// <param name="value">    parameter of type <see cref="char"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessCharacter( char value )
        {
            CharXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            CharXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Character. </summary>
        [TestMethod]
        public void CodecShouldProcessCharacter()
        {
            for ( int i = 0; i < byte.MaxValue; i++ )
            {
                AssertCodecShouldProcessCharacter( Encoding.ASCII.GetString( new byte[] { ( byte ) i } )[0] );
            }
        }

        /// <summary>   Assert codec should process Double. </summary>
        /// <param name="value">    parameter of type <see cref="double"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessDouble( double value )
        {
            DoubleXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            DoubleXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
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
        /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessDynamicOpaque( byte[] value )
        {
            DynamicOpaqueXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            DynamicOpaqueXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( request.GetValue(), result.GetValue() ) );
        }

        /// <summary>   (Unit Test Method) codec should process DynamicOpaque. </summary>
        [TestMethod]
        public void CodecShouldProcessDynamicOpaque()
        {
            AssertCodecShouldProcessDynamicOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process <see cref="float"/>. </summary>
        /// <param name="value">    parameter of type <see cref="float"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessFloat( float value )
        {
            FloatXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            FloatXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Float. </summary>
        [TestMethod]
        public void CodecShouldProcessFloat()
        {
            AssertCodecShouldProcessFloat( float.MinValue );
            AssertCodecShouldProcessFloat( float.MaxValue );
            AssertCodecShouldProcessFloat( ( float ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="int"/>. </summary>
        /// <param name="value">    parameter of type <see cref="int"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessInteger( int value )
        {
            IntXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            IntXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Integer. </summary>
        [TestMethod]
        public void CodecShouldProcessInteger()
        {
            AssertCodecShouldProcessInteger( int.MinValue );
            AssertCodecShouldProcessInteger( int.MaxValue );
            AssertCodecShouldProcessInteger( ( int ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="long"/>. </summary>
        /// <param name="value">    parameter of type <see cref="long"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessLong( long value )
        {
            LongXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            LongXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
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
        /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessOpaque( byte[] arg1 )
        {
            OpaqueXdrCodec request = new( arg1 );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            OpaqueXdrCodec result = new( encoder.GetEncodedData() );
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 ); // encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            byte[] expected = request.GetValue();
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


        /// <summary>   Assert codec should process <see cref="short"/>. </summary>
        /// <param name="value">    parameter of type <see cref="short"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessShort( short value )
        {
            ShortXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            ShortXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process Short. </summary>
        [TestMethod]
        public void CodecShouldProcessShort()
        {
            AssertCodecShouldProcessShort( short.MinValue );
            AssertCodecShouldProcessShort( short.MaxValue );
            AssertCodecShouldProcessShort( ( short ) 0 );
        }


        /// <summary>   Assert codec should process <see cref="string"/>. </summary>
        /// <param name="value">    parameter of type <see cref="string"/> to encode and decode. </param>
        private static void AssertCodecShouldProcessString( string value )
        {
            StringXdrCodec request = new( value );
            XdrBufferEncodingStream encoder = new( 1024 );
            request.Encode( encoder );

            StringXdrCodec result = new();
            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
            decoder.BeginDecoding();
            result.Decode( decoder );
            decoder.EndDecoding();

            Assert.AreEqual( request.Value, result.Value );
        }

        /// <summary>   (Unit Test Method) codec should process string. </summary>
        [TestMethod]
        public void CodecShouldProcessString()
        {
            AssertCodecShouldProcessString( "XDR serialized string" );
        }
    }
}

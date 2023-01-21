using System.Text;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class StreamsTests
    {


        /// <summary>   Assert streams should process. </summary>
        /// <param name="value">    parameter of type <see cref="bool"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( bool value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            bool actual = decoder.DecodeBoolean();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Boolean. </summary>
        [TestMethod]
        public void StreamsShouldProcessBoolean()
        {
            AssertStreamsShouldProcess( true );
            AssertStreamsShouldProcess( false );
        }

        /// <summary>   Assert codec should process <see cref="byte"/>. </summary>
        /// <param name="value">    parameter of type <see cref="byte"/> to encode and decode.
        private static void AssertStreamsShouldProcess( byte value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte actual = decoder.DecodeByte();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="byte"/>. </summary>
        [TestMethod]
        public void StreamsShouldProcessByte()
        {
            AssertStreamsShouldProcess( byte.MinValue );
            AssertStreamsShouldProcess( byte.MaxValue );
            AssertStreamsShouldProcess( ( byte ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="byte"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessBytes( byte[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte[] actual = decoder.DecodeByteVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="byte"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessBytes()
        {
            AssertStreamsShouldProcessBytes( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Character. </summary>
        /// <param name="value">    parameter of type <see cref="char"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessCharacter( char value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            char actual = decoder.DecodeChar();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Character. </summary>
        [TestMethod]
        public void StreamsShouldProcessCharacter()
        {
            for ( int i = 0; i < byte.MaxValue; i++ )
            {
                AssertStreamsShouldProcessCharacter( Encoding.ASCII.GetString( new byte[] { ( byte ) i } )[0] );
            }
        }

        private static void AssertStreamsShouldProcess( char[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.EncodeDynamicOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            char[] actual = decoder.DecodeDynamicOpaqueChar();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="char"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessChars()
        {
            AssertStreamsShouldProcess( new char[] { ( char ) byte.MinValue, ( char ) 0, ( char ) byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Double. </summary>
        /// <param name="value">    parameter of type <see cref="double"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessDouble( double value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            double actual = decoder.DecodeDouble();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Double. </summary>
        [TestMethod]
        public void StreamsShouldProcessDouble()
        {
            AssertStreamsShouldProcessDouble( double.MinValue );
            AssertStreamsShouldProcessDouble( double.MaxValue );
            AssertStreamsShouldProcessDouble( ( double ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="double"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="double[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( double[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            double[] actual = decoder.DecodeDoubleVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="double"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessDoubles()
        {
            AssertStreamsShouldProcess( new double[] { double.MinValue, 0, double.MaxValue } );
        }

        /// <summary>   Assert codec should process DynamicOpaque. </summary>
        /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessDynamicOpaque( byte[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.EncodeDynamicOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte[] actual = decoder.DecodeDynamicOpaque();
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process DynamicOpaque. </summary>
        [TestMethod]
        public void StreamsShouldProcessDynamicOpaque()
        {
            AssertStreamsShouldProcessDynamicOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Float. </summary>
        /// <param name="value">    parameter of type <see cref="float"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( float value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            float actual = decoder.DecodeFloat();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Float. </summary>
        [TestMethod]
        public void StreamsShouldProcessFloat()
        {
            AssertStreamsShouldProcess( float.MinValue );
            AssertStreamsShouldProcess( float.MaxValue );
            AssertStreamsShouldProcess( ( float ) 0 );
        }


        /// <summary>   Assert codec should process <see cref="float"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="float[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( float[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            float[] actual = decoder.DecodeFloatVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="float"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessFloats()
        {
            AssertStreamsShouldProcess( new float[] { float.MinValue, 0, float.MaxValue } );
        }


        /// <summary>   Assert codec should process Integer. </summary>
        /// <param name="value">    parameter of type <see cref="int"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessInteger( int value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            int actual = decoder.DecodeInt();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Integer. </summary>
        [TestMethod]
        public void StreamsShouldProcessInteger()
        {
            AssertStreamsShouldProcessInteger( int.MinValue );
            AssertStreamsShouldProcessInteger( int.MaxValue );
            AssertStreamsShouldProcessInteger( ( int ) 0 );
        }

        /// <summary>   Assert codec should process Long. </summary>
        /// <param name="value">    parameter of type <see cref="long"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessLong( long value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            long actual = decoder.DecodeLong();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   Assert codec should process <see cref="int"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="int[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( int[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            int[] actual = decoder.DecodeIntVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="int"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessInts()
        {
            AssertStreamsShouldProcess( new int[] { int.MinValue, 0, int.MaxValue } );
        }


        /// <summary>   (Unit Test Method) codec should process Long. </summary>
        [TestMethod]
        public void StreamsShouldProcessLong()
        {
            AssertStreamsShouldProcessLong( long.MinValue );
            AssertStreamsShouldProcessLong( long.MaxValue );
            AssertStreamsShouldProcessLong( ( long ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="long"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="long[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( long[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            long[] actual = decoder.DecodeLongVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="long"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessLongs()
        {
            AssertStreamsShouldProcess( new long[] { long.MinValue, 0, long.MaxValue } );
        }


        /// <summary>   Assert codec should process Opaque. </summary>
        /// <param name="value">    parameter of type <see cref="[]byte"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcessOpaque( byte[] value )
        {

            XdrBufferEncodingStream encoder = new( 1024 );
            value.EncodeOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte[] actual = decoder.DecodeOpaque( value.Length );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process Opaque. </summary>
        [TestMethod]
        public void StreamsShouldProcessOpaque()
        {
            AssertStreamsShouldProcessOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }


        /// <summary>   Assert codec should process Short. </summary>
        /// <param name="value">    parameter of type <see cref="short"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( short value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            short actual = decoder.DecodeShort();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Short. </summary>
        [TestMethod]
        public void StreamsShouldProcessShort()
        {
            AssertStreamsShouldProcess( short.MinValue );
            AssertStreamsShouldProcess( short.MaxValue );
            AssertStreamsShouldProcess( ( short ) 0 );
        }

        /// <summary>   Assert codec should process <see cref="short"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="short[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( short[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            short[] actual = decoder.DecodeShortVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="short"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessShorts()
        {
            AssertStreamsShouldProcess( new short[] { short.MinValue, 0, short.MaxValue } );
        }


        /// <summary>   Assert codec should process string. </summary>
        /// <param name="value">    parameter of type <see cref="string"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( string value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            string actual = decoder.DecodeString();
            decoder.EndDecoding();
            Assert.AreEqual( value, actual );
        }

        /// <summary>   (Unit Test Method) codec should process string. </summary>
        [TestMethod]
        public void StreamsShouldProcessString()
        {
            AssertStreamsShouldProcess( "XDR serialized string" );
        }

        /// <summary>   Assert codec should process <see cref="string"/>s. </summary>
        /// <param name="value">    parameter of type <see cref="string[]"/> to encode and decode. </param>
        private static void AssertStreamsShouldProcess( string[] value )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            value.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            string[] actual = decoder.DecodeStringVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( value, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see cref="string"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessStrings()
        {
            AssertStreamsShouldProcess( new string[] { "XDR", "serialized", "string" } );
        }


    }
}

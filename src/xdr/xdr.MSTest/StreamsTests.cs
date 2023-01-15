using System.Text;

namespace cc.isr.XDR.MSTest
{
    [TestClass]
    public class StreamsTests
    {


        /// <summary>   Assert streams should process. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( bool arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            bool actual = decoder.DecodeBoolean();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Boolean. </summary>
        [TestMethod]
        public void StreamsShouldProcessBoolean()
        {
            AssertStreamsShouldProcess( true );
            AssertStreamsShouldProcess( false );
        }

        /// <summary>   Assert codec should process <see langword="byte"/>. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( byte arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte actual = decoder.DecodeByte();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="byte"/>. </summary>
        [TestMethod]
        public void StreamsShouldProcessByte()
        {
            AssertStreamsShouldProcess( byte.MinValue );
            AssertStreamsShouldProcess( byte.MaxValue );
            AssertStreamsShouldProcess( ( byte ) 0 );
        }

        /// <summary>   Assert codec should process <see langword="byte"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessBytes( byte[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte[] actual = decoder.DecodeByteVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="byte"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessBytes()
        {
            AssertStreamsShouldProcessBytes( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Character. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessCharacter( char arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            char actual = decoder.DecodeChar();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Character. </summary>
        [TestMethod]
        public void StreamsShouldProcessCharacter()
        {
            for ( int i = 0;i < byte.MaxValue ; i++ )
            {
                AssertStreamsShouldProcessCharacter( Encoding.ASCII.GetString( new byte[] { (byte) i } )[0] );
            }
        }

        private static void AssertStreamsShouldProcess( char[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.EncodeDynamicOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            char[] actual = decoder.DecodeDynamicOpaqueChar();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="char"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessChars()
        {
            AssertStreamsShouldProcess( new char[] { ( char) byte.MinValue, ( char ) 0, ( char ) byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Double. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessDouble( double arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            double actual = decoder.DecodeDouble();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Double. </summary>
        [TestMethod]
        public void StreamsShouldProcessDouble()
        {
            AssertStreamsShouldProcessDouble( double.MinValue );
            AssertStreamsShouldProcessDouble( double.MaxValue );
            AssertStreamsShouldProcessDouble( ( double ) 0 );
        }

        /// <summary>   Assert codec should process <see langword="double"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( double[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            double[] actual = decoder.DecodeDoubleVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="double"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessDoubles()
        {
            AssertStreamsShouldProcess( new double[] { double.MinValue, 0, double.MaxValue } );
        }

        /// <summary>   Assert codec should process DynamicOpaque. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessDynamicOpaque( byte[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.EncodeDynamicOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            byte[] actual = decoder.DecodeDynamicOpaque();
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process DynamicOpaque. </summary>
        [TestMethod]
        public void StreamsShouldProcessDynamicOpaque()
        {
            AssertStreamsShouldProcessDynamicOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }

        /// <summary>   Assert codec should process Float. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( float arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            float actual = decoder.DecodeFloat();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Float. </summary>
        [TestMethod]
        public void StreamsShouldProcessFloat()
        {
            AssertStreamsShouldProcess( float.MinValue );
            AssertStreamsShouldProcess( float.MaxValue );
            AssertStreamsShouldProcess( ( float ) 0 );
        }


        /// <summary>   Assert codec should process <see langword="float"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( float[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            float[] actual = decoder.DecodeFloatVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="float"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessFloats()
        {
            AssertStreamsShouldProcess( new float[] { float.MinValue, 0, float.MaxValue } );
        }


        /// <summary>   Assert codec should process Integer. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessInteger( int arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            int actual = decoder.DecodeInt();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
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
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessLong( long arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            long actual = decoder.DecodeLong();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   Assert codec should process <see langword="int"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( int[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            int[] actual = decoder.DecodeIntVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="int"/>s. </summary>
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

        /// <summary>   Assert codec should process <see langword="long"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( long[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            long[] actual = decoder.DecodeLongVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="long"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessLongs()
        {
            AssertStreamsShouldProcess( new long[] { long.MinValue, 0, long.MaxValue } );
        }


        /// <summary>   Assert codec should process Opaque. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessOpaque( byte[] arg1 )
        {

            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.EncodeOpaque( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding(); 
            byte[] actual = decoder.DecodeOpaque( arg1.Length );
            decoder.EndDecoding();

            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process Opaque. </summary>
        [TestMethod]
        public void StreamsShouldProcessOpaque()
        {
            AssertStreamsShouldProcessOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
        }


        /// <summary>   Assert codec should process Short. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessShort( short arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            short actual = decoder.DecodeShort();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process Short. </summary>
        [TestMethod]
        public void StreamsShouldProcessShort()
        {
            AssertStreamsShouldProcessShort( short.MinValue );
            AssertStreamsShouldProcessShort( short.MaxValue );
            AssertStreamsShouldProcessShort( ( short ) 0 );
        }

        /// <summary>   Assert codec should process <see langword="short"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( short[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            short[] actual = decoder.DecodeShortVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="short"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessShorts()
        {
            AssertStreamsShouldProcess( new short[] { short.MinValue, 0, short.MaxValue } );
        }


        /// <summary>   Assert codec should process string. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcessString( string arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            string actual = decoder.DecodeString();
            decoder.EndDecoding();
            Assert.AreEqual( arg1, actual );
        }

        /// <summary>   (Unit Test Method) codec should process string. </summary>
        [TestMethod]
        public void StreamsShouldProcessString()
        {
            AssertStreamsShouldProcessString( "XDR serialized string" );
        }

        /// <summary>   Assert codec should process <see langword="string"/>s. </summary>
        /// <param name="arg1"> The first argument. </param>
        private static void AssertStreamsShouldProcess( string[] arg1 )
        {
            XdrBufferEncodingStream encoder = new( 1024 );
            arg1.Encode( encoder );

            XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 );
            decoder.BeginDecoding();
            string[] actual = decoder.DecodeStringVector();
            decoder.EndDecoding();
            Assert.IsTrue( Enumerable.SequenceEqual( arg1, actual ) );
        }

        /// <summary>   (Unit Test Method) codec should process <see langword="string"/>s. </summary>
        [TestMethod]
        public void StreamsShouldProcessStrings()
        {
            AssertStreamsShouldProcess( new string[] { "XDR", "serialized", "string" } );
        }


    }
}

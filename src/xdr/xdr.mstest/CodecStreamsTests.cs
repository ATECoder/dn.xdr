using System.Net;
using System.Text;

namespace cc.isr.XDR.MSTest;

[TestClass]
public class CodecStreamsTests
{

    #region " construction and cleanup "

    /// <summary> Initializes the test class before running the first test. </summary>
    /// <param name="testContext"> Gets or sets the test context which provides information about
    /// and functionality for the current test run. </param>
    /// <remarks>Use ClassInitialize to run code before running the first test in the class</remarks>
    [ClassInitialize()]
    public static void InitializeTestClass( TestContext testContext )
    {
        try
        {
            string methodFullName =  $"{testContext.FullyQualifiedTestClassName}.{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}";
            if ( Logger is null )
                Console.WriteLine( methodFullName );
            else
                Logger?.LogMemberInfo( methodFullName );
        }
        catch ( Exception ex )
        {
            if ( Logger is null )
                Console.WriteLine( $"Failed initializing the test class: {ex}" );
            else
                Logger.LogMemberError( "Failed initializing the test class:", ex );

            // cleanup to meet strong guarantees

            try
            {
                CleanupTestClass();
            }
            finally
            {
            }
        }
    }

    /// <summary> Cleans up the test class after all tests in the class have run. </summary>
    /// <remarks> Use <see cref="CleanupTestClass"/> to run code after all tests in the class have run. </remarks>
    [ClassCleanup()]
    public static void CleanupTestClass()
    { }

    private IDisposable? _loggerScope;

    /// <summary> Initializes the test class instance before each test runs. </summary>
    [TestInitialize()]
    public void InitializeBeforeEachTest()
    {
        this._loggerScope = Logger?.BeginScope( this.TestContext?.TestName ?? string.Empty );

    }

    /// <summary> Cleans up the test class instance after each test has run. </summary>
    [TestCleanup()]
    public void CleanupAfterEachTest()
    {
        this._loggerScope?.Dispose();
    }

    /// <summary>
    /// Gets or sets the test context which provides information about and functionality for the
    /// current test run.
    /// </summary>
    /// <value> The test context. </value>
    public TestContext? TestContext { get; set; }

    /// <summary>   Gets a logger instance for this category. </summary>
    /// <value> The logger. </value>
    public static ILogger<CodecStreamsTests>? Logger { get; } = LoggerProvider.InitLogger<CodecStreamsTests>();

    #endregion

    #region " initialization tests "

    /// <summary>   (Unit Test Method) 00 logger should be enabled. </summary>
    /// <remarks>   2023-05-31. </remarks>
    [TestMethod]
    public void A00LoggerShouldBeEnabled()
    {
        Assert.IsNotNull( Logger, $"{nameof( Logger )} should initialize" );
        Assert.IsTrue( Logger.IsEnabled( LogLevel.Information ),
            $"{nameof( Logger )} should be enabled for the {LogLevel.Information} {nameof( LogLevel )}" );
    }

    #endregion

    #region " codec tests "

    /// <summary>   Assert codecs should encode and decode <see cref="bool"/>. </summary>
    /// <param name="value">    parameter of type <see cref="bool"/> to encode and decode. </param>
    private static void AssertShouldDecode( bool value )
    {
        BooleanXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        BooleanXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Boolean. </summary>
    [TestMethod]
    public void ShouldDecodeBoolean()
    {
        AssertShouldDecode( true );
        AssertShouldDecode( false );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="byte"/>. </summary>
    /// <param name="value">    parameter of type <see cref="byte"/> to encode and decode. </param>
    private static void AssertShouldDecode( byte value )
    {
        ByteXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        ByteXdrCodec result = new();
        XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode <see cref="byte"/>. </summary>
    [TestMethod]
    public void ShouldDecodeByte()
    {
        AssertShouldDecode( byte.MinValue );
        AssertShouldDecode( byte.MaxValue );
        AssertShouldDecode( ( byte ) 0 );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="byte"/>s. </summary>
    /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
    private static void AssertShouldDecode( byte[] value )
    {
        BytesXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        BytesXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.IsTrue( Enumerable.SequenceEqual( request.GetValue(), result.GetValue() ) );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode <see cref="byte"/>s. </summary>
    [TestMethod]
    public void ShouldDecodeBytes()
    {
        AssertShouldDecode( new byte[] { byte.MinValue, 0, byte.MaxValue } );
    }

    /// <summary>   Assert codecs should encode and decode Character. </summary>
    /// <param name="value">    parameter of type <see cref="char"/> to encode and decode. </param>
    private static void AssertShouldDecode( char value )
    {
        CharXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        CharXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Character. </summary>
    [TestMethod]
    public void ShouldDecodeCharacter()
    {
        for ( int i = 0; i < byte.MaxValue; i++ )
        {
            AssertShouldDecode( Encoding.ASCII.GetString( new byte[] { ( byte ) i } )[0] );
        }
    }

    /// <summary>   Assert codecs should encode and decode Double. </summary>
    /// <param name="value">    parameter of type <see cref="double"/> to encode and decode. </param>
    private static void AssertShouldDecode( double value )
    {
        DoubleXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        DoubleXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Double. </summary>
    [TestMethod]
    public void ShouldDecodeDouble()
    {
        AssertShouldDecode( double.MinValue );
        AssertShouldDecode( double.MaxValue );
        AssertShouldDecode( ( double ) 0 );
    }

    /// <summary>   Assert codecs should encode and decode DynamicOpaque. </summary>
    /// <param name="value">    parameter of type <see cref="byte[]"/> to encode and decode. </param>
    private static void AssertShouldDecodeDynamicOpaque( byte[] value )
    {
        DynamicOpaqueXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        DynamicOpaqueXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.IsTrue( Enumerable.SequenceEqual( request.GetValue(), result.GetValue() ) );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode DynamicOpaque. </summary>
    [TestMethod]
    public void ShouldDecodeDynamicOpaque()
    {
        AssertShouldDecodeDynamicOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="float"/>. </summary>
    /// <param name="value">    parameter of type <see cref="float"/> to encode and decode. </param>
    private static void AssertShouldDecode( float value )
    {
        FloatXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        FloatXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Float. </summary>
    [TestMethod]
    public void ShouldDecodeFloat()
    {
        AssertShouldDecode( float.MinValue );
        AssertShouldDecode( float.MaxValue );
        AssertShouldDecode( ( float ) 0 );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="int"/>. </summary>
    /// <param name="value">    parameter of type <see cref="int"/> to encode and decode. </param>
    private static void AssertShouldDecode( int value )
    {
        IntXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        IntXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Integer. </summary>
    [TestMethod]
    public void ShouldDecodeInteger()
    {
        AssertShouldDecode( int.MinValue );
        AssertShouldDecode( int.MaxValue );
        AssertShouldDecode( ( int ) 0 );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="uint"/>. </summary>
    /// <param name="value">    parameter of type <see cref="uint"/> to encode and decode. </param>
    private static void AssertShouldDecode( uint value )
    {
        UIntXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        UIntXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode unsigned integer. </summary>
    [TestMethod]
    public void ShouldDecodeUnsignedInteger()
    {
        AssertShouldDecode( uint.MinValue );
        AssertShouldDecode( uint.MaxValue );
        AssertShouldDecode( ( uint ) 0 );
        AssertShouldDecode( unchecked(( uint ) ( int ) -24) );
    }

    /// <summary>   Assert codecs should encode and decode <see cref="long"/>. </summary>
    /// <param name="value">    parameter of type <see cref="long"/> to encode and decode. </param>
    private static void AssertShouldDecode( long value )
    {
        LongXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        LongXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Long. </summary>
    [TestMethod]
    public void ShouldDecodeLong()
    {
        AssertShouldDecode( long.MinValue );
        AssertShouldDecode( long.MaxValue );
        AssertShouldDecode( ( long ) 0 );
    }

    /// <summary>   Assert codecs should encode and decode Opaque. </summary>
    /// <remarks>   2023-01-26. </remarks>
    /// <param name="values">   parameter of type <see cref="byte[]"/> to encode and decode. </param>
    private static void AssertShouldDecodeOpaque( byte[] values )
    {
        OpaqueXdrCodec request = new( values );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        OpaqueXdrCodec result = new( encoder.GetEncodedData() );
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), 1024 ); // encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        byte[] expected = request.GetValue();
        byte[] actual = new byte[expected.Length];
        Array.Copy( result.GetValue(), actual, expected.Length );

        Assert.IsTrue( Enumerable.SequenceEqual( expected, actual ) );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Opaque. </summary>
    [TestMethod]
    public void ShouldDecodeOpaque()
    {
        AssertShouldDecodeOpaque( new byte[] { byte.MinValue, 0, byte.MaxValue } );
    }


    /// <summary>   Assert codecs should encode and decode <see cref="short"/>. </summary>
    /// <param name="value">    parameter of type <see cref="short"/> to encode and decode. </param>
    private static void AssertShouldDecode( short value )
    {
        ShortXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        ShortXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode Short. </summary>
    [TestMethod]
    public void ShouldDecodeShort()
    {
        AssertShouldDecode( short.MinValue );
        AssertShouldDecode( short.MaxValue );
        AssertShouldDecode( ( short ) 0 );
    }


    /// <summary>   Assert codecs should encode and decode <see cref="string"/>. </summary>
    /// <param name="value">    parameter of type <see cref="string"/> to encode and decode. </param>
    private static void AssertShouldDecode( string value )
    {
        StringXdrCodec request = new( value );
        using XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        StringXdrCodec result = new();
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result.Decode( decoder );
        decoder.EndDecoding();

        Assert.AreEqual( request.Value, result.Value );
    }

    /// <summary>   (Unit Test Method) codecs should encode and decode string. </summary>
    [TestMethod]
    public void ShouldDecodeString()
    {
        AssertShouldDecode( "XDR serialized string" );
    }

    private static void AssertShouldEncode( System.Diagnostics.TraceEventType value )
    {
        using XdrBufferEncodingStream encoder = new( 1024 );
        value.Encode( encoder );

        System.Diagnostics.TraceEventType result;
        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        result = ( System.Diagnostics.TraceEventType ) decoder.DecodeInt();
        decoder.EndDecoding();

        Assert.AreEqual( value, result, $"{nameof( System.Diagnostics.TraceEventType )}.{value}({( int ) value}) should decode" );
    }

    [TestMethod]
    public void ShouldEncodeEnum()
    {
        AssertShouldEncode( System.Diagnostics.TraceEventType.Warning );
        AssertShouldEncode( System.Diagnostics.TraceEventType.Error );
    }

    private static void AsserShouldEncode( IPAddress expected )
    {
        using XdrBufferEncodingStream encoder = new( 1024 );
        expected.Encode( encoder );

        using XdrDecodingStreamBase decoder = new XdrBufferDecodingStream( encoder.GetEncodedData(), encoder.GetEncodedDataLength() );
        decoder.BeginDecoding();
        IPAddress actual = decoder.DecodeIPAddress();
        decoder.EndDecoding();
        Assert.AreEqual( expected.ToString(), actual.ToString(), $"{nameof( IPAddress )}.{expected} should decode" );
    }

    [TestMethod]
    public void ShouldEncodeIPAddress()
    {
        AsserShouldEncode( IPAddress.Parse( "255.255.255.255" ) );
        AsserShouldEncode( IPAddress.Parse( "1.1.1.1" ) );
        AsserShouldEncode( IPAddress.Parse( "192.160.0.1" ) );
        AsserShouldEncode( IPAddress.Parse( "127.0.0.1" ) );
    }

    #endregion

}

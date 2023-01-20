using cc.isr.XDR.EnumExtensions;

namespace cc.isr.XDR.MSTest;

/// <summary>   (Unit Test Class) a support tests. </summary>
[TestClass]
public class SupportTests
{

    #region " fixture construction and cleanup "

    /// <summary>   Initializes the fixture. </summary>
    /// <param name="context">  The context. </param>
    [ClassInitialize]
    public static void InitializeFixture( TestContext context )
    {
        try
        {
            Console.WriteLine( $"{context.FullyQualifiedTestClassName}.{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}" );
            _classTestContext = context;
            Console.WriteLine( @$"{DateTime.Now:yyyy:MM:dd:hh:mm:ss.fff} starting {_classTestContext.FullyQualifiedTestClassName}.{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}" );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"Failed initializing fixture: \n{ex} " );
            CleanupFixture();
        }
    }

    public TestContext? TestContext { get; set; }

    private static TestContext? _classTestContext;

    /// <summary>   Cleanup fixture. </summary>
    [ClassCleanup]
    public static void CleanupFixture()
    {
    }

    #endregion

    #region " enum extensions "

    /// <summary>   Assert should get description. </summary>
    /// <param name="value">                The value. </param>
    /// <param name="expectedDescription">  Information describing the expected. </param>
    private static void AssertShouldGetDescription( XdrExceptionReason value, string expectedDescription )
    {
        string actual = value.GetDescription();
        Assert.AreEqual( expectedDescription, actual );
    }

    /// <summary>   (Unit Test Method) message type should get description. </summary>
    [TestMethod]
    public void XdrExceptionReasonShouldGetDescription()
    {
        AssertShouldGetDescription( XdrExceptionReason.XdrSuccess, "The remote procedure call was carried out successfully." );
    }

    /// <summary>   Assert <see langword="int"/> should cast to <see cref="XdrExceptionReason"/>. </summary>
    /// <param name="expected"> The expected value. </param>
    private static void AssertIntShouldCastToExceptionReason( int expected )
    {
        XdrExceptionReason actual = expected.ToExceptionReason();
        Assert.AreEqual( expected, ( int ) actual );
    }

    /// <summary>   (Unit Test Method) <see langword="int"/> should cast to <see cref="XdrExceptionReason"/>. </summary>
    [TestMethod]
    public void IntShouldCastToExceptionReason()
    {
        int value = 0;
        int maxValue = 0;
        foreach ( var enumValue in Enum.GetValues( typeof( XdrExceptionReason ) ) )
        {
            value = ( int ) enumValue;
            maxValue = value > maxValue ? value : maxValue;
            AssertIntShouldCastToExceptionReason( value );
        }
        _ = Assert.ThrowsException<ArgumentException>( () => { AssertIntShouldCastToExceptionReason( maxValue + 1 ); } );
    }

    #endregion

}

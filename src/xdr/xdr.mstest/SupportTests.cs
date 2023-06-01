using cc.isr.XDR.EnumExtensions;

namespace cc.isr.XDR.MSTest;

/// <summary>   (Unit Test Class) a support tests. </summary>
[TestClass]
public class SupportTests
{

    #region " fixture construction and cleanup "

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
    public static ILogger<SupportTests>? Logger { get; } = LoggerProvider.InitLogger<SupportTests>();

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

    /// <summary>   Assert <see cref="int"/> should cast to <see cref="XdrExceptionReason"/>. </summary>
    /// <param name="expected"> The expected value. </param>
    private static void AssertIntShouldCastToExceptionReason( int expected )
    {
        XdrExceptionReason actual = expected.ToExceptionReason();
        Assert.AreEqual( expected, ( int ) actual );
    }

    /// <summary>   (Unit Test Method) <see cref="int"/> should cast to <see cref="XdrExceptionReason"/>. </summary>
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

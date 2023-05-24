using System.Text;

using cc.isr.XDR.Logging;
using cc.isr.XDR.MSTest.Codecs;

namespace cc.isr.XDR.MSTest.MockOncRpc;

[TestClass]
public class MockOncRpcClientTests
{

    #region " fixture construction and cleanup "

    /// <summary>   Initializes the fixture. </summary>
    /// <param name="testContext"> Gets or sets the test context which provides information about
    /// and functionality for the current test run. </param>
    [ClassInitialize]
    public static void InitializeFixture( TestContext testContext )
    {
        try
        {
            _classTestContext = context;
            Logger.Writer.LogInformation( $"{_classTestContext.FullyQualifiedTestClassName}.{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}" );
        }
        catch ( Exception ex )
        {
            Logger.Writer.LogMemberError( $"Failed initializing fixture:", ex );
            CleanupFixture();
        }
    }

    /// <summary>
    /// Gets or sets the test context which provides information about and functionality for the
    /// current test run.
    /// </summary>
    /// <value> The test context. </value>
    public TestContext? TestContext { get; set; }

    private static TestContext? _classTestContext;

    /// <summary>   Cleanup fixture. </summary>
    [ClassCleanup]
    public static void CleanupFixture()
    { }
    #endregion

    /// <summary>   Assert client should ping. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldPing( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to ping: " );
        client.CallRemoteProcedureNull();
        Logger.Writer.LogInformation( "okay" );
    }

    /// <summary>   Assert client should ping. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.054,About to ping:
    /// 2023-02-02 09:45:52.054,okay
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldPing()
    {
        MockOncRpcClient client = new();
        AssertClientShouldPing( client );
    }

    /// <summary>   (Unit Test Method) client should ping version 2. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.059,About to ping:
    /// 2023-02-02 09:45:52.059,okay
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldPingVersion2()
    {
        MockOncRpcClient client = new();
        AssertClientShouldPing( client );
    }


    /// <summary>   Assert client should echo messages. </summary>
    /// <param name="client">   The client. </param>
    /// <param name="messages"> The messages. </param>
    private static void AssertClientShouldEchoMessages( MockOncRpcClient client, string[] messages )
    {
        foreach ( string message in messages )
        {
            System.Console.Out.Write( $"checking echo of '{message}': " );
            string echoed = client.CallRemoteProcedureEcho( message );
            Assert.AreEqual( message, echoed, $"answer '{echoed}' does not match '{message}' call" );
            System.Console.Out.WriteLine( $"Okay; echoed: '{echoed}'" );
        }
    }

    /// <summary>   (Unit Test Method) client should echo messages. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// checking echo of 'UNIX': Okay; echoed: 'UNIX'
    /// checking echo of 'AUTH': Okay; echoed: 'AUTH'
    /// checking echo of 'is like': Okay; echoed: 'is like'
    /// checking echo of '*NO* authentication': Okay; echoed: '* NO* authentication'
    /// checking echo of '--': Okay; echoed: '--'
    /// checking echo of 'it': Okay; echoed: 'it'
    /// checking echo of 'uses': Okay; echoed: 'uses'
    /// checking echo of '*NO CRYPTOGRAPHY*': Okay; echoed: '* NO CRYPTOGRAPHY*'
    /// checking echo of 'for securing': Okay; echoed: 'for securing'
    /// checking echo of 'ONC/RPC messages': Okay; echoed: 'ONC/RPC messages'
    /// </code></remarks>
    [TestMethod]
    public void ClientShouldEchoMessages()
    {
        MockOncRpcClient client = new();
        string[] messages = new string[] { "UNIX", "AUTH", "is like", "*NO* authentication", "--", "it", "uses", "*NO CRYPTOGRAPHY*", "for securing", "ONC/RPC messages" };
        AssertClientShouldEchoMessages( client, messages );
    }

    /// <summary>   Assert client should echo. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldEcho( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to echo: " );
        string expected = "Hello, Remote Tea!";
        string actual = client.CallRemoteProcedureEcho( expected );
        Assert.AreEqual( expected, actual );
        Logger.Writer.LogInformation( $"Okay; echoed: '{actual}'" );
    }

    /// <summary>   (Unit Test Method) client should echo. </summary>
    /// <remarks>   
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.052,About to echo:
    /// 2023-02-02 09:45:52.052,Okay; echoed: 'Hello, Remote Tea!'
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldEcho()
    {
        MockOncRpcClient client = new();
        AssertClientShouldEcho( client );
    }

    /// <summary>   Assert client should concatenate. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldConcatenate( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to concatenate: " );
        StringVectorCodec strings = new();
        strings.SetValues( new StringCodec[] { new StringCodec( "Hello, " ), new StringCodec( "Remote " ), new StringCodec( "Tea!" ) } );
        string expected = "Hello, Remote Tea!";
        string actual = client.CallRemoteProcedureConcatenateInputParameters( strings );
        Assert.AreEqual( expected, actual );
        Logger.Writer.LogInformation( $"Okay; echoed: '{actual}'" );
    }

    /// <summary>   (Unit Test Method) client should concatenate. </summary>
    /// <remarks>   
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.050,About to concatenate:
    /// 2023-02-02 09:45:52.051,Okay; echoed: 'Hello, Remote Tea!'
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldConcatenate()
    {
        MockOncRpcClient client = new();
        AssertClientShouldConcatenate( client );
    }

    /// <summary>   Assert client should concatenate exactly. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldConcatenateExactly( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to concatenating exactly three strings: " );
        string expected = "(1:Hello )(2:Remote )(3:Tea!)";
        string actual = client.CallRemoteProcedureConcatenatedThreeItems( "(1:Hello )", "(2:Remote )", "(3:Tea!)" );
        Assert.AreEqual( expected, actual );
        Logger.Writer.LogInformation( $"The three arguments concatenated: '{actual}'" );
    }

    /// <summary>   (Unit Test Method) client should concatenate exactly. </summary>
    /// <remarks>   
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.051,About to concatenating exactly three strings:
    /// 2023-02-02 09:45:52.052, The three arguments concatenated: '(1:Hello )(2:Remote )(3:Tea!)'
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldConcatenateExactly()
    {
        MockOncRpcClient client = new();
        AssertClientShouldConcatenateExactly( client );
    }

    /// <summary>   Assert client should check for foo. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldCheckForFoo( MockOncRpcClient client )
    {
        Assert.IsFalse( client.CallRemoteProcedureCompareInputToFoo( EnumFoo.BAR ), $"oops: but a {EnumFoo.BAR} is not a {EnumFoo.FOO}!" );
        Assert.IsTrue( client.CallRemoteProcedureCompareInputToFoo( EnumFoo.FOO ), $"oops: a {EnumFoo.FOO} should be a {EnumFoo.FOO}!" );
    }

    /// <summary>   (Unit Test Method) client should check for foo. </summary>
    [TestMethod]
    public void ClientShouldCheckForFoo()
    {
        MockOncRpcClient client = new();
        AssertClientShouldCheckForFoo( client );
    }

    /// <summary>   Assert client should get foo. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldGetFoo( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to get a foo: " );
        Assert.AreEqual( client.CallRemoteProcedureReturnEnumFooValue(), ( int ) EnumFoo.FOO, $"oops: got a {EnumFoo.BAR} instead of a {EnumFoo.FOO}!" );
    }

    /// <summary>   (Unit Test Method) client should get foo. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.053,About to get a foo:
    /// </code></remarks>
    [TestMethod]
    public void ClientShouldGetFoo()
    {
        MockOncRpcClient client = new();
        AssertClientShouldGetFoo( client );
    }

    /// <summary>   Assert client should get numbered foo. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldGetNumberedFoo( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "About to get a numbered foo string: " );
        EnumFoo expectedValue = EnumFoo.FOO;
        string expected = MockOncRpcServer.ReturnYouAreFooValue( ( int ) expectedValue );
        string echo = client.CallRemoteProcedureReturnYouAreFooValue( expectedValue );
        Assert.AreEqual( expected, echo, $"oops: should echo '{expected}'" );
    }

    /// <summary>   (Unit Test Method) client should get numbered foo. </summary>
    /// <remarks>   
    /// <code>
    /// 2023-02-02 09:45:52.053,About to get a numbered foo string:
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldGetNumberedFoo()
    {
        MockOncRpcClient client = new();
        AssertClientShouldGetNumberedFoo( client );
    }

    /// <summary>   Assert client should prepend linked list. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldPrependLinkedList( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "Linked List test: " );
        LinkedListCodec node1 = new() {
            Foo = 0
        };
        LinkedListCodec node2 = new() {
            Foo = 8
        };
        node1.Next = node2;
        LinkedListCodec node3 = new() {
            Foo = 15
        };
        node2.Next = node3;
        LinkedListCodec? list = client.CallRemoteProcedurePrependLinkedList( node1 );
        LinkedListCodec? expected = MockOncRpcServer.PrependLinkedList( node1 );
        Assert.IsNotNull( list, "list should get built" );
        LinkedListCodec? actual = list;
        int i = 0;
        StringBuilder builder = new();
        while ( expected != null )
        {
            i++;
            Assert.IsNotNull( actual, $"node{i} actual list should have the same number of nodes as expected" ); ;
            Assert.AreEqual( expected.Foo, actual.Foo, $"nodes {i} should have the same value" );
            _ = builder.Append( actual.Foo + ", " );
            actual = actual.Next;
            expected = expected.Next;
        }
        Logger.Writer.LogInformation( $"built list {builder}" );
    }

    /// <summary>   (Unit Test Method) client should prepend linked list. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.060,Linked List test:
    /// 2023-02-02 09:45:52.060,built list 42, 0, 8, 15,
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldPrependLinkedList()
    {
        MockOncRpcClient client = new();
        AssertClientShouldPrependLinkedList( client );
    }


    /// <summary>   Assert client should link linked list. </summary>
    /// <param name="client">   The client. </param>
    private static void AssertClientShouldLinkLinkedList( MockOncRpcClient client )
    {
        Logger.Writer.LogInformation( "Linking Linked Lists test: " );
        LinkedListCodec node1 = new() {
            Foo = 0
        };
        LinkedListCodec node2 = new() {
            Foo = 8
        };
        LinkedListCodec node3 = new() {
            Foo = 15
        };
        node2.Next = node3;
        LinkedListCodec? list = client.CallRemoteProcedureLinkListItems( node2, node1 );
        // link the lists as expected by the server
        node2.Next = node1;
        LinkedListCodec? expected = node2;
        Assert.IsNotNull( list, "list should get built" );
        LinkedListCodec? actual = list;
        int i = 0;
        StringBuilder builder = new();
        while ( expected != null )
        {
            i++;
            Assert.IsNotNull( actual, $"node{i} actual list should have the same number of nodes as expected" ); ;
            Assert.AreEqual( expected.Foo, actual.Foo, $"nodes {i} should have the same value" );
            _ = builder.Append( actual.Foo + ", " );
            actual = actual.Next;
            expected = expected.Next;
        }
        Logger.Writer.LogInformation( $"built list {builder}" );
    }

    /// <summary>   (Unit Test Method) client should link linked list. </summary>
    /// <remarks>
    /// <code>
    /// Standard Output: 
    /// 2023-02-02 09:45:52.054,Linking Linked Lists test:
    /// 2023-02-02 09:45:52.054, built list 8, 0,
    /// </code>
    /// </remarks>
    [TestMethod]
    public void ClientShouldLinkLinkedList()
    {
        MockOncRpcClient client = new();
        AssertClientShouldLinkLinkedList( client );
    }

}

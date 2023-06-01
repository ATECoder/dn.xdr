using System.Text;

using cc.isr.XDR.MSTest.Codecs;

namespace cc.isr.XDR.MSTest.MockOncRpc;

public partial class MockOncRpcServer
{

    #region " handle procedure calls "

    /// <summary>   Dispatch (handle) an ONC/RPC request from a client. </summary>
    /// <remarks>
    /// This interface has some fairly deep semantics, so please read the description above for how
    /// to use it properly. For background information about fairly deep semantics, please also refer
    /// to <i>Gigzales</i>, <i>J</i>.: Semantics considered harmful. Addison-Reilly, 1992, ISBN 0-542-
    /// 10815-X. <para>
    ///  
    /// See the introduction to this class for examples of how to use this interface properly.</para>
    /// </remarks>
    /// <param name="call">         <see cref="MockOncRpcCallHandler"/> about the call to handle, 
    ///                             like the caller's Internet address, the ONC/RPC 
    ///                             call header, etc. </param>
    /// <param name="program">      Program number requested by client. </param>
    /// <param name="version">      Version number requested. </param>
    /// <param name="procedure">    Procedure number requested. </param>
    public static void DispatchOncRpcCall( MockOncRpcCallHandler call, int version, int procedure )
    {
        if ( version == RpcProgramConstants.Version1 )
        {
            MockOncRpcServer.DispatchOncRpcCall( call, ( RemoteProceduresVersion1 ) procedure );
        }
        else if ( version == RpcProgramConstants.Version2 )
        {
            MockOncRpcServer.DispatchOncRpcCall( call, ( RemoteProceduresVersion2 ) procedure );
        }
        else
        {
        }
    }

    /// <summary>   Dispatch (handle) an ONC/RPC request from a client. </summary>
    /// <param name="call">         The call. </param>
    /// <param name="procedure">    The procedure. </param>
    private static void DispatchOncRpcCall( MockOncRpcCallHandler call, RemoteProceduresVersion1 procedure )
    {
        switch ( procedure )
        {
            case RemoteProceduresVersion1.Nop:
                {
                    // ping
                    call.RetrieveCall( VoidXdrCodec.VoidXdrCodecInstance );
                    MockOncRpcServer.Nop();
                    call.Reply( VoidXdrCodec.VoidXdrCodecInstance );
                    break;
                }
            case RemoteProceduresVersion1.Echo:
                {
                    StringXdrCodec request = new();
                    call.RetrieveCall( request );
                    StringXdrCodec result = new( MockOncRpcServer.EchoInput( request.Value ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion1.ConcatenateInputParameters:
                {
                    StringVectorCodec request = new();
                    call.RetrieveCall( request );
                    StringXdrCodec result = new( MockOncRpcServer.ConcatenateInputStringVector( request ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion1.CompareInputToFoo:
                {
                    IntXdrCodec request = new();
                    call.RetrieveCall( request );
                    BooleanXdrCodec result = new( MockOncRpcServer.CompareInputToFoo( request.Value ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion1.ReturnEnumFooValue:
                {
                    call.RetrieveCall( VoidXdrCodec.VoidXdrCodecInstance );
                    IntXdrCodec result = new( MockOncRpcServer.ReturnEnumFooValue() );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion1.PrepentLinkedList:
                {
                    LinkedListCodec request = new();
                    call.RetrieveCall( request );
                    LinkedListCodec result = MockOncRpcServer.PrependLinkedList( request );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion1.RemoteProcedureReadSomeResult:
                {
                    call.RetrieveCall( VoidXdrCodec.VoidXdrCodecInstance );
                    SomeResultCodec result = MockOncRpcServer.ReadSomeResult();
                    call.Reply( result );
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>   Dispatch (handle) an ONC/RPC request from a client. </summary>
    /// <param name="call">         <see cref="MockOncRpcCallHandler"/> about the call to handle, 
    ///                             like the caller's Internet address, the ONC/RPC
    ///                             call header, etc. </param>
    /// <param name="procedure">    Procedure number requested. </param>
    private static void DispatchOncRpcCall( MockOncRpcCallHandler call, RemoteProceduresVersion2 procedure )
    {
        switch ( procedure )
        {
            case RemoteProceduresVersion2.Nop:
                {
                    call.RetrieveCall( VoidXdrCodec.VoidXdrCodecInstance );
                    MockOncRpcServer.Nop();
                    call.Reply( VoidXdrCodec.VoidXdrCodecInstance );
                    break;
                }
            case RemoteProceduresVersion2.ConcatenateTwoValues:
                {
                    DualStringsCodec request = new();
                    call.RetrieveCall( request );
                    StringXdrCodec result = new( MockOncRpcServer.ConcatenateTwoValues( request.FirstValue, request.SecondValue ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion2.ConcatenateThreeItems:
                {
                    TripleStringsCodec request = new();
                    call.RetrieveCall( request );
                    StringXdrCodec result = new( MockOncRpcServer.ConcatenateThreeItems( request.One, request.Two, request.Three ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion2.ReturnYouAreFooValue:
                {
                    IntXdrCodec request = new();
                    call.RetrieveCall( request );
                    StringXdrCodec result = new( MockOncRpcServer.ReturnYouAreFooValue( request.Value ) );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion2.LinkListItems:
                {
                    DualLinkedListsCodec request = new();
                    call.RetrieveCall( request );
                    LinkedListCodec result = MockOncRpcServer.LinkListItems( request.List1!, request.List2! );
                    call.Reply( result );
                    break;
                }
            case RemoteProceduresVersion2.ProcessFourArguments:
                {
                    StringTripleIntegerCodec request = new();
                    call.RetrieveCall( request );
                    MockOncRpcServer.ProcessFourArguments( request.A, request.B, request.C, request.D );
                    call.Reply( VoidXdrCodec.VoidXdrCodecInstance );
                    break;
                }
            default:
                break;
        }

    }

    #endregion

    #region " remote procedures "

    /// <summary>   No operation. </summary>
    public static void Nop()
    {
        // definitely nothing to do here...
    }

    /// <summary>   Echo the specified parameters. </summary>
    /// <param name="input">   value to echo. </param>
    /// <returns>   A string. </returns>
    public static string EchoInput( string input )
    {
        return input;
    }

    /// <summary>   Compare parameters to <see cref="EnumFoo.FOO"/>; return true if <paramref name="expected"/> equals <see cref="EnumFoo.FOO"/>. </summary>
    /// <param name="expected">   expected value. </param>
    /// <returns>   <see langword="true"/> if it succeeds; otherwise, <see langword="false"/>. </returns>
    public static bool CompareInputToFoo( int expected )
    {
        return expected == ( int ) EnumFoo.FOO;
    }

    /// <summary>   Return <see cref="EnumFoo.FOO"/>. </summary>
    /// <returns>   An int. </returns>
    public static int ReturnEnumFooValue()
    {
        return ( int ) EnumFoo.FOO;
    }

    /// <summary>   Concatenate input string vector. </summary>
    /// <param name="inputCodec">   the input codec. </param>
    /// <returns>   A string. </returns>
    public static string ConcatenateInputStringVector( StringVectorCodec inputCodec )
    {
        StringBuilder reply = new();
        foreach ( StringCodec stringCodec in inputCodec.GetValues() )
            _ = reply.Append( stringCodec.Value );
        return reply.ToString();
    }

    /// <summary>   Echo a linked list. </summary>
    /// <param name="linkedListCodec">   the linked list codec input. </param>
    /// <returns>   A <see cref="LinkedListCodec"/>. </returns>
    public static LinkedListCodec EchoLinkedList( LinkedListCodec linkedListCodec )
    {
        LinkedListCodec newNode = new( linkedListCodec );
        return newNode;
    }

    /// <summary>   Prepend a node to the given linked list. </summary>
    /// <param name="linkedListCodec">   the linked list codec input. </param>
    /// <returns>   A <see cref="LinkedListCodec"/>. </returns>
    public static LinkedListCodec PrependLinkedList( LinkedListCodec linkedListCodec )
    {
        LinkedListCodec newNode
            = new() {
                Foo = 42,
                Next = linkedListCodec
            };
        return newNode;
    }

    /// <summary>   Reads some result. </summary>
    /// <returns>   some result 1. </returns>
    public static SomeResultCodec ReadSomeResult()
    {
        SomeResultCodec res = new();
        return res;
    }

    /// <summary>   Concatenate two values. </summary>
    /// <param name="firstValue"> The first parameter of type <see cref="string"/> to concatenate and to
    ///                     encode and decode. </param>
    /// <param name="secondValue"> The second parameter of type <see cref="string"/> to concatenate and to
    ///                     encode and decode. </param>
    /// <returns>   A string. </returns>
    public static string ConcatenateTwoValues( string firstValue, string secondValue )
    {
        return $"{firstValue}{secondValue}";
    }

    /// <summary>   Concatenate three items. </summary>
    /// <remarks>   2023-01-21. </remarks>
    /// <param name="one">      The first parameter of type <see cref="string"/> to concatenate. </param>
    /// <param name="two">      The second parameter of type <see cref="string"/> to concatenate. </param>
    /// <param name="three">    The third parameter of type <see cref="string"/> to concatenate. </param>
    /// <returns>   A string. </returns>
    public static string ConcatenateThreeItems( string one, string two, string three )
    {
        return $"{one}{two}{three}";
    }

    /// <summary>   Return 'you are Foo' value. </summary>
    /// <param name="foo">  The foo. </param>
    /// <returns>   A string. </returns>
    public static string ReturnYouAreFooValue( int foo )
    {
        return $"You are foo {foo}.";
    }

    /// <summary>   Link linked list <paramref name="l2"/> as next item of linked list <paramref name="l1"/>. </summary>
    /// <param name="l1">   The first <see cref="LinkedListCodec"/>. </param>
    /// <param name="l2">   The second <see cref="LinkedListCodec"/>. </param>
    /// <returns>   An <see cref="LinkedListCodec"/>. </returns>
    public static LinkedListCodec LinkListItems( LinkedListCodec l1, LinkedListCodec l2 )
    {
        l1.Next = l2;
        return l1;
    }

    /// <summary>   Process four arguments. </summary>
    /// <param name="a">    A string to process. </param>
    /// <param name="b">    An int to process. </param>
    /// <param name="c">    An int to process. </param>
    /// <param name="d">    An int to process. </param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage( "Style", "IDE0060:Remove unused parameter", Justification = "<Pending>" )]
    public static void ProcessFourArguments( string a, int b, int c, int d )
    { }

    #endregion

}

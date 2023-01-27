using cc.isr.XDR.MSTest.Codecs;

namespace cc.isr.XDR.MSTest.MockOncRpc;

public class MockOncRpcClient
{

    #region " encoding and decoding "

    /// <summary>   Emulates a remote procedure call. </summary>
    /// <param name="request">  parameter of type <see cref="IXdrCodec"/> to send to the remote
    ///                         procedure call. </param>
    /// <param name="reply">    parameter of type <see cref="IXdrCodec"/> to receive the reply from
    ///                         the remote procedure call. </param>
    public static void Call( int procedureNumber, int versionNumber, IXdrCodec request, IXdrCodec reply )
    {
        XdrBufferEncodingStream encoder = new( 1024 );
        request.Encode( encoder );

        // the transport will handle the decoding of the data into the encoded information that is sent from the 
        // mock server and will be decoded below.

        XdrBufferEncodingStream transportEncoder = new( 1024 );
        XdrBufferDecodingStream transportDecoder = new( encoder.GetEncodedData(), 1024 );
        MockOncRpcTransport transport = new( transportEncoder, transportDecoder );

        MockOncRpcServer.DispatchOncRpcCall( new MockOncRpcCallHandler( transport ), versionNumber, procedureNumber );

        XdrBufferDecodingStream replyDecoder = new( transportEncoder.GetEncodedData(), 1024 );

        replyDecoder.BeginDecoding();
        reply.Decode( replyDecoder );
        replyDecoder.EndDecoding();
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.Nop"/> Null.
    /// </summary>
    public virtual void CallRemoteProcedureNull()
    {
        VoidXdrCodec request = VoidXdrCodec.VoidXdrCodecInstance;
        VoidXdrCodec reply = VoidXdrCodec.VoidXdrCodecInstance;
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.Nop, RpcProgramConstants.Version1, request, reply );
    }

    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.Echo"/>. </summary>
    /// <param name="value">    parameter of type <see cref="string"/> to send to the remote
    ///                         procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="string"/>). </returns>
    public virtual string CallRemoteProcedureEcho( string value )
    {
        StringXdrCodec request = new( value );
        StringXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.Echo, RpcProgramConstants.Version1, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.ConcatenateInputParameters"/>.
    /// </summary>
    /// <param name="request">  parameter of type <see cref="StringVectorCodec"/> to send to the
    ///                         remote procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="string"/>). </returns>
    public virtual string CallRemoteProcedureConcatenateInputParameters( StringVectorCodec request )
    {
        StringXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.ConcatenateInputParameters, RpcProgramConstants.Version1, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.CompareInputToFoo"/>.
    /// </summary>
    /// <param name="value">    parameter of type <see cref="EnumFoo"/> to send to the remote
    ///                         procedure call. </param>
    /// <returns>   Result from remote procedure call (of type boolean). </returns>
    public virtual bool CallRemoteProcedureCompareInputToFoo( EnumFoo value )
    {
        IntXdrCodec request = new( ( int ) value );
        BooleanXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.CompareInputToFoo, RpcProgramConstants.Version1, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.ReturnEnumFooValue"/>.
    /// </summary>
    /// <returns>   Result from remote procedure call (of type <see cref="EnumFoo"/>). </returns>
    public virtual int CallRemoteProcedureReturnEnumFooValue()
    {
        VoidXdrCodec request = VoidXdrCodec.VoidXdrCodecInstance;
        IntXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.ReturnEnumFooValue, RpcProgramConstants.Version1, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion1.PrepentLinkedList"/>.
    /// </summary>
    /// <param name="request">  the request of type <see cref="LinkedListCodec"/> to send to the
    ///                         remote procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="LinkedListCodec"/>). </returns>
    public virtual LinkedListCodec CallRemoteProcedurePrependLinkedList( LinkedListCodec request )
    {
        LinkedListCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.PrepentLinkedList, RpcProgramConstants.Version1, request, reply );
        return reply;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion2.ConcatenateTwoValues"/>.
    /// </summary>
    /// <returns>
    /// Result from remote procedure call (of type <see cref="SomeResultCodec"/>).
    /// </returns>
    public virtual SomeResultCodec CallRemoteProcedureReadSomeResult()
    {
        VoidXdrCodec request = VoidXdrCodec.VoidXdrCodecInstance;
        SomeResultCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion1.Nop, RpcProgramConstants.Version1, request, reply );
        return reply;
    }

    /// <summary>   Emulates calling remote procedure `ConcatenateTwoValues`. </summary>
    /// <remarks>   2023-01-26. </remarks>
    /// <param name="firstValue">   parameter of type <see cref="string"/> to concatenate and send to
    ///                             the remote procedure call. </param>
    /// <param name="secondValue">  parameter of type <see cref="string"/> to concatenate and send to
    ///                             the remote procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="string"/>). </returns>
    public virtual string CallRemoteProcedureConcatenateTwoValues( string firstValue, string secondValue )
    {
        DualStringsCodec request = new() {
            FirstValue = firstValue,
            SecondValue = secondValue
        };
        StringXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion2.ConcatenateTwoValues, RpcProgramConstants.Version2, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion2.ConcatenateThreeItems"/>.
    /// </summary>
    /// <param name="one">      parameter (of type <see cref="string"/>) to the remote procedure
    ///                         call. </param>
    /// <param name="two">      parameter (of type <see cref="string"/>) to the remote procedure
    ///                         call. </param>
    /// <param name="three">    parameter (of type <see cref="string"/>) to the remote procedure
    ///                         call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="string"/>). </returns>
    public virtual string CallRemoteProcedureConcatenatedThreeItems( string one, string two, string three )
    {
        TripleStringsCodec request = new() {
            One = one,
            Two = two,
            Three = three
        };
        StringXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion2.ConcatenateThreeItems, RpcProgramConstants.Version2, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion2.ReturnYouAreFooValue"/>.
    /// </summary>
    /// <param name="value">    parameter of type <see cref="EnumFoo"/> to send to the remote
    ///                         procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="string"/>). </returns>
    public virtual string CallRemoteProcedureReturnYouAreFooValue( EnumFoo value )
    {
        IntXdrCodec request = new( ( int ) value );
        StringXdrCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion2.ReturnYouAreFooValue, RpcProgramConstants.Version2, request, reply );
        return reply.Value;
    }

    /// <summary>
    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion2.LinkListItems"/>.
    /// </summary>
    /// <param name="list1">    parameter of type <see cref="LinkedListCodec"/> to link and send to
    ///                         the remote procedure call. </param>
    /// <param name="list2">    parameter of type <see cref="LinkedListCodec"/> to link and send to
    ///                         the remote procedure call. </param>
    /// <returns>   Result from remote procedure call (of type <see cref="LinkedListCodec"/>). </returns>
    public virtual LinkedListCodec CallRemoteProcedureLinkListItems( LinkedListCodec list1, LinkedListCodec list2 )
    {
        DualLinkedListsCodec request = new() {
            List1 = list1,
            List2 = list2
        };
        LinkedListCodec reply = new();
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion2.LinkListItems, RpcProgramConstants.Version2, request, reply );
        return reply;
    }

    /// Emulates calling remote procedure <see cref="RemoteProceduresVersion2.ProcessFourArguments"/>. </summary>
    /// <param name="a">    parameter of type <see cref="string"/> to send to the remote procedure call. </param>
    /// <param name="b">    parameter of type <see cref="EnumFoo"/> to send to the remote procedure call. </param>
    /// <param name="c">    parameter of type <see cref="EnumFoo"/> to send to the remote procedure call. </param>
    /// <param name="d">    parameter <see cref="int"/> to send to the remote procedure call. </param>
    public virtual void CallRemoteProcedureProcessFourArguments( string a, int b, int c, int d )
    {
        StringTripleIntegerCodec request = new() {
            A = a,
            B = b,
            C = c,
            D = d
        };
        VoidXdrCodec reply = VoidXdrCodec.VoidXdrCodecInstance;
        MockOncRpcClient.Call( ( int ) RemoteProceduresVersion2.ProcessFourArguments, RpcProgramConstants.Version2, request, reply );
    }

    #endregion

}

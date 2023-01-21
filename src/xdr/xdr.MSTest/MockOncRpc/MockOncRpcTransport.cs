namespace cc.isr.XDR.MSTest.MockOncRpc;

public class MockOncRpcTransport
{

    #region " construction "

    /// <summary>   Constructor. </summary>
    /// <param name="encoder">  The XDR stream which can be used for serializing the reply to this
    ///                         ONC/RPC call. </param>
    /// <param name="decoder">  The XDR stream which can be used for deserializing the parameters of
    ///                         this ONC/RPC call. </param>
    public MockOncRpcTransport( XdrBufferEncodingStream encoder, XdrBufferDecodingStream decoder )
    {
        this.Decoder = decoder;
        this.Encoder = encoder;
    }

    #endregion

    #region " members "

    /// <summary>
    /// Returns XDR stream which can be used for deserializing the parameters of this ONC/RPC call.
    /// </summary>
    /// <remarks>
    /// This method belongs to the lower-level access pattern when handling ONC/RPC calls.
    /// </remarks>
    /// <returns>   Reference to decoding XDR stream. </returns>
    public XdrBufferDecodingStream? Decoder { get; set; }

    /// <summary>
    /// Returns XDR stream which can be used for serializing the reply to this ONC/RPC call.
    /// </summary>
    /// <remarks>
    /// This method belongs to the lower-level access pattern when handling ONC/RPC calls.
    /// </remarks>
    /// <value>   Reference to encoding XDR stream. </value>
    public XdrBufferEncodingStream? Encoder { get; set; }

    #endregion

    #region " actions "

    /// <summary>   Retrieves the parameters sent within an ONC/RPC call message. </summary>
    /// <remarks>
    /// It also makes sure that the deserialization process is properly finished after the call
    /// parameters have been retrieved. Under the hood this method therefore calls
    /// <see cref="XdrBufferDecodingStream.EndDecoding()"/>
    /// to free any pending resources from the decoding stage.
    /// </remarks>
    /// <exception cref="OncRpcException">  Thrown when an ONC/RPC error condition occurs. </exception>
    /// <param name="call"> The call. </param>
    internal void RetrieveCall( IXdrCodec call )
    {
        if ( this.Decoder is null || call is null ) return;
        this.Decoder.BeginDecoding();
        call.Decode( this.Decoder );
        this.Decoder.EndDecoding();
    }

    /// <summary> Sends back an ONC/RPC reply to the original caller. </summary>
    /// <remarks>
    /// This is rather a low-level method, typically not used by applications. Dispatcher handling
    /// ONC/RPC calls have to use the <see cref="MockOncRpcCallHandler.Reply(IXdrCodec)"/>
    /// method instead on the call object supplied to the handler.
    /// </remarks>
    /// <exception cref="OncRpcException">  Thrown when an ONC/RPC error condition occurs. </exception>
    /// <param name="reply">    If not (<see langword="null"/>), then this parameter references the reply to
    ///                         be serialized after the reply message header. </param>
    internal void Reply( IXdrCodec reply )
    {
        if ( this.Encoder is null || reply is null ) return;
        this.Encoder.BeginEncoding( );
        reply.Encode( this.Encoder );
        this.Encoder.EndEncoding();
    }

    #endregion

}

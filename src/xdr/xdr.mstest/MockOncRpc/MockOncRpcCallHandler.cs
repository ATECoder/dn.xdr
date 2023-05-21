namespace cc.isr.XDR.MSTest.MockOncRpc;

public class MockOncRpcCallHandler
{
    /// <summary>
    /// Create an <see cref="MockOncRpcCallHandler"/> object and associate it with a ONC/RPC server
    /// transport.
    /// </summary>
    /// <remarks>
    /// Typically, <see cref="MockOncRpcCallHandler"/> objects are created by transports
    /// once before handling incoming calls using the same call handler object.
    /// To support multi-threaded handling of calls in the future (for UDP/IP),
    /// the transport is already divided from the call info.
    /// </remarks>
    /// <param name="transport">    ONC/RPC server transport. </param>
    internal MockOncRpcCallHandler( MockOncRpcTransport transport )
    {
        this.Transport = transport;
    }

    /// <summary>
    /// Gets or sets the associated transport from which we receive the ONC/RPC call parameters and
    /// to which we serialize the ONC/RPC reply. Never mess with this member or you might break all
    /// future extensions horribly -- but this warning probably only stimulates you...
    /// </summary>
    /// <value> The transport. </value>
    internal MockOncRpcTransport Transport { get; set; }

    /// <summary>   Retrieves the <see cref="IXdrCodec"/> request that was sent within an ONC/RPC call message. </summary>
    /// <remarks>
    /// It also makes sure that the deserialization process is properly finished after the call
    /// parameters have been retrieved.
    /// </remarks>
    /// <exception cref="OncRpcException">  Thrown when an ONC/RPC error condition occurs. </exception>
    /// <param name="request"> The <see cref="IXdrCodec"/> request that was sent within an ONC/RPC call message. </param>
    public virtual void RetrieveCall( IXdrCodec request )
    {
        this.Transport.RetrieveCall( request );
    }

    /// <summary> Sends back an ONC/RPC reply to the caller who sent in this call. </summary>
    /// <remarks>
    /// This is a low-level function and typically should not be used by call dispatchers. Instead
    /// use the other <see cref="Reply(IXdrCodec)">reply method</see> which just expects a serializable
    /// object to Sends back to the caller.
    /// </remarks>
    /// <exception cref="OncRpcException">  Thrown when an ONC/RPC error condition occurs. </exception>
    /// <param name="state">    ONC/RPC reply message header indicating success or failure and
    ///                         containing associated state information. </param>
    /// <param name="reply">    If not (<see langword="null"/>), then this parameter references the reply to
    ///                         be serialized after the reply message header. </param>
    public virtual void Reply( IXdrCodec reply )
    {
        this.Transport.Reply( reply );
    }

}


namespace cc.isr.XDR;

/// <summary>
/// The class <see cref="XdrException"/> indicates XDR conditions that a reasonable
/// application might want to catch.
/// </summary>
[Serializable]
public class XdrException : Exception
{

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with a reason of <see cref="XdrExceptionReason.XdrFailed"/>.
    /// </summary>
    public XdrException() : this( XdrExceptionReason.XdrFailed )
    {
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail message.
    /// </summary>
    /// <param name="message"> The detail message. </param>
    public XdrException( string message ) : base()
    {
        this.Reason = XdrExceptionReason.XdrFailed;
        this._message = message;
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail reason and message.
    /// </summary>
    /// <param name="reason">  The detail reason. </param>
    /// <param name="message"> The detail message. </param>
    public XdrException( XdrExceptionReason reason, string message ) : base()
    {
        this.Reason = reason;
        this._message = message;
    }

    /// <summary>
    /// Constructs an <see cref="XdrException"/> with the specified detail reason.
    /// </summary>
    /// <remarks>   The detail message is derived automatically from the <paramref name="reason"/>. </remarks>
    /// <param name="reason">   The reason. This can be one of the constants -- oops, that should be
    ///                         "public final static integers" -- defined in this interface. </param>
    public XdrException( XdrExceptionReason reason ) : base()
    {
        this.Reason = reason;
        switch ( reason )
        {
            case XdrExceptionReason.XdrCannotReceive:
                {
                    this._message = "cannot receive XDR data";
                    break;
                }

            case XdrExceptionReason.XdrCannotSend:
                {
                    this._message = "cannot send XDR data";
                    break;
                }

            case XdrExceptionReason.XdrFailed:
                {
                    this._message = "XDR generic failure";
                    break;
                }

            case XdrExceptionReason.XdrBufferOverflow:
                {
                    this._message = "XDR buffer overflow";
                    break;
                }

            case XdrExceptionReason.XdrBufferUnderflow:
                {
                    this._message = "XDR buffer underflow";
                    break;
                }

            case XdrExceptionReason.XdrSuccess:
            default:
                {
                    this._message = string.Empty;
                    break;
                }
        }
    }

    /// <summary>
    /// Specific detail (<see cref="XdrExceptionReason"/>) for this <see cref="XdrException"/>.
    /// </summary>
    /// <value> The reason for this <see cref="XdrException"/> if it was
    /// <see cref="XdrException(XdrExceptionReason)">created</see> with an error reason; or
    /// <see cref="XdrExceptionReason.XdrFailed"/> if it was <see cref="XdrException()">created</see>
    /// without specifying a reason (using the default constructor).
    /// </value>
    /// 
    public XdrExceptionReason Reason { get; private set; }

    /// <summary>
    /// Specific detail about this <see cref="XdrException"/>, like a detailed error message.
    /// </summary>
    private readonly string _message;

    /// <summary>   Returns the error message string of this XDR object. </summary>
    /// <value>
    /// The error message string of this <see cref="XdrException"/>
    /// object if it was created either with an error message string or an XDR error code.
    /// </value>
    public override string Message => this._message;

}

/// <summary>   Values that represent reasons for <see cref="XdrException"/>. </summary>
/// <remarks>   2023-01-07. </remarks>
public enum XdrExceptionReason
{
    /// <summary>   The remote procedure call was carried out successfully. <para>
    /// Renamed from RPC_SUCCESS (=0), which maps to VXI-11 Visa32.VISA.VI_SUCCESS </para></summary>
    XdrSuccess = 0,

    /// <summary>   Encoded information cannot be sent. <para>
    /// Renamed from RPC_CANTSEMD (=3)  </para></summary>
    XdrCannotSend = 1,

    /// <summary>   Information to be decoded cannot be received. <para> 
    /// Renamed from RPC_CANTRECV (=4) </para></summary>
    XdrCannotReceive = 2,

    /// <summary>   A generic XDR exception occurred. <para>
    /// Renamed from RPC_FAILED (=16)  </para></summary>
    XdrFailed = 3,

    /// <summary>
    /// A buffer overflow occurred with an encoding XDR stream. This happens if you use
    /// UDP-based (datagram-based) XDR streams and you try to encode more data than can fit into the
    /// sending buffers. <para>
    /// Renamed from RPC_BUFFEROVERFLOW (=42)  </para></summary>
    XdrBufferOverflow = 4,

    /// <summary>
    /// A buffer underflow occurred with an decoding XDR stream. This happens if you try
    /// to decode more data than was sent by the other communication partner. <para>
    /// Renamed from RPC_BUFFERUNDERFLOW (=43)  </para></summary>
    XdrBufferUnderflow = 5,
}

